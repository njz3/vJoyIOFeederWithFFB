using BackForceFeeder.Configuration;
using BackForceFeeder.FFBManagers;
using BackForceFeeder.Inputs;
using BackForceFeeder.IOCommAgents;
using BackForceFeeder.Outputs;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;
using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BackForceFeeder.BackForceFeeder
{
    /// <summary>
    /// This is the main thread of the BackForceFeeder.
    /// It handles reading from the IOboard(s) and process data to expose them
    /// to either the Input or vJoy interfaces.
    /// The force feedback is managed by a separate periodic thread (realtime)
    /// to ensure correct effect processing. Then this thread will send the
    /// latest computed torque command to the IOboard connected to the motor 
    /// driver.
    /// </summary>
    public class BFFManager
    {
        /// <summary>
        /// Manager configuration - only when instance for whole application
        /// before we access it before creating a BFFManager instance.
        /// </summary>
        public static FeederConfig Config = new FeederConfig();
        /// <summary>
        /// Currently selected control set
        /// </summary>
        public static ControlSetDB CurrentControlSet { get; protected set; } = new ControlSetDB();

        public void ChangeCurrentControlSet(ControlSetDB newcs)
        {
            Logger.Log("Changing control set for " + newcs.UniqueName, LogLevels.IMPORTANT);
            CheckControlSet(newcs);
            // Clear all current states
            if (Outputs!=null)
                Outputs.ClearAll();
            if (Inputs!=null)
                Inputs.ClearAll();
            // Swap control set
            CurrentControlSet = newcs;
        }

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
        public FFBManager FFB = null;
        /// <summary>
        /// Inputs from IOBoard
        /// </summary>
        public InputsManager Inputs = null;
        /// <summary>
        /// Output from emulators
        /// </summary>
        public OutputsManager Outputs = null;
        /// <summary>
        /// Keystroke emulation
        /// </summary>
        public KeyStrokesManager KeyStrokes = null;

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
        /// 1 = every tick/period
        /// 2 = every 2 ticks/periods
        /// n = every n
        /// </summary>
        public const int OutputUpdate = 1; //5*2 = 10ms
        /// <summary>
        /// 1 = every tick/period
        /// 2 = every 2 ticks/periods
        /// n = every n
        /// </summary>
        public const int KeystrokeUpdate = 4; //5*4 = 20ms

        /// <summary>
        /// Raw analog inputs after being updated (up to 8)
        /// </summary>
        double[] RawAxisFromIOBoard_pct = new double[8];
        double[] CorrectedAxisFromIOBoard_pct = new double[8];
        double[] vJoyAxes_pct = new double[8];

        /// <summary>
        /// Raw outputs mapped from game (up to 32).
        /// Will be send to IOboard. Current bitmap:
        /// - 0-7: lamps
        /// - 8-15: driveboard
        /// </summary>
        public UInt64 RawOutputsToIOBoard = 0;


        protected bool Running = false;
        protected Thread ManagerThread = null;
        protected Thread ProcessScannerThread = null;
        public ulong TickCount { get; protected set; } = 0;

        public bool IsRunning { get { return Running; } }

        public BFFManager()
        {
        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        { Logger.Log("[MANAGER] " + text, level); }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        { Logger.LogFormat(level, "[MANAGER] " + text, args); }

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


        public bool InitIOBoard(USBSerialIO ioboard)
        {
            if (ioboard==null) {
                Log("No IO board connected, cannot initialize hardware", LogLevels.ERROR);
                return false;
            }
            Log("Initializing IO board", LogLevels.IMPORTANT);
            // Initialize board
            ioboard.PerformInit();

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
            // Enable safety watchdog
            ioboard.EnableWD();
            if (Config.Hardware.UseStreamingMode) {
                // Enable auto-streaming
                ioboard.StartStreaming();
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
                FFB = FFBManager.Factory(Config.Hardware.TranslatingModes, GlobalRefreshPeriod_ms);
            }

            // Use this to allow 1ms sleep granularity (else default is 16ms!!!)
            // This consumes more CPU cycles in the OS, but does improve
            // a lot reactivity when soft real-time work needs to be done.
            MultimediaTimer.SetTickGranularityOnWindows();

            // IOBoad
            InitIOBoard(IOboard);

            // vJoy
            if (vJoy!=null) {
                vJoy.EnablevJoy(); // Create joystick interface
                vJoy.Acquire(1); // Use first enumerated vJoy device
                vJoy.StartAndRegisterFFB(FFB); // Start FFB callback mechanism in vJoy
            }

            // In case we want to use XInput/DInput devices to gather multiple inputs and feed them to vJoy?
            //XInput();
            //DirectInput();

            KeyStrokes = new KeyStrokesManager();

            // Output system for drvbd/lamps (always created)
            Outputs = new OutputsManager();

            // Initialize inputs and outputs considering the number of IOs we have
            Inputs = new InputsManager();
            // Initialize inputs and outputs considering the number of IOs we have
            Inputs.Initialize(IOboard.AnalogInputs.Length, IOboard.DigitalInputs8.Length*8);
            Outputs.Initialize(16);
            // Start output manager
            Outputs.Start();


            // Start FFB manager
            if (FFB!=null)
                FFB.Start();


            // Now that eveyrthing is created, perform a sanity check on control
            // set before we start the manager's loop
            CheckControlSet(CurrentControlSet);

            uint missingFrameCounter = 0;
            uint error_counter = 0;
            UInt64 nextRun_ms = (UInt64)(MultimediaTimer.RefTimer.ElapsedMilliseconds);

            if (Config.Application.VerbosevJoyManager) {
                Log("Start manager loop...");
            }

            while (Running) {
                TickCount++;
                // Code which purpose is to synchronize the while loop with the serial communication
                // It is a bit difficult on Windows to garanty a wait time down to a ms.
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



                // Perform communication with IOBoard
                if (IOboard != null) {
                    try {
                        if (IOboard.IsOpen) {

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
                            // Monitor missing packets to detect a dead IOboard
                            if (nbproc>0) {
                                missingFrameCounter = 0;
                            } else {
                                missingFrameCounter++;
                                if (missingFrameCounter>99 && (missingFrameCounter%100==0)) {
                                    Log("After " + missingFrameCounter + " ticks, IOBoard still not responding, is it dead or reseted? Trying to activate it", LogLevels.IMPORTANT);
                                    // Enable safety watchdog
                                    IOboard.EnableWD();
                                    // Enable auto-streaming
                                    if (Config.Hardware.UseStreamingMode) {
                                        IOboard.StartStreaming();
                                    }
                                    if (missingFrameCounter>499) {
                                        Log("IOBoard still not responding, perform reset of serial communication", LogLevels.IMPORTANT);
                                        throw new Exception("Reseting serial communication");
                                    }
                                }
                            }
                            #endregion

                            #region Process IOboard inputs and update vJoy values or send keystroke
                            ProcessIOBoardInputs(IOboard.AnalogInputs, IOboard.DigitalInputs8);
                            #endregion

                            #region Process game outputs (lamps/driveboard/other)
                            // get outputs from game for lamps/driveboard/other
                            if (Outputs!=null) {
                                ProcessGameOutputs();

                                // /!\ Outputs block are in reverse order, from most important to less
                                //-  Block[0]: reserved for control of actuators, like fwd/rev direction, ...
                                // - Block[1]: used for lamps (on mega2560)
                                //-  Block[2]: used for driveboard communication (mega2560)
                                // Raw lamps output will actually be configured to map either of these
                                // two last blocks

                                // Save to outputs skipping the first outputblock (managed for direction)
                                for (int i = 0; i<IOboard.DigitalOutputs8.Length-1; i++) {
                                    var shift = (i<<3);
                                    IOboard.DigitalOutputs8[i+1] = (byte)((this.Outputs.RawOutputsStates>>shift)&0xFF);
                                }
                            }
                            #endregion

                            #region Torque output (if FFB enabled). This will overwrite DigitalOutputs[0/2]
                            ProcessFFBOutput();
                            #endregion

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
                        Log("StackTrace: " + ex.StackTrace, LogLevels.DEBUG);
                        // Ensure current control set is not missing elements
                        CheckControlSet(CurrentControlSet);
                        // Then verify communication
                        try {
                            if (IOboard.IsOpen)
                                IOboard.CloseComm();
                        } catch (Exception ex2) {
                            Log("Unable to close communication " + ex2.Message, LogLevels.ERROR);
                        }
                        error_counter++;
                        System.Threading.Thread.Sleep(500);
                        if (error_counter > 10) {
                            // Serious problem here, try complete restart with scanning
                            if (Outputs!=null)
                                Outputs.Stop();
                            if (FFB!=null)
                                FFB.Stop();
                            if (IOboard != null)
                                IOboard.CloseComm();
                            if (vJoy!=null)
                                vJoy.Release();
                            goto __restart;
                        }
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

        #region Process IOboard inputs
        protected double _PrevWheelAngle = 0.0;

        protected void ProcessIOBoardInputs(UInt16[] analogAxes, byte[] digitalInputs)
        {
            #region Wheel angle and pedals
            if (analogAxes.Length > 0) {
                // Refresh hardware wheel angle (between -1...1)

                // Analog potentiometer version:
                // Scale analog input in cts between 0..0xFFF, then map it to -1/+1, 0 being center
                var wheel_cts = analogAxes[0]; // Potentiometer
                var angle_u = (wheel_cts * Config.Hardware.WheelScaleFactor_u_per_cts) - Config.Hardware.WheelCenterOffset_u;

                // For encoder, needs a MIN encoder position for full left, and MAX for right

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
                _PrevWheelAngle = angle_u;


                // For debugging purpose, add a 4th axis to display torque output
                long[] rawAxisValues12bits = new long[5];
                analogAxes.CopyTo(rawAxisValues12bits, 0);
                rawAxisValues12bits[4] = (long)(FFB.OutputTorqueLevel * 0x7FF + 0x800);

                // - 360deg POV to view for wheel angle?
                //vJoy.UpodateContinuousPOV((uint)((IOboard.AnalogInputs[0] / (double)0xFFF) * 35900.0) + 18000);

                // Update analog raw axis
                Inputs.UpdateAllRawAxes(rawAxisValues12bits);

                // Set axis values into vJoy report:
                if (vJoy!=null) {
                    // Process raw values and correct them to get pct
                    Inputs.GetCorrectedAxes(ref CorrectedAxisFromIOBoard_pct);
                    // Update vJoy axes
                    vJoy.UpdateAllAxes(CorrectedAxisFromIOBoard_pct);
                }
            }
            #endregion

            #region Buttons, inputs, shifter decoders

            if (digitalInputs.Length > 0) {
                // New raw input state
                UInt64 rawinput_states = 0;

                // For each single input, process mapping, autofire and toggle
                for (int idxDin = 0; idxDin<Math.Min(4, digitalInputs.Length); idxDin++) {
                    rawinput_states |= (UInt64)digitalInputs[idxDin]<<(8*idxDin);
                }
                Inputs.UpdateAllDigitalInputs(rawinput_states);
            }

            // Update vJoy and send to driver every n ticks to limit workload on driver
            if ((TickCount % vJoyUpdate) == 0) {
                if (vJoy!=null)
                    vJoy.PublishReport();
            }

            #endregion

            #region Now that everything was updated, process keystrokes
            if ((TickCount % KeystrokeUpdate)==0) {
                Inputs.GetCorrectedAxes(ref CorrectedAxisFromIOBoard_pct);
                KeyStrokes.ProcessKeyStrokes(RawAxisFromIOBoard_pct, CorrectedAxisFromIOBoard_pct, Inputs.RawInputsStates, Inputs.ButtonsValues);
            }
            #endregion
        }
        #endregion

        #region Process Outputs
        /// <summary>
        /// Process FFB outputs and fill IOboard for:
        /// - analog ouput [0]: torque as PWM
        /// - digital outputs [0]: wheel control direction/enable/rev/fwd
        /// - digital outputs [2]: driveboard bytecode
        /// </summary>
        protected void ProcessFFBOutput()
        {
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
                            var outlevel = Outputs.GameDriveBoard;
                            // Save driveboard command code
                            if (IOboard.DigitalOutputs8.Length > 2) {
                                IOboard.DigitalOutputs8[2] = (byte)(outlevel & 0xFF);
                            }
                        }
                        break;
                }
            }

        }
        /// <summary>
        /// Read lamps and driveboard outputs, map them and store the result
        /// in RawOutputsStates
        /// </summary>
        protected void ProcessGameOutputs()
        {
            if (this.TickCount % OutputUpdate==0) {
                Outputs.UpdateOutput();
                this.RawOutputsToIOBoard = Outputs.RawOutputsStates;
            }
            // Split per 8bit (byte) word
            /*
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

        protected UInt32 MapGameLampsToRawOutputs(UInt32 gameLampOutputs, List<RawOutputDB> rawoutputbitmap)
        {
            UInt32 rawOutputsStates = 0;
            // Decode lamps: use mapping to set raw bits accordingly
            for (int idxbit = 0; idxbit<rawoutputbitmap.Count; idxbit++) {
                // Single bit value of the lamp : on/off state
                var rawLampBitValue = (gameLampOutputs & (1<<idxbit))!=0;
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
                        rawOutputsStates |= bitmask;
                    } else {
                        rawOutputsStates &= (uint)~bitmask;
                    }

                }
            }
            return rawOutputsStates;
        }
        protected UInt32 MapGameDriveboardToRawOutputs(UInt32 lampOutputs, List<RawOutputDB> rawoutputbitmap)
        {
            UInt32 rawOutputsStates;
            // No mapping for driveboard data, just copy it straight from game
            rawOutputsStates = lampOutputs;
            return rawOutputsStates;
        }
        #endregion

        #region Process scanner
        Process LastKnownProcess = null;

        protected void ProcessScannerThreadMethod()
        {
            List<Tuple<string, string>> namesAndTitle = new List<Tuple<string, string>>();

            uint tick_cnt = 0;
            while (Running) {
                // Slow period to wait for other app
                Thread.Sleep(500);
                tick_cnt++;

                if (!BFFManager.Config.Application.AutodetectControlSetAtRuntime) {
                    LastKnownProcess = null;
                    continue;
                }
                try {
                    // First ensure current process still exists!
                    if (LastKnownProcess!=null) {
                        // If exited, then we will scan again
                        if (LastKnownProcess.HasExited) {
                            LastKnownProcess = null;
                        } else {
                            //continue;
                        }
                    }

                    // Run check every 3s (6 ticks@500ms)
                    if (tick_cnt%6==0) {
                        namesAndTitle.Clear();
                        int currentidx = -1;
                        // Loop on control sets and build list of known process/title
                        for (int i = 0; i<BFFManager.Config.AllControlSets.ControlSets.Count; i++) {
                            var cs = BFFManager.Config.AllControlSets.ControlSets[i];
                            namesAndTitle.Add(new Tuple<string, string>(cs.ProcessDescriptor.ProcessName, cs.ProcessDescriptor.MainWindowTitle));
                            if (cs == CurrentControlSet)
                                currentidx = i;
                        }

                        // Scan processes and main windows title
                        var found = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(namesAndTitle, true, false);
                        // Store detected profiles
                        if (found.Count>0) {
                            // Display all matches and save highest priority
                            int newidx = -1;
                            ControlSetDB newcs = null;
                            Process newproc = null;
                            for (int i = 0; i<found.Count; i++) {
                                int idx = found[i].Item2;
                                var cs = BFFManager.Config.AllControlSets.ControlSets[idx];
                                if (newcs==null || cs.PriorityLevel>=newcs.PriorityLevel) {
                                    newcs = cs;
                                    newproc = found[0].Item1;
                                    newidx = idx;
                                }
                                if (BFFManager.Config.Application.VerboseScanner) {
                                    Log("Scanner found " + found[i].Item1.ProcessName + " main window " + found[i].Item1.MainWindowTitle + " matched control set " + cs.UniqueName, LogLevels.DEBUG);
                                }
                            }

                            // Pick last control set with highest priority
                            if (newcs==null) {
                                Log("Scanner failed to find highest control set", LogLevels.DEBUG);
                                continue;
                            }

                            if ((currentidx!=newidx) ||
                                (LastKnownProcess==null) ||
                                (newproc.Id != LastKnownProcess.Id) ||
                                (newproc.MainWindowTitle != LastKnownProcess.MainWindowTitle)) {

                                LastKnownProcess = newproc;
                                var cs = BFFManager.Config.AllControlSets.ControlSets[newidx];
                                ChangeCurrentControlSet(cs);
                                Log("Detected " + LastKnownProcess.ProcessName + " (" + LastKnownProcess.MainWindowTitle + "), auto-switching to control set " + cs.UniqueName, LogLevels.IMPORTANT);
                            }
                        }
                    }
                } catch (Exception ex) {
                    Log("Exception while trying to scan process: " + ex.Message, LogLevels.IMPORTANT);
                    LastKnownProcess = null;
                }
            }
        }
        #endregion

        #region NOT DONE - DirectInput/XInput inputs to merge into vJoy

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
                    if (OSUtilities.IsKeyPressed(ConsoleKey.Escape)) {
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

        #endregion

        #region Configuration
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

        public void CheckControlSet(ControlSetDB cs)
        {
            if (cs==null ||
                this.vJoy==null ||
                this.IOboard==null ||
                this.Inputs == null ||
                this.Outputs ==null)
                return;
            bool modified = false;
            // Check number of axis match current vJoy configuration or hardware - else 
            // increase number of config
            int nbaxes = Math.Max(this.vJoy.NbAxes, this.IOboard.AnalogInputs.Length);
            if (cs.RawAxisDBs.Count<nbaxes) {
                for (int i = cs.RawAxisDBs.Count; i<nbaxes; i++) {
                    RawAxisDB newDB = new RawAxisDB();
                    newDB.vJoyAxisIndex = i;
                    cs.RawAxisDBs.Add(newDB);
                    modified = true;
                }
            }
            // After size is corrected, check each rawaxisdb and correct index 
            // and control point
            for (int i = 0; i<cs.RawAxisDBs.Count; i++) {
                var rawdb = cs.RawAxisDBs[i];
                rawdb.vJoyAxisIndex = i;
                if (rawdb.ControlPoints.Count<2) {
                    rawdb.ResetCorrectionFactors();
                    modified = true;
                }
            }

            // Ensure enough rawinputs are defined according to hardware, else 
            // add missing
            for (int i = cs.RawInputDBs.Count; i<this.Inputs.RawInputs.Count; i++) {
                var db = new RawInputDB();
                db.MappedvJoyBtns = new List<int>(1) { i };
                cs.RawInputDBs.Add(db);
                modified = true;
            }

            // Ensure all outputs are defined, else add missing
            for (int i = cs.RawOutputDBs.Count; i<this.Outputs.RawOutputs.Count; i++) {
                var db = new RawOutputDB();
                // Must skip first 8 outputs because they are reserved for
                // motor control (rev/fwd)
                db.MappedRawOutputBit = new List<int>(1) { i };
                cs.RawOutputDBs.Add(db);
                modified = true;
            }

            if (modified) {
                Log("Sanity check for control set " + cs.UniqueName + ": fixed config issues!", LogLevels.IMPORTANT);
            } else {
                Log("Sanity check for control set " + cs.UniqueName + ": ok", LogLevels.DEBUG);
            }
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
                    CurrentControlSet = Config.AllControlSets.ControlSets[0];
                else
                    CurrentControlSet = cs;
            } else {
                var cs = new ControlSetDB();
                cs.UniqueName = "Default";
                Config.AllControlSets.ControlSets.Add(cs);
                CurrentControlSet = Config.AllControlSets.ControlSets[0];
            }

            // Save its name
            Config.Application.DefaultControlSetName = CurrentControlSet.UniqueName;

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
                for (int i = 0; i<cs.RawAxisDBs.Count; i++) {
                    var db = cs.RawAxisDBs[i];
                    db.vJoyAxisIndex = i;
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
            Config.Application.DefaultControlSetName = CurrentControlSet.UniqueName;
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
                warnfile.WriteLine("Created by BackForceFeeder v" + typeof(BFFManager).Assembly.GetName().Version.ToString() +".");
                warnfile.WriteLine("Files will be removed or added automatically by BackForceFeeder. Do not change directory content while BackForceFeeder is running.");
                warnfile.Close();
            } catch (Exception ex) {
                Log("Cannot create " + filename + ", " + ex.Message, LogLevels.IMPORTANT);
            }
        }
        #endregion

    }
}
