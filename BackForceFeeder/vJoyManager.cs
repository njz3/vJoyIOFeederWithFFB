#define USE_RAW_M2PAC_MODE
using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BackForceFeeder.Configuration;
using BackForceFeeder.FFBAgents;
using BackForceFeeder.IOCommAgents;
using BackForceFeeder.Outputs;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;

namespace BackForceFeeder
{
    /// <summary>
    /// Translating mode for force feedback commands
    /// </summary>
    public enum FFBTranslatingModes : int
    {
        /// <summary>
        /// PWM + Dir (Fwd/Rev)
        /// </summary>
        PWM_DIR = 0,
        /// <summary>
        /// Centered PWM signal (50%=0 force)
        /// </summary>
        PWM_CENTERED,

        // COMPATIBILITY MODE

        /// <summary>
        /// Indy Model 2/Touring car/Le mans drive board
        /// </summary>
        COMP_M2_INDY_STC,
        /// <summary>
        /// Le mans drive board
        /// </summary>
        COMP_M3_LEMANS,
        /// <summary>
        /// Scud Race Model 3 drive board
        /// </summary>
        COMP_M3_SCUD,
        /// <summary>
        /// Scud Race Model 3 drive board
        /// </summary>
        COMP_M3_SR2,
        /// <summary>
        /// Model 3 generic drive board (unknown EEPROM)
        /// Use parallel port communication (8bits TX, 8bits RX)
        /// All Effects emulated using constant torque effect
        /// with codes 0x50 and 0x60.
        /// </summary>
        COMP_M3_UNKNOWN = 100,

#if USE_RAW_M2PAC_MODE
        /// <summary>
        /// RAW M2pac mode : raw sending of drive board command
        /// WARNING: on non compatible board it will not work!
        /// Compatible games:
        /// - Indy Model 2/Touring car/Le mans
        /// - Scud Race/Daytona2/Emergency Call Ambulance/Dirt Devil
        /// </summary>
        RAW_M2PAC_MODE,
#endif
        /// <summary>
        /// Lindbergh RS422 drive board through RS232
        /// </summary>
        //LINDBERGH_GENERIC_DRVBD = 300,

    }

    public class vJoyManager
    {
        /// <summary>
        /// Manager configuration - only when instance
        /// </summary>
        public static FeederConfig Config = new FeederConfig();

        /// <summary>
        /// vJoy abstraction layer
        /// </summary>
        public vJoyFeeder vJoy = null;
        /// <summary>
        /// IO abstraction layer
        /// </summary>
        public USBSerialIO IOboard = null;
        /// <summary>
        /// Force feedback management/computations layer
        /// </summary>
        public AFFBManager FFB = null;
        /// <summary>
        /// Output from emulators
        /// </summary>
        public OutputsManager Outputs = null;
        /// <summary>
        /// Global refresh period for whole application, includes
        /// serial port comm + FFB computation.
        /// This needs to be tuned!
        /// </summary>
        public const int GlobalRefreshPeriod_ms = 5;

        /// <summary>
        /// 1 = every tick/period
        /// 2 = every 2 ticks/periods
        /// n = every n
        /// </summary>
        public const int vJoyUpdate = 2; //5*2 = 10ms

        /// <summary>
        /// Raw inputs (up to 32)
        /// </summary>
        public UInt64 RawInputsStates = 0;

        /// <summary>
        /// Raw outputs (up to 32)
        /// </summary>
        public UInt32 RawOutputsStates = 0;

        public UInt32 RawLampOutput = 0;
        public UInt32 RawDriveOutput = 0;

        protected bool Running = false;
        protected Thread ManagerThread = null;
        protected Thread ProcessScannerThread = null;
        protected ulong TickCount = 0;

        public bool IsRunning { get { return Running; } }

        public vJoyManager()
        {
        }


        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[MANAGER] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[MANAGER] " + text, args);
        }

        public void Start()
        {
            if (Running) return;
            if (ManagerThread != null) Stop();


            ManagerThread = new Thread(ManagerThreadMethod);
            ProcessScannerThread = new Thread(ProcessScannerThreadMethod);
            Running = true;
            ManagerThread.Name = "vJoy Manager";
            ManagerThread.Priority = ThreadPriority.Normal;
            ManagerThread.Start();
            ProcessScannerThread.Name = "Process Scanner";
            ProcessScannerThread.Priority = ThreadPriority.Lowest;
            ProcessScannerThread.Start();
        }
        public void Stop()
        {
            if (!Running) return;

            Running = false;
            if (ManagerThread == null)
                return;
            Thread.Sleep(GlobalRefreshPeriod_ms * 10);
            ManagerThread.Join(1000);
            ProcessScannerThread.Join(2000);
            ManagerThread = null;
            ProcessScannerThread = null;
        }

        public void CheckControlSet(ControlSetDB cs)
        {
            if (cs==null || this.vJoy==null)
                return;
            bool modified = false;
            // Check axis
            if (cs.vJoyMapping.RawAxisTovJoyDB.Count<this.vJoy.NbUsedAxis) {
                for (int i = cs.vJoyMapping.RawAxisTovJoyDB.Count; i<this.vJoy.NbUsedAxis; i++) {
                    RawAxisDB newDB = new RawAxisDB();
                    newDB.MappedIndexUsedvJoyAxis = i;
                    cs.vJoyMapping.RawAxisTovJoyDB.Add(newDB);
                    modified = true;
                }
            }
            // Check each rawdb and correct index and control point
            for (int i = 0; i<cs.vJoyMapping.RawAxisTovJoyDB.Count; i++) {
                var rawdb = cs.vJoyMapping.RawAxisTovJoyDB[i];
                rawdb.MappedIndexUsedvJoyAxis = i;
                if (rawdb.ControlPoints.Count<2) {
                    rawdb.ResetCorrectionFactors();
                    modified = true;
                }
            }
            // Ensure all inputs are defined, else add missing
            for (int i = cs.vJoyMapping.RawInputTovJoyMap.Count; i<vJoyFeeder.MAX_BUTTONS_VJOY; i++) {
                var db = new RawInputDB();
                db.MappedvJoyBtns = new List<int>(1) { i };
                cs.vJoyMapping.RawInputTovJoyMap.Add(db);
                modified = true;
            }

            // Ensure all outputs are defined, else add missing
            for (int i = cs.RawOutputBitMap.Count; i<16; i++) {
                var db = new RawOutputDB();
                db.MappedRawOutputBit = new List<int>(1) { i + 8 };
                cs.RawOutputBitMap.Add(db);
                modified = true;
            }
            if (modified) {
                Log("Sanity check for control set " + cs.UniqueName + ": fixed config issues!", LogLevels.IMPORTANT);
            } else {
                Log("Sanity check for control set " + cs.UniqueName + ": ok", LogLevels.DEBUG);
            }
        }

