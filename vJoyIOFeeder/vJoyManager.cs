using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using vJoyIOFeeder.Configuration;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.IOCommAgents;
using vJoyIOFeeder.Outputs;
using vJoyIOFeeder.Utils;
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace vJoyIOFeeder
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

        /// <summary>
        /// Model 3 generic drive board (unknown EEPROM)
        /// Use parallel port communication (8bits TX, 8bits RX)
        /// All Effects emulated using constant torque effect
        /// with codes 0x50 and 0x60.
        /// </summary>
        MODEL3_UNKNOWN_DRVBD = 100,
        /// <summary>
        /// Le Mans Model 3 drive board
        /// </summary>
        MODEL3_LEMANS_DRVBD,
        /// <summary>
        /// Scud Race Model 3 drive board
        /// </summary>
        MODEL3_SCUD_DRVBD,

        /// <summary>
        /// Lindbergh RS422 drive board through RS232
        /// </summary>
        LINDBERGH_GENERIC_DRVBD = 300,

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
        public AOutput Outputs = null;
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


        protected bool Running = true;
        protected Thread ManagerThread = null;
        protected ulong TickCount = 0;

        public bool IsRunning { get { return Running; } }

        public vJoyManager()
        {
            vJoy = new vJoyFeeder();
        }


        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[MANAGER] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[MANAGER] " + text, args);
        }


        public bool InitIOBoard(USBSerialIO ioboard)
        {
            if (ioboard==null) {
                Log("No IO board conneted", LogLevels.IMPORTANT);
                return false;
            }
            Log("Initializing IO board", LogLevels.IMPORTANT);
            // Initialize board
            ioboard.PerformInit();
            // Enable safety watchdog
            ioboard.EnableWD();
            // Enable auto-streaming
            ioboard.StartStreaming();
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
                ioboard.SetParameter("pwmmode", pwmmode);

                ioboard.SetParameter("wheelmode", 2); // Filtered value
                ioboard.SetParameter("pedalmode", 0); // No option
            }
            return true;
        }


        protected void ManagerThreadMethod()
        {
        __restart:

            Log("Program configured for " + Config.Hardware.TranslatingModes, LogLevels.IMPORTANT);

            var boards = USBSerialIO.ScanAllCOMPortsForIOBoards();
            if (boards.Length > 0) {
                IOboard = boards[0];
                Log("Found io board on " + IOboard.COMPortName + " version=" + IOboard.BoardVersion + " type=" + IOboard.BoardDescription, LogLevels.IMPORTANT);
            } else {
                IOboard = null;
                if (Config.Hardware.RunWithoutIOBoard) {
                    Log("No boards found! Continue without real hardware", LogLevels.ERROR);
                } else {
                    Log("No boards found! Thread will terminate", LogLevels.ERROR);
                    Running = false;
                    //Console.ReadKey(true);
                    return;
                }
            }

            // Output system : lamps
            Outputs = new MAMEOutputWinAgent();
            Outputs.Start();

            switch (Config.Hardware.TranslatingModes) {
                case FFBTranslatingModes.PWM_CENTERED:
                case FFBTranslatingModes.PWM_DIR: {
                        FFB = new FFBManagerTorque(GlobalRefreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.MODEL3_UNKNOWN_DRVBD: {
                        // Default to Scud/Daytona2
                        FFB = new FFBManagerModel3Scud(GlobalRefreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.MODEL3_LEMANS_DRVBD: {
                        FFB = new FFBManagerModel3Lemans(GlobalRefreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.MODEL3_SCUD_DRVBD: {
                        FFB = new FFBManagerModel3Scud(GlobalRefreshPeriod_ms);
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported FFB mode " + Config.Hardware.TranslatingModes.ToString());
            }


            // Use this to allow 1ms sleep granularity (else default is 16ms!!!)
            // This consumes more CPU cycles in the OS, but does improve
            // a lot reactivity when soft real-time work needs to be done.
            MultimediaTimer.SetTickGranularityOnWindows();

            vJoy.EnablevJoy(); // Create joystick interface
            vJoy.Acquire(1); // Use first enumerated vJoy device
            vJoy.StartAndRegisterFFB(FFB); // Start FFB callback mechanism in vJoy

            // In case we want to use XInput/DInput devices to gather multiple inputs?
            //XInput();
            //DirectInput();

            InitIOBoard(IOboard);

            if (Config.Application.VerbosevJoyManager) {
                Log("Start feeding...");
            }

            // Start FFB manager
            FFB.Start();

            // Internal values for special operation
            double prev_angle = 0.0;

            UInt32 autofire_mode_on = 0;
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
                }
                if (IOboard != null) {
                    try {
                        if (IOboard.IsOpen) {
                            // Empty serial buffer
                            if (delay_ms<0) {
                                IOboard.UpdateOnStreaming((-delay_ms)/GlobalRefreshPeriod_ms);
                            }
                            // Shift tick to synch with IOboard
                            var before = MultimediaTimer.RefTimer.ElapsedMilliseconds;
                            // Update status on received packets
                            var nbproc = IOboard.UpdateOnStreaming();
                            var after = MultimediaTimer.RefTimer.ElapsedMilliseconds;
                            // Delay is expected to be 1-2ms for processing in stream
                            delay_ms =  (int)(after-before);
                            // Accept up to 2ms of delay (jitter), else consider we have
                            if (delay_ms>2 && nbproc==1) {
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
                            uint[] axesXYRZplusSL0ForTrq = new uint[5];
                            IOboard.AnalogInputs.CopyTo(axesXYRZplusSL0ForTrq, 0);
                            axesXYRZplusSL0ForTrq[4] = (uint)(FFB.OutputTorqueLevel * 0x7FF + 0x800);

                            // Set values into vJoy report:
                            // - axes
                            vJoy.UpdateAxes12(axesXYRZplusSL0ForTrq);

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
                                for (int i = 0; i<Math.Min(4, IOboard.DigitalInputs8.Length); i++) {

                                    // Scan 8bit input block, increase each time the raw index
                                    for (int j = 0; j<8; j++, rawidx++) {
                                        // Get configuration of this raw input
                                        var rawdb = Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx];

                                        // Default input value is current logic (false if not inverted)
                                        bool newrawval = rawdb.IsInvertedLogic;

                                        // Check if input is "on" and invert default value
                                        if ((IOboard.DigitalInputs8[i] & (1<<j))!=0) {
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

                                        // Check if we toggle the bit (or autofire mode)
                                        if (rawdb.IsToggle) {
                                            // Toggle only if we detect a false->true transition in raw value
                                            if (newrawval && (!prev_state)) {
                                                // Toggle = xor on every vJoy buttons
                                                vJoy.ToggleButtons(rawdb.vJoyBtns);
                                            }
                                        } else if (rawdb.IsAutoFire) {
                                            // Autofire set, if false->true transition, then toggle autofire state
                                            if (newrawval && (!prev_state)) {
                                                // Enable/disable autofire
                                                autofire_mode_on ^= rawbit;
                                            }
                                            // No perform autofire toggle if autofire enabled
                                            if ((autofire_mode_on&rawbit)!=0) {
                                                // Toggle = xor every 20 periods
                                                if ((TickCount%20)==0) {
                                                    vJoy.ToggleButtons(rawdb.vJoyBtns);
                                                }
                                            }
                                        } else if (rawdb.IsSequencedvJoy) {
                                            // Sequenced vJoy buttons - everyrising edge, will trigger a new vJoy
                                            // if false->true transition, then toggle vJoy and move index
                                            if (newrawval && (!prev_state)) {
                                                // Clear all buttons first
                                                vJoy.ClearButtons(rawdb.vJoyBtns);
                                                if (rawdb.SequenceCurrentToSet>rawdb.vJoyBtns.Count) {
                                                    rawdb.SequenceCurrentToSet = 0;
                                                }
                                                if (rawdb.vJoyBtns.Count<1)
                                                    continue;
                                                // Set only indexed one
                                                vJoy.Set1Button(rawdb.vJoyBtns[rawdb.SequenceCurrentToSet]);
                                                // Move indexer
                                                rawdb.SequenceCurrentToSet++;
                                            }
                                        } else if (rawdb.ShifterDecoder!= ShifterDecoderMap.No) {
                                            // Part of HShifter decoder map, just save the values
                                            switch (rawdb.ShifterDecoder) {
                                                case ShifterDecoderMap.HSHifterLeftRight:
                                                case ShifterDecoderMap.HSHifterUp:
                                                case ShifterDecoderMap.HSHifterDown:
                                                    // rawdb
                                                    HShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HSHifterLeftRight] = rawdb;
                                                    // state of raw input
                                                    HShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HSHifterLeftRight] = newrawval;
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
                                            // Nothing specific : perform simple mask
                                            if (newrawval) {
                                                vJoy.SetButtons(Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx].vJoyBtns);
                                            } else {
                                                vJoy.ClearButtons(Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap[rawidx].vJoyBtns);
                                            }
                                        }

                                    }

                                }

                                // Decode HShifter map
                                if (HShifterDecoderMap[0]!=null && HShifterDecoderMap[1]!=null && HShifterDecoderMap[2]!=null) {
                                    int selectedshift = 0; //0=neutral
                                    // First switch pressed?
                                    if (HShifterPressedMap[0]) {
                                        // Left up or down?
                                        if (HShifterPressedMap[1]) {
                                            // Left Up = 1
                                            selectedshift = 1;
                                        } else if (HShifterPressedMap[2]) {
                                            // Left Down = 2
                                            selectedshift = 2;
                                        } else {
                                            // Neutral
                                        }
                                    } else {
                                        // Right up or down?
                                        if (HShifterPressedMap[1]) {
                                            // Right Up = 3
                                            selectedshift = 3;
                                        } else if (HShifterPressedMap[2]) {
                                            // Right Down = 4
                                            selectedshift = 4;
                                        } else {
                                            // Neutral
                                        }
                                    }
                                    // Detect change
                                    if (selectedshift!=HShifterCurrent) {
                                        Log("HShifter decoder from=" + HShifterCurrent + " to " + selectedshift, LogLevels.INFORMATIVE);
                                        HShifterCurrent = selectedshift;
                                    }
                                }

                                // Decode Up/Down shifter map
                                if (UpDownShifterDecoderMap[0]!=null && UpDownShifterDecoderMap[1]!=null) {
                                    int selectedshift = 0; //0=neutral
                                                           // Detect change

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

                            // Outputs (Lamps)
                            if (Outputs!=null) {
                                // First 2 bits are unused for lamps (used by PWM Fwd/Rev)
                                IOboard.DigitalOutputs8[0] = (byte)(Outputs.LampsValue);
                            }

                            // Now output torque to Pwm+Dir or drive board command
                            switch (Config.Hardware.TranslatingModes) {
                                // PWM centered mode (50% = 0 torque)
                                case FFBTranslatingModes.PWM_CENTERED: {
                                        // Latch a copy
                                        var outlevel = FFB.OutputTorqueLevel;
                                        // Enforce range again to be [-1; 1]
                                        outlevel = Math.Min(1.0, Math.Max(outlevel, -1.0));
                                        UInt16 analogOut = (UInt16)(outlevel * 0x7FF + 0x800);
                                        IOboard.AnalogOutputs[0] = analogOut;
                                    }
                                    break;
                                // PWM+dir mode (0% = 0 torque, direction given by first output)
                                case FFBTranslatingModes.PWM_DIR: {
                                        // Latch a copy
                                        var outlevel = FFB.OutputTorqueLevel;
                                        if (outlevel >= 0.0) {
                                            UInt16 analogOut = (UInt16)(outlevel * 0xFFF);
                                            // Save into IOboard
                                            IOboard.AnalogOutputs[0] = analogOut;
                                            IOboard.DigitalOutputs8[0] |= 1<<0; // set FwdCmd bit 0
                                            IOboard.DigitalOutputs8[0] &= 0xFD; // clear RevCmd bit 1
                                        } else {
                                            UInt16 analogOut = (UInt16)(-outlevel * 0xFFF);
                                            // Save into IOboard
                                            IOboard.AnalogOutputs[0] = analogOut;
                                            IOboard.DigitalOutputs8[0] |= 1<<1; // set RevCmd bit 1
                                            IOboard.DigitalOutputs8[0] &= 0xFE; // clear FwdCmd bit 0
                                        }
                                    }
                                    break;
                                // Driveboard translation mode
                                case FFBTranslatingModes.MODEL3_UNKNOWN_DRVBD:
                                case FFBTranslatingModes.MODEL3_LEMANS_DRVBD:
                                case FFBTranslatingModes.MODEL3_SCUD_DRVBD: {
                                        // Latch a copy
                                        var outlevel = FFB.OutputEffectCommand;
                                        if (IOboard.DigitalOutputs8.Length > 1) {
                                            IOboard.DigitalOutputs8[1] = (byte)(outlevel & 0xFF);
                                        }
                                    }
                                    break;
                            }

                            // Save output state
                            RawOutputsStates = 0;
                            for (int i = 0; i<IOboard.DigitalOutputs8.Length; i++) {
                                var shift = (i<<3);
                                RawOutputsStates = (UInt32)(IOboard.DigitalOutputs8[i]<<shift);
                            }

                            // Send all outputs - this will revive the watchdog!
                            IOboard.SendOutputs();

                        } else {
                            Log("Re-connecting to same IO board on port " + IOboard.COMPortName, LogLevels.IMPORTANT);
                            IOboard.OpenComm();
                            // Enable safety watchdog
                            IOboard.EnableWD();
                            // Enable auto-streaming
                            IOboard.StartStreaming();
                            error_counter = 0;
                        }
                    } catch (Exception ex) {
                        Log("IO board Failing with " + ex.Message, LogLevels.ERROR);
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
            FFB.Stop();
            if (IOboard != null)
                IOboard.CloseComm();
            vJoy.Release();
        }

        public void Start()
        {
            if (ManagerThread != null) {
                Stop();
            }

            ManagerThread = new Thread(ManagerThreadMethod);
            Running = true;
            ManagerThread.Name = "vJoy Manager";
            ManagerThread.Priority = ThreadPriority.Normal;
            ManagerThread.Start();
        }
        public void Stop()
        {
            Running = false;
            if (ManagerThread == null)
                return;
            Thread.Sleep(GlobalRefreshPeriod_ms * 10);
            ManagerThread.Join(1000);
            ManagerThread = null;
        }

        public void DirectInput()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty) {
                Log("No joystick/Gamepad found.", LogLevels.ERROR);
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);

            Log(String.Format("Found Joystick/Gamepad with GUID: {0}", joystickGuid));

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Log(String.Format("Effect available {0}", effectInfo.Name));

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true) {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                    Log(state.ToString());
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
            var sorted = Config.AllControlSets.ControlSets.OrderBy(x=>x.UniqueName).ToList();
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
            // Copy to axis/mode
            for (int i = 0; i < Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Count; i++) {
                // Find mapping vJoy axis
                var name = Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB[i].vJoyAxis;
                var axisinfo = vJoy.AxesInfo.Find(x => (x.Name==name));
                if (axisinfo!=null) {
                    axisinfo.AxisCorrection.ControlPoints = Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB[i].ControlPoints;
                }
            }
            // Ensure all inputs are defined, else add missing
            for (int i = Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap.Count; i<vJoyIOFeederAPI.vJoyFeeder.MAX_BUTTONS_VJOY; i++) {
                var db = new RawInputDB();
                db.vJoyBtns = new List<int>(1) { i };
                Config.CurrentControlSet.vJoyMapping.RawInputTovJoyMap.Add(db);
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
            Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Clear();
            for (int i = 0; i < vJoy.AxesInfo.Count; i++) {
                var db = new RawAxisDB();
                db.vJoyAxis = vJoy.AxesInfo[i].Name;
                db.ControlPoints = vJoy.AxesInfo[i].AxisCorrection.ControlPoints;
                Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Add(db);
            }

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
            var filename = Path.Combine(Config.Application.ControlSetsDirectory, "_Directory managed by vJoyIOFeeder.txt");
            try {
                var warnfile = File.CreateText(filename);
                warnfile.WriteLine("Last accessed on: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                warnfile.WriteLine("Files will be removed or added automatically by vJoyIOFeeder. Do not change directory content.");
                warnfile.Close();
            } catch (Exception ex) {
                Log("Cannot create " + filename + ", " + ex.Message, LogLevels.IMPORTANT);
            }
        }

    }
}