        public bool InitIOBoard(USBSerialIO ioboard)
        {
            if (ioboard==null) {
                Log("No IO board connected, cannot initialize hardware", LogLevels.ERROR);
                return false;
            }
            Log("Initializing IO board", LogLevels.IMPORTANT);
            // Initialize board
            ioboard.PerformInit();
            // Enable safety watchdog
            ioboard.EnableWD();
            if (Config.Hardware.UseStreamingMode) {
                // Enable auto-streaming
                ioboard.StartStreaming();
            }
            // Set configuration in compatible boards
            if (ioboard.ProtocolVersionReceived) {
                byte pwmmode = 0; // Standard PWM
                if (Config.Hardware.TranslatingModes == FFBTranslatingModes.PWM_CENTERED) {
                    pwmmode |= 1<<0; // Notify centered PWM computations
                }
                if (Config.Hardware.DualModePWM) {
                    pwmmode |= 1<<1; // Dual PWM on D9/D10
                }
                if (Config.Hardware.DigitalPWM) {
                    pwmmode |= 1<<2; // Digital PWM on serial port
                }
                Log("Configuring IO board for pwmmode=" + pwmmode.ToString("X"), LogLevels.INFORMATIVE);
                ioboard.SetParameter("pwmmode", pwmmode);

                byte wheelmode = 2;
                Log("Configuring IO board for wheelmode=" + wheelmode.ToString("X"), LogLevels.INFORMATIVE);
                ioboard.SetParameter("wheelmode", wheelmode); // Filtered value

                byte pedalmode = 0;
                Log("Configuring IO board for pedalmode=" + pwmmode.ToString("X"), LogLevels.INFORMATIVE);
                ioboard.SetParameter("pedalmode", pedalmode); // No option

                byte ffbcontrollermode = 0;
                if (Config.Hardware.AlternativePinFFBController) {
                    ffbcontrollermode = 1; // Set FFB shield as present
                } else {
                    ffbcontrollermode = 0; // No option
                }

                Log("Configuring IO board for ffbcontrollermode=" + ffbcontrollermode.ToString("X"), LogLevels.INFORMATIVE);
                ioboard.SetParameter("ffbcontrollermode", ffbcontrollermode); // No option
            }
            return true;
        }


        protected void ManagerThreadMethod()
        {
            __restart:

            if (Config.Application.OutputOnly) {
                Log("Program configured for output only (no FFB, no vJoy)", LogLevels.IMPORTANT);
                vJoy = null;
            } else {
                Log("Program configured for " + Config.Hardware.TranslatingModes, LogLevels.IMPORTANT);
                vJoy = new vJoyFeeder();
            }

            IOboard = null;
            var boards = USBSerialIO.ScanAllCOMPortsForIOBoards();
            if (boards.Length > 0) {
                IOboard = boards[0];
                Log("Found io board on " + IOboard.COMPortName + " version=" + IOboard.BoardVersion + " type=" + IOboard.BoardDescription, LogLevels.IMPORTANT);
            } else {
                IOboard = null;
                if (Config.Application.RunWithoutIOBoard) {
                    Log("No boards found! Continue without real hardware", LogLevels.ERROR);
                } else {
                    Log("No boards found! Thread will terminate", LogLevels.ERROR);
                    Running = false;
                    return;
                }
            }

            if (!Config.Application.OutputOnly) {
                switch (Config.Hardware.TranslatingModes) {
                    case FFBTranslatingModes.PWM_CENTERED:
                    case FFBTranslatingModes.PWM_DIR: {
                            FFB = new FFBManagerTorque(GlobalRefreshPeriod_ms);
                        }
                        break;
                    case FFBTranslatingModes.COMP_M3_UNKNOWN: {
                            // Default to Scud/Daytona2
                            FFB = new FFBManagerModel3Scud(GlobalRefreshPeriod_ms);
                        }
                        break;
                    case FFBTranslatingModes.COMP_M2_INDY_STC:
                    case FFBTranslatingModes.COMP_M3_LEMANS: {
                            FFB = new FFBManagerModel3Lemans(GlobalRefreshPeriod_ms);
                        }
                        break;
                    case FFBTranslatingModes.COMP_M3_SCUD: {
                            FFB = new FFBManagerModel3Scud(GlobalRefreshPeriod_ms*2);
                        }
                        break;
                    case FFBTranslatingModes.COMP_M3_SR2: {
                            FFB = new FFBManagerModel3SegaRally2(GlobalRefreshPeriod_ms*2);
                        }
                        break;
#if USE_RAW_M2PAC_MODE
                    case FFBTranslatingModes.RAW_M2PAC_MODE: {
                            FFB = new FFBManagerRawModel23(GlobalRefreshPeriod_ms);
                        }
                        break;
#endif
                    default:
                        throw new NotImplementedException("Unsupported FFB mode " + Config.Hardware.TranslatingModes.ToString());
                }
            }

            // Use this to allow 1ms sleep granularity (else default is 16ms!!!)
            // This consumes more CPU cycles in the OS, but does improve
            // a lot reactivity when soft real-time work needs to be done.
            MultimediaTimer.SetTickGranularityOnWindows();

            // Output system for drvbd/lamps (always created)
            Outputs = new OutputsManager();
            Outputs.Start();

            // vJoy
            if (vJoy!=null) {
                vJoy.EnablevJoy(); // Create joystick interface
                vJoy.Acquire(1); // Use first enumerated vJoy device
                vJoy.StartAndRegisterFFB(FFB); // Start FFB callback mechanism in vJoy
            }

            // In case we want to use XInput/DInput devices to gather multiple inputs?
            //XInput();
            //DirectInput();

            // IOBoad
            InitIOBoard(IOboard);

            if (Config.Application.VerbosevJoyManager) {
                Log("Start feeding...");
            }

            // Start FFB manager
            if (FFB!=null)
                FFB.Start();

            // Internal values for special operation
            double prev_angle = 0.0;

            UInt32 autofire_mode_on = 0;

            HShifterDecoder HShifter = new HShifterDecoder();
            UpDnShifterDecoder UpDnShifter = new UpDnShifterDecoder();

            int HShifterCurrent = 0;
            int UpDownShifterCurrent = 0;

            uint error_counter = 0;
            UInt64 nextRun_ms = (UInt64)(MultimediaTimer.RefTimer.ElapsedMilliseconds);

            while (Running) {
                TickCount++;

                nextRun_ms += GlobalRefreshPeriod_ms;
                UInt64 now = (UInt64)(MultimediaTimer.RefTimer.ElapsedMilliseconds);
                int delay_ms = (int)(nextRun_ms-now);
                if (delay_ms>=0) {
                    // Sleep until next tick
                    Thread.Sleep(delay_ms);
                } else {
                    if (Config.Application.VerbosevJoyManager) {
                        Log("One period missed by " + (-delay_ms) + "ms", LogLevels.DEBUG);
                    }
                    // If not in streaming, then reloop immediatly
                    if (!Config.Hardware.UseStreamingMode) {
                        continue;
                    } else {
                        // Add as many periods as delayed
                        int periods = (-delay_ms)/GlobalRefreshPeriod_ms;
                        nextRun_ms += (ulong)(GlobalRefreshPeriod_ms*periods);
                    }
                }

                #region Output retrieving from game

                // First get outputs from Lamps/other
                if (Outputs!=null) {

                    int lamps = Outputs.GetLampsOutputs();
                    if (lamps>=0) {
                        // Detect change
                        if (lamps!=RawLampOutput) {
                            RawLampOutput = (UInt32)lamps;
                            Log("Lamps=" + RawLampOutput.ToString("X"), LogLevels.INFORMATIVE);

                            // Decode lamps: use mapping to set raw bits accordingly
                            var rawoutputbitmap = Config.CurrentControlSet.RawOutputBitMap;
                            for (int idxbit = 0; idxbit<rawoutputbitmap.Count; idxbit++) {
                                // Single bit value of the lamp : on/off state
                                var rawLampBitValue = (RawLampOutput & (1<<idxbit))!=0;
                                // List of final bit position(s) in digital output word
                                var bitsToChange = rawoutputbitmap[idxbit].MappedRawOutputBit;
                                for (int idxout = 0; idxout<bitsToChange.Count; idxout++) {
                                    // Get single final bit position
                                    int finalbitpos = bitsToChange[idxout];
                                    // Raw state value with inverted logic
                                    bool state;
                                    if (rawoutputbitmap[idxbit].IsInvertedLogic)
                                        state = !rawLampBitValue;
                                    else {
                                        state = rawLampBitValue;
                                    }


                                    var bitmask = (uint)(1<<finalbitpos);
                                    // Set or clear bit depending on logic
                                    if (state) {
                                        this.RawOutputsStates |= bitmask;
                                    } else {
                                        this.RawOutputsStates &= (uint)~bitmask;
                                    }
                                    /*
                                    // Split per 8bit (byte) word
                                    if (finalbitpos<8) {
                                        // Create bitmask
                                        var bitmask = (byte)(1<<finalbitpos);
                                        if (IOboard.DigitalOutputs8.Length>1) {
                                            // Set or clear bit depending on logic
                                            if (state) {
                                                IOboard.DigitalOutputs8[0] |= bitmask;
                                            } else {
                                                IOboard.DigitalOutputs8[0] &= (byte)~bitmask;
                                            }
                                        }
                                    } else {
                                        // Ensure we have enough outputs
                                        if (IOboard.DigitalOutputs8.Length>1) {
                                            // Create bitmask
                                            var bitmask = (byte)(1<<(finalbitpos-8));
                                            // Set or clear bit
                                            if (state) {
                                                IOboard.DigitalOutputs8[0] |= bitmask;
                                            } else {
                                                IOboard.DigitalOutputs8[0] &= (byte)~bitmask;
                                            }
                                        }
                                    }*/
                                }
                            }

                        }
                    } else {
                        // Error, no lamps detected
                        this.RawOutputsStates = 0;
                    }

                    // Driveboard outputs
                    int drive = Outputs.GetRawDriveOutputs();
                    if (drive>=0) {
                        if (drive!=RawDriveOutput) {
                            RawDriveOutput = (UInt32)drive;
                            Log("Drive=" + RawDriveOutput.ToString("X"), LogLevels.INFORMATIVE);
                            // M2PAC mode : save value from game, but it will be overwritten
                            // depending on the FFB mode selected.
                            // Clear 8 bits for driveboard
                            this.RawOutputsStates &= ~(uint)(0xFF<<8);
                            // Set new bits for driveboard
                            this.RawOutputsStates |= (RawDriveOutput&0xFF)<<8;
                        }
                    }
                }
                #endregion


                if (IOboard != null) {
                    try {
                        if (IOboard.IsOpen) {

                            #region Inputs - vJoy
                            if (vJoy!=null) {
                                #region Serial read from Arduino gateway

                                // Shift tick to synch with IOboard
                                var before = MultimediaTimer.RefTimer.ElapsedMilliseconds;
                                int nbproc = 0;
                                // Empty serial buffer
                                if (Config.Hardware.UseStreamingMode) {
                                    if (delay_ms<0) {
                                        // Update status on received packets
                                        nbproc = IOboard.UpdateOnStreaming(Math.Max(10, 10 + (-delay_ms)/GlobalRefreshPeriod_ms));
                                    } else {
                                        // Wait for a packet
                                        nbproc = IOboard.UpdateOnStreaming();
                                    }
                                } else {
                                    // Wait for a packet
                                    nbproc = IOboard.UpdateOnStreaming();
                                    // Than ask for next packet
                                    IOboard.SendUpdate();
                                }
                                var after = MultimediaTimer.RefTimer.ElapsedMilliseconds;
                                // Delay is expected to be less than 1-2ms for processing stream
                                delay_ms =  (int)(after-before);
                                // Accept up to 2ms of delay (jitter), else consider we have a wrong
                                // tick alignment with IO board
                                if (delay_ms>2 && nbproc>=1) {
                                    var add_delay = Math.Min(GlobalRefreshPeriod_ms-1, delay_ms-1);
                                    add_delay = 1;
                                    nextRun_ms += (ulong)add_delay;
                                    if (Config.Application.VerbosevJoyManager) {
                                        Log("Read took " + delay_ms + "ms delay, adding " + add_delay + "ms to sync with IO board serial port", LogLevels.DEBUG);
                                    }
                                }

                                if (Config.Application.VerbosevJoyManager) {
                                    if (nbproc>1) {
                                        Log("Processed " + nbproc + " msg instead of 1", LogLevels.DEBUG);
                                    }
                                }
                                #endregion

                                #region Wheel angle and pedals
                                // Refresh wheel angle (between -1...1)
                                if (IOboard.AnalogInputs.Length > 0) {
                                    // Scale analog input in cts between 0..0xFFF, then map it to -1/+1, 0 being center
                                    var angle_u = ((double)IOboard.AnalogInputs[0] * Config.Hardware.WheelScaleFactor_u_per_cts) - Config.Hardware.WheelCenterOffset_u;

                                    // Refresh values in FFB manager
                                    if (IOboard.WheelStates.Length > 0) {
                                        // If full state given by IO board (should be in cts_per_s or cts_per_s2!)
                                        FFB.RefreshCurrentState(angle_u,
                                            IOboard.WheelStates[0]* Config.Hardware.WheelScaleFactor_u_per_cts,
                                            IOboard.WheelStates[1]* Config.Hardware.WheelScaleFactor_u_per_cts);
                                    } else {
                                        // If only periodic position
                                        FFB.RefreshCurrentPosition(angle_u);
                                    }
                                    prev_angle = angle_u;
                                }

                                // For debugging purpose, add a 4th axis to display torque output
                                uint[] rawAxisValues12bits = new uint[5];
                                IOboard.AnalogInputs.CopyTo(rawAxisValues12bits, 0);
                                rawAxisValues12bits[4] = (uint)(FFB.OutputTorqueLevel * 0x7FF + 0x800);

                                // Set values into vJoy report:
                                // - axes
                                vJoy.UpdateAxes12bits(rawAxisValues12bits);
                                #endregion

                                #region Buttons, inputs, shifter decoders

                                // - buttons (only32 supported for now)
                                if (IOboard.DigitalInputs8.Length > 0) {

                                    // HShifter decoder map - Not yet done
                                    RawInputDB[] HShifterDecoderMap = new RawInputDB[3];
                                    bool[] HShifterPressedMap = new bool[3];
                                    // Up/Down shifter decoder map - Not yet done
                                    RawInputDB[] UpDownShifterDecoderMap = new RawInputDB[2];
                                    bool[] UpDownShifterPressedMap = new bool[2];


                                    // New raw input state
                                    UInt64 rawinput_states = 0;

                                    // Raw index (increasing for each din, over all blocks)
                                    int rawidx = 0;
                                    // For each single input, process mapping, autofire and toggle
                                    for (int idxDin = 0; idxDin<Math.Min(4, IOboard.DigitalInputs8.Length); idxDin++) {

                                        // Scan 8bit input block, increase each time the raw index
                                        for (int j = 0; j<8; j++, rawidx++) {
                                            // Get configuration of this raw input
                                            var rawdb = Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx];

                                            // Default input value is current logic (false if not inverted)
                                            bool newrawval = rawdb.IsInvertedLogic;

                                            // Check if input is "on" and invert default value
                                            if ((IOboard.DigitalInputs8[idxDin] & (1<<j))!=0) {
                                                // If was false, then set true
                                                newrawval = !newrawval;
                                            }
                                            // Now newrawval is the raw state of the input taking into account inv.logic

                                            // Bit corresponding to this input
                                            var rawbit = (UInt32)(1<<rawidx);
                                            // Store new state of raw input
                                            if (newrawval) {
                                                rawinput_states |= rawbit;
                                            }

                                            // Previous state of this input (for transition detection)
                                            var prev_state = (RawInputsStates&rawbit)!=0;

                                            //-----------------------------------------------
                                            // Perform vJoy button set depending on type
                                            //-----------------------------------------------

                                            // Check if we toggle the bit (or autofire mode)
                                            if (rawdb.IsToggle) {
                                                // Toggle only if we detect a false->true transition in raw value
                                                if (newrawval && (!prev_state)) {
                                                    // Toggle = xor on every vJoy buttons
                                                    vJoy.ToggleButtons(rawdb.MappedvJoyBtns);
                                                }
                                            } else if (rawdb.IsAutoFire) {
                                                // Autofire set, if false->true transition, then toggle autofire state
                                                if (newrawval && (!prev_state)) {
                                                    // Enable/disable autofire
                                                    autofire_mode_on ^= rawbit;
                                                }
                                                // No perform autofire toggle if autofire enabled
                                                if ((autofire_mode_on&rawbit)!=0) {
                                                    // Toggle = xor every n periods
                                                    ulong n = (ulong)(Config.CurrentControlSet.vJoyMapping.AutoFirePeriod_ms/GlobalRefreshPeriod_ms);
                                                    if ((TickCount%n)==0) {
                                                        vJoy.ToggleButtons(rawdb.MappedvJoyBtns);
                                                    }
                                                }
                                            } else if (rawdb.IsSequencedvJoy) {
                                                // Sequenced vJoy buttons - every rising edge, will trigger a new vJoy
                                                // if false->true transition, then toggle vJoy and move index
                                                if (newrawval && (!prev_state)) {
                                                    // Clear previous button first
                                                    vJoy.Clear1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                    // Move indexer
                                                    rawdb.SequenceCurrentToSet++;
                                                    if (rawdb.SequenceCurrentToSet>=rawdb.MappedvJoyBtns.Count) {
                                                        rawdb.SequenceCurrentToSet = 0;
                                                    }
                                                    if (rawdb.MappedvJoyBtns.Count<1)
                                                        continue;
                                                    // Set only indexed one
                                                    vJoy.Set1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                }
                                            } else if (rawdb.ShifterDecoder!= ShifterDecoderMap.No) {
                                                // Part of HShifter decoder map, just save the values
                                                switch (rawdb.ShifterDecoder) {
                                                    case ShifterDecoderMap.HShifterLeftRight:
                                                    case ShifterDecoderMap.HShifterUp:
                                                    case ShifterDecoderMap.HShifterDown:
                                                        // rawdb
                                                        HShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = rawdb;
                                                        // state of raw input
                                                        HShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = newrawval;
                                                        break;
                                                    case ShifterDecoderMap.SequencialUp:
                                                    case ShifterDecoderMap.SequencialDown:
                                                        // rawdb
                                                        UpDownShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = rawdb;
                                                        // state of raw input
                                                        UpDownShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = newrawval;
                                                        break;
                                                }
                                            } else {
                                                // Nothing specific : perform simple mask set or clear button
                                                if (newrawval) {
                                                    vJoy.SetButtons(Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx].MappedvJoyBtns);
                                                } else {
                                                    vJoy.ClearButtons(Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx].MappedvJoyBtns);
                                                }
                                            }

                                        }

                                    }

                                    // Decode HShifter map (if used)
                                    if (HShifterDecoderMap[0]!=null && HShifterDecoderMap[1]!=null && HShifterDecoderMap[2]!=null) {
                                        // First left/right switch pressed?
                                        HShifter.HSHifterLeftRightPressed = HShifterPressedMap[0];
                                        // Up pressed?
                                        HShifter.UpPressed = HShifterPressedMap[1];
                                        // Down pressed?
                                        HShifter.DownPressed = HShifterPressedMap[2];
                                        // Now get decoded value
                                        int selectedshift = HShifter.CurrentShift; //0=neutral

                                        // Detect change
                                        if (selectedshift!=HShifterCurrent) {
                                            Log("HShifter decoder from=" + HShifterCurrent + " to " + selectedshift, LogLevels.DEBUG);
                                            HShifterCurrent = selectedshift;
                                            var rawdb = HShifterDecoderMap[0];
                                            if (rawdb.MappedvJoyBtns.Count>0) {
                                                // Clear previous buttons first
                                                if (rawdb.SequenceCurrentToSet>=0 && rawdb.SequenceCurrentToSet<rawdb.MappedvJoyBtns.Count) {
                                                    vJoy.Clear1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                }
                                                // Set indexer to new shift value
                                                if (rawdb.IsNeutralFirstBtn) {
                                                    // Neutral is first button
                                                    rawdb.SequenceCurrentToSet = HShifterCurrent;
                                                } else {
                                                    // Neutral is not a button
                                                    rawdb.SequenceCurrentToSet = HShifterCurrent-1;
                                                }
                                                // Check min/max
                                                if (rawdb.SequenceCurrentToSet>=rawdb.MappedvJoyBtns.Count) {
                                                    rawdb.SequenceCurrentToSet = rawdb.MappedvJoyBtns.Count-1;
                                                }
                                                if (rawdb.SequenceCurrentToSet>=0) {
                                                    // Set only indexed one
                                                    vJoy.Set1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                }
                                            }
                                        }
                                    }

                                    // Decode Up/Down shifter map
                                    if (UpDownShifterDecoderMap[0]!=null && UpDownShifterDecoderMap[1]!=null) {
                                        //UpDnShifter.MaxShift = UpDownShifterDecoderMap[0].SequenceCurrentToSet;
                                        //UpDnShifter.MinShift = UpDownShifterDecoderMap[1].SequenceCurrentToSet;
                                        UpDnShifter.ValidateDelay_ms = (ulong)Config.CurrentControlSet.vJoyMapping.UpDownDelay_ms;
                                        // Up pressed?
                                        UpDnShifter.UpPressed = UpDownShifterPressedMap[0];
                                        // Down pressed?
                                        UpDnShifter.DownPressed = UpDownShifterPressedMap[1];
                                        // Now get decoded value
                                        int selectedshift = UpDnShifter.CurrentShift; //0=neutral

                                        // Detect change
                                        if (selectedshift!=UpDownShifterCurrent) {
                                            Log("UpDnShifter decoder from=" + UpDownShifterCurrent + " to " + selectedshift, LogLevels.DEBUG);
                                            UpDownShifterCurrent = selectedshift;
                                            var rawdb = UpDownShifterDecoderMap[0];
                                            if (rawdb.MappedvJoyBtns.Count>0) {
                                                // Clear all buttons first
                                                if (rawdb.SequenceCurrentToSet>=0 && rawdb.SequenceCurrentToSet<rawdb.MappedvJoyBtns.Count) {
                                                    vJoy.Clear1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                }
                                                // Update max shift, just in cast
                                                UpDnShifter.MaxShift = rawdb.MappedvJoyBtns.Count;
                                                // Set indexer to new shift value
                                                if (rawdb.IsNeutralFirstBtn) {
                                                    // Neutral is first button
                                                    rawdb.SequenceCurrentToSet = UpDownShifterCurrent;
                                                } else {
                                                    // Neutral is not a button
                                                    rawdb.SequenceCurrentToSet = UpDownShifterCurrent-1;
                                                }
                                                // Check min/max
                                                if (rawdb.SequenceCurrentToSet>=rawdb.MappedvJoyBtns.Count) {
                                                    rawdb.SequenceCurrentToSet = rawdb.MappedvJoyBtns.Count-1;
                                                }
                                                if (rawdb.SequenceCurrentToSet>=0) {
                                                    // Set only indexed one
                                                    vJoy.Set1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                                                }
                                            }
                                        }
                                    }

                                    // Save raw input state for next run
                                    RawInputsStates = rawinput_states;
                                }

                                // - 360deg POV to view for wheel angle
                                //vJoy.UpodateContinuousPOV((uint)((IOboard.AnalogInputs[0] / (double)0xFFF) * 35900.0) + 18000);

                                // Update vJoy and send to driver every n ticks to limit workload on driver
                                if ((TickCount % vJoyUpdate) == 0) {
                                    vJoy.PublishiReport();
                                }

                                #endregion
                            }
                            #endregion

                            #region Copy lamps & outputs from RawOutputsStates to IOboard
                            // /!\ Outputs block are in reverse order, from most important to less
                            //-  Block[0]: reserved for control of actuators, like fwd/rev direction, ...
                            // - Block[1]: reserved for lamps (on mega2560)
                            //-  Block[2]: reserved for driveboard communication (mega2560)
                            // Raw lamps output will actually be configured to map either of these
                            // two first blocks

                            // Save to outputs skipping the first outputblock (managed for direction)
                            for (int i = 0; i<IOboard.DigitalOutputs8.Length-1; i++) {
                                var shift = (i<<3);
                                IOboard.DigitalOutputs8[i+1] = (byte)((this.RawOutputsStates>>shift)&0xFF);
                            }
                            #endregion

                            #region Torque output (if FFB enabled). This will overwrite DigitalOutputs[0/2]
                            if (FFB!=null) {
                                // Now output torque to Pwm+Dir or drive board command - this can overwrite
                                // lamps data depending on hardware translation
                                switch (Config.Hardware.TranslatingModes) {
                                    // PWM centered mode (50% = 0 torque)
                                    case FFBTranslatingModes.PWM_CENTERED: {
                                            // Latch a copy
                                            var outlevel = FFB.OutputTorqueLevel;
                                            // Enforce range again to be [-1; 1]
                                            outlevel = Math.Min(1.0, Math.Max(outlevel, -1.0));
                                            UInt16 analogOut = (UInt16)(outlevel * 0x7FF + 0x800);
                                            IOboard.AnalogOutputs[0] = analogOut; // PWM
                                        }
                                        break;
                                    // PWM+dir mode (0% = 0 torque, direction given by first output)
                                    case FFBTranslatingModes.PWM_DIR: {
                                            // Latch a copy
                                            var outlevel = FFB.OutputTorqueLevel;
                                            if (outlevel >= 0.0) {
                                                UInt16 analogOut = (UInt16)(outlevel * 0xFFF);
                                                // Save into IOboard
                                                IOboard.AnalogOutputs[0] = analogOut; // PWM
                                                IOboard.DigitalOutputs8[0] |= 1<<0; // set FwdCmd bit 0
                                                IOboard.DigitalOutputs8[0] &= 0xFD; // clear RevCmd bit 1
                                            } else {
                                                UInt16 analogOut = (UInt16)(-outlevel * 0xFFF);
                                                // Save into IOboard
                                                IOboard.AnalogOutputs[0] = analogOut; // PWM
                                                IOboard.DigitalOutputs8[0] |= 1<<1; // set RevCmd bit 1
                                                IOboard.DigitalOutputs8[0] &= 0xFE; // clear FwdCmd bit 0
                                            }
                                        }
                                        break;
                                    // Driveboard translation mode
                                    case FFBTranslatingModes.COMP_M3_UNKNOWN:
                                    case FFBTranslatingModes.COMP_M2_INDY_STC:
                                    case FFBTranslatingModes.COMP_M3_LEMANS:
                                    case FFBTranslatingModes.COMP_M3_SCUD:
                                    case FFBTranslatingModes.COMP_M3_SR2: {
                                            // Latch a copy
                                            var outlevel = FFB.OutputEffectCommand;
                                            // Save driveboard command code
                                            if (IOboard.DigitalOutputs8.Length > 2) {
                                                IOboard.DigitalOutputs8[2] = (byte)(outlevel & 0xFF);
                                            }
                                        }
                                        break;
                                    case FFBTranslatingModes.RAW_M2PAC_MODE: {
                                            // Latch a copy
                                            var outlevel = RawDriveOutput;
                                            // Save driveboard command code
                                            if (IOboard.DigitalOutputs8.Length > 2) {
                                                IOboard.DigitalOutputs8[2] = (byte)(outlevel & 0xFF);
                                            }
                                        }
                                        break;
                                }
                            }
                            #endregion

                            // Save output state for GUI - only lamps and driveboards
                            this.RawOutputsStates = 0;
                            for (int i = 0; i<IOboard.DigitalOutputs8.Length-1; i++) {
                                var shift = (i<<3);
                                this.RawOutputsStates |= (UInt32)(IOboard.DigitalOutputs8[i+1]<<shift);
                            }

                            // Send all outputs - this will revive the watchdog!
                            IOboard.SendOutputs();

                        } else {
                            Log("Re-connecting to same IO board on port " + IOboard.COMPortName, LogLevels.IMPORTANT);
                            IOboard.OpenComm();
                            // Enable safety watchdog
                            IOboard.EnableWD();
                            // Enable auto-streaming
                            if (Config.Hardware.UseStreamingMode) {
                                IOboard.StartStreaming();
                            }
                            error_counter = 0;
                        }
                    } catch (Exception ex) {
                        Log("IO board Failing with " + ex.Message, LogLevels.ERROR);
                        // Ensure current control set is not missing elements
                        CheckControlSet(vJoyManager.Config.CurrentControlSet);
                        // Then verify communication
                        try {
                            if (IOboard.IsOpen)
                                IOboard.CloseComm();
                        } catch (Exception ex2) {
                            Log("Unable to close communication " + ex2.Message, LogLevels.ERROR);
                        }
                        error_counter++;
                        if (error_counter > 10) {
                            // Serious problem here, try complete restart with scanning
                            FFB.Stop();
                            goto __restart;
                        }
                        System.Threading.Thread.Sleep(500);
                    }
                }

            };

            MultimediaTimer.RestoreTickGranularityOnWindows();

            if (Outputs!=null)
                Outputs.Stop();
            if (FFB!=null)
                FFB.Stop();
            if (IOboard != null)
                IOboard.CloseComm();
            if (vJoy!=null)
                vJoy.Release();
        }



        Process LastKnownProcess = null;

        protected void ProcessScannerThreadMethod()
        {
            List<Tuple<string, string>> namesAndTitle = new List<Tuple<string, string>>();

            uint tick_cnt = 0;
            while (Running) {
                // Slow period to wait for other app
                Thread.Sleep(500);
                tick_cnt++;

                if (!vJoyManager.Config.Application.AutodetectControlSetAtRuntime) {
                    LastKnownProcess = null;
                    continue;
                }

                // First ensure current process still exists!
                if (LastKnownProcess!=null) {
                    // If exited, then we will scan again
                    if (LastKnownProcess.HasExited) {
                        LastKnownProcess = null;
                    } else {
                        //continue;
                    }
                }

                // Run check every 5s (10 ticks)
                if (tick_cnt%10==0) {
                    namesAndTitle.Clear();
                    int currentidx = -1;
                    // Loop on control sets and build list of known process/title
                    for (int i = 0; i<vJoyManager.Config.AllControlSets.ControlSets.Count; i++) {
                        var cs = vJoyManager.Config.AllControlSets.ControlSets[i];
                        namesAndTitle.Add(new Tuple<string, string>(cs.ProcessDescriptor.ProcessName, cs.ProcessDescriptor.MainWindowTitle));
                        if (cs == vJoyManager.Config.CurrentControlSet)
                            currentidx = i;
                    }

                    // Scan processes and main windows title
                    var found = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(namesAndTitle, true, false);
                    // Store detected profile
                    if (found.Count>0) {
                        for (int i = 0; i<found.Count; i++) {
                            int idx = found[i].Item2;
                            var cs = vJoyManager.Config.AllControlSets.ControlSets[idx];
                            if (vJoyManager.Config.Application.VerboseScanner) {
                                Log("Scanner found " + found[i].Item1.ProcessName + " main window " + found[i].Item1.MainWindowTitle + " matched control set " + cs.UniqueName, LogLevels.DEBUG);
                            }
                        }
                        // Pick first
                        var newproc = found[0].Item1;
                        var newidx = found[0].Item2;
                        if ((currentidx!=newidx) ||
                            (LastKnownProcess==null) ||
                            (newproc.Id != LastKnownProcess.Id) ||
                            (newproc.MainWindowTitle != LastKnownProcess.MainWindowTitle)) {

                            LastKnownProcess = newproc;
                            var cs = vJoyManager.Config.AllControlSets.ControlSets[newidx];
                            CheckControlSet(cs);
                            vJoyManager.Config.CurrentControlSet = cs;
                            Log("Detected " + LastKnownProcess.ProcessName + " (" + LastKnownProcess.MainWindowTitle + "), auto-switching to control set " + cs.UniqueName, LogLevels.IMPORTANT);
                        }
                    }
                }
            }
        }






        public void DirectInput()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Gamepad Guid first
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                joystickGuid = deviceInstance.InstanceGuid;
            }

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty) {
                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                    joystickGuid = deviceInstance.InstanceGuid;
                }
            }

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty) {
                Log("No joystick/Gamepad found.", LogLevels.ERROR);
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);

            Log(String.Format("Found Joystick/Gamepad with GUID: {0}", joystickGuid), LogLevels.INFORMATIVE);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Log(String.Format("Effect available {0}", effectInfo.Name), LogLevels.INFORMATIVE);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true) {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                    Log(state.ToString(), LogLevels.INFORMATIVE);
            }
        }

        public void XInput()
        {
            Log("Start XGamepadApp");
            // Initialize XInput
            var controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };

            // Get 1st controller available
            Controller controller = null;
            foreach (var selectControler in controllers) {
                if (selectControler.IsConnected) {
                    controller = selectControler;
                    break;
                }
            }

            if (controller == null) {
                Log("No XInput controller installed");
            } else {

                Log("Found a XInput controller available");
                Log("Press buttons on the controller to display events or escape key to exit... ");

                // Poll events from joystick
                var previousState = controller.GetState();
                while (controller.IsConnected) {
                    if (IsKeyPressed(ConsoleKey.Escape)) {
                        break;
                    }
                    var state = controller.GetState();
                    if (previousState.PacketNumber != state.PacketNumber)
                        Log(state.Gamepad.ToString());
                    Thread.Sleep(8);
                    previousState = state;
                }
            }
            Log("End XGamepadApp");
        }

        /// <summary>
        /// Determines whether the specified key is pressed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is pressed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKeyPressed(ConsoleKey key)
        {
            return Console.KeyAvailable && Console.ReadKey(true).Key == key;
        }


        /// <summary>
        /// Load configuration files.
        /// </summary>
        /// <param name="appfilename"></param>
        /// <param name="hardfilename"></param>
        public void LoadConfigurationFiles(string appfilename, string hardfilename)
        {
            // Load application and hardware config
            Config.Application = Files.Deserialize<ApplicationDB>(appfilename);
            Config.Hardware = Files.Deserialize<HardwareDB>(hardfilename);
            // Restore internal values
            Logger.LogLevel = Config.Application.LogLevel;
        }

        public void SortControlSets()
        {
            var sorted = Config.AllControlSets.ControlSets.OrderBy(x => x.UniqueName).ToList();
            Config.AllControlSets.ControlSets = sorted;
        }

        /// <summary>
        /// Load control set files from Application Configuration directory.
        /// Optionnaly, a consolidated control set save file can be loaded
        /// instead of scanning the directory.
        /// </summary>
        /// <param name="csfilename"></param>
        /// <param name="loadcsfile">[in] load consolidated control set file</param>
        public void LoadControlSetFiles(bool loadcsfile = false, string csfilename = "")
        {
            if (loadcsfile && csfilename!="") {
                // Use consolidated save
                Config.AllControlSets = Files.Deserialize<ControlSetsDB>(csfilename);
                SortControlSets();
            } else {
                // Load each controlset from content of the ControlSet directory
                string path = Config.Application.ControlSetsDirectory;

                Config.AllControlSets = new ControlSetsDB();
                Config.AllControlSets.ControlSets = new List<ControlSetDB>();

                CheckAndCreateControlSetDir();
                // Scan all xml files and load each control set
                var files = Directory.EnumerateFiles(Config.Application.ControlSetsDirectory, "*.xml");
                foreach (var file in files) {
                    var newcs = Files.Deserialize<ControlSetDB>(file);
                    Config.AllControlSets.ControlSets.Add(newcs);
                }
            }

            // Find default control set
            if (Config.AllControlSets.ControlSets.Count>0) {
                var cs = Config.AllControlSets.ControlSets.Find(x => (x.UniqueName == Config.Application.DefaultControlSetName));
                if (cs==null)
                    Config.CurrentControlSet = Config.AllControlSets.ControlSets[0];
                else
                    Config.CurrentControlSet = cs;
            } else {
                var cs = new ControlSetDB();
                cs.UniqueName = "Default";
                Config.AllControlSets.ControlSets.Add(cs);
                Config.CurrentControlSet = Config.AllControlSets.ControlSets[0];
            }

            // Save its name
            Config.Application.DefaultControlSetName = Config.CurrentControlSet.UniqueName;

            // Fix any misconfigured control set
            foreach (var cs in Config.AllControlSets.ControlSets) {
                // Ensure all axes are defined, else add missing and reset counters
                /*
                for (int i = cs.vJoyMapping.RawAxisTovJoyDB.Count; i<vJoyIOFeederAPI.vJoyFeeder.MAX_AXES_VJOY; i++) {
                    var db = new RawAxisDB();
                    db.MappedIndexUsedvJoyAxis = i;
                    cs.vJoyMapping.RawAxisTovJoyDB.Add(db);
                }*/
                // Enforce correct axis mapping in the order of the XML file
                for (int i = 0; i<cs.vJoyMapping.RawAxisTovJoyDB.Count; i++) {
                    var db = cs.vJoyMapping.RawAxisTovJoyDB[i];
                    db.MappedIndexUsedvJoyAxis = i;
                }

                /*
                // Ensure all inputs are defined, else add missing
                for (int i = cs.vJoyMapping.RawInputTovJoyMap.Count; i<vJoyIOFeederAPI.vJoyFeeder.MAX_BUTTONS_VJOY; i++) {
                    var db = new RawInputDB();
                    db.MappedvJoyBtns = new List<int>(1) { i };
                    cs.vJoyMapping.RawInputTovJoyMap.Add(db);
                }

                // Ensure all outputs are defined, else add missing
                for (int i = cs.RawOutputBitMap.Count; i<16; i++) {
                    var db = new RawOutputDB();
                    db.MappedRawOutputBit = new List<int>(1) { i + 8 };
                    cs.RawOutputBitMap.Add(db);
                }
                */
            }
        }

        public void SaveConfigurationFiles(string appfilename, string hardfilename)
        {
            // Copy internal values
            Config.Application.DefaultControlSetName = Config.CurrentControlSet.UniqueName;
            Config.Application.LogLevel = Logger.LogLevel;
            // save all
            Files.Serialize<ApplicationDB>(appfilename, Config.Application);
            Files.Serialize<HardwareDB>(hardfilename, Config.Hardware);
        }

        public void SaveControlSetFiles(bool savecsfile = false, string csfilename = "")
        {
            // Update from current Axis/mode
            /*Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Clear();
            if (vJoy!=null) {
                for (int i = 0; i < vJoy.AllAxesInfo.Count; i++) {
                    var db = new RawAxisDB();
                    db.MappedvJoyAxis = vJoy.AllAxesInfo[i].Name;
                    db.ControlPoints = vJoy.AllAxesInfo[i].AxisCorrection.ControlPoints;
                    Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Add(db);
                }
            }
            */

            // Load from dir or from consolidated?
            if (savecsfile && csfilename!="") {
                // Make a consolidated save, just in case (could be used in the future
                // to allow "undo" operations.
                Files.Serialize<ControlSetsDB>(csfilename, Config.AllControlSets);
            } else {
                CheckAndCreateControlSetDir();

                // Remove all xml files and save each control set, using its unique name.
                var files = Directory.EnumerateFiles(Config.Application.ControlSetsDirectory, "*.xml");
                foreach (var file in files) {
                    try {
                        File.Delete(file);
                    } catch (Exception ex) {
                        Log("Cannot delete " + file + ", " + ex.Message, LogLevels.IMPORTANT);
                    }
                }
                // Save all control set as XML files
                foreach (var cs in Config.AllControlSets.ControlSets) {
                    var filename = Path.Combine(Config.Application.ControlSetsDirectory, cs.UniqueName + ".xml");
                    Files.Serialize<ControlSetDB>(filename, cs);
                }
            }
        }

        public void CheckAndCreateControlSetDir()
        {
            // Check CS directory exists
            if (!Directory.Exists(Config.Application.ControlSetsDirectory)) {
                try {
                    // Create it
                    Directory.CreateDirectory(Config.Application.ControlSetsDirectory);
                } catch (Exception ex) {
                    Log("Cannot create " + Config.Application.ControlSetsDirectory + ", " + ex.Message, LogLevels.IMPORTANT);
                }
            }
            // Add a warning text file
            var filename = Path.Combine(Config.Application.ControlSetsDirectory, "_Directory managed by BackForceFeeder.txt");
            try {
                var warnfile = File.CreateText(filename);
                warnfile.WriteLine("Last accessed on: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                warnfile.WriteLine("Created by BackForceFeeder v" + typeof(vJoyManager).Assembly.GetName().Version.ToString() +".");
                warnfile.WriteLine("Files will be removed or added automatically by BackForceFeeder. Do not change directory content.");
                warnfile.Close();
            } catch (Exception ex) {
                Log("Cannot create " + filename + ", " + ex.Message, LogLevels.IMPORTANT);
            }
        }

    }
}
