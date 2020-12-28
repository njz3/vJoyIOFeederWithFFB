#define CONSOLE_DUMP
#define FFB

// From vJoy SDK example

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

// Don't forget to add this
using vJoyInterfaceWrap;
using BackForceFeeder.Configuration;
using BackForceFeeder.FFBManagers;
using BackForceFeeder.Utils;
using static BackForceFeeder.vJoyIOFeederAPI.vJoyFeeder;

namespace BackForceFeeder.vJoyIOFeederAPI
{

    /// <summary>
    /// vJoy feeder used to report buttons and analog axis back to the driver.
    /// Normally used as a singleton/static class
    /// </summary>
    public class vJoyFeeder
    {

        public const int MAX_BUTTONS_VJOY = 32;
        public const int MAX_AXES_VJOY = 8;


        // Declaring one joystick (Device id 1) and a position structure. 
        public vJoy.JoystickState Report;

        /// <summary>
        /// FFB packet receiver
        /// </summary>
        public vJoyFFBReceiver FFBReceiver;

        public uint joyID {
            get;
            protected set;
        }
        protected vJoy Joystick;

        public UInt32 vJoyVersionDll = 0;
        public UInt32 vJoyVersionDriver = 0;
        public bool vJoyVersionMatch = false;


        protected List<vJoyAxisInfos> AllAxesInfo = new List<vJoyAxisInfos>(MAX_AXES_VJOY);
        protected List<UsedvJoyAxis> UsedAxis = new List<UsedvJoyAxis>(MAX_AXES_VJOY);

        public int NbUsedAxis {
            get {
                return this.UsedAxis.Count;
            }
        }

        public UsedvJoyAxis SafeGetUsedAxis(int selectedvJoyAxis)
        {
            if (selectedvJoyAxis<0 || selectedvJoyAxis>=this.UsedAxis.Count)
                return null;
            return this.UsedAxis[selectedvJoyAxis];
        }

        public vJoyFeeder()
        {
            // Create one joystick object and a position structure.
            Joystick = new vJoy();
            Report = new vJoy.JoystickState();
            FFBReceiver = new vJoyFFBReceiver();

            // Prepare list of vJoy's axes configuration. This is done
            // only one time after retreiving vJoy's information

            // Fill all axes
            AllAxesInfo.Clear();
            foreach (HID_USAGES toBeTested in Enum.GetValues(typeof(HID_USAGES))) {
                // Skip POV
                if (toBeTested == HID_USAGES.HID_USAGE_POV)
                    continue;
                var name = toBeTested.ToString().Replace("HID_USAGE_", "");
                var axisinfo = new vJoyAxisInfos();
                axisinfo.HID_Usage = toBeTested;
                axisinfo.Name = name;
                AllAxesInfo.Add(axisinfo);
            }

            // Fill OnlyUsedInfo by copying reference from AllAxesInfo once done
            UsedAxis.Clear();
        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[FEEDER] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[FEEDER] " + text, args);
        }


        public int EnablevJoy()
        {
            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!Joystick.vJoyEnabled()) {
                Log("vJoy driver not enabled: Failed Getting vJoy attributes.", LogLevels.ERROR);
                return -2;
            } else {
                LogFormat(LogLevels.DEBUG, "Vendor: {0}\tProduct :{1}\tVersion Number:{2}", Joystick.GetvJoyManufacturerString(), Joystick.GetvJoyProductString(), Joystick.GetvJoySerialNumberString());
            }

            // Test if DLL matches the driver
            this.vJoyVersionMatch = Joystick.DriverMatch(ref this.vJoyVersionDll, ref this.vJoyVersionDriver);
            if (this.vJoyVersionMatch) {
                LogFormat(LogLevels.DEBUG, "Version of Driver Matches DLL Version ({0:X})", this.vJoyVersionDll);
                return 1;
            } else {
                LogFormat(LogLevels.ERROR, "Version of Driver ({0:X}) does NOT match DLL Version ({1:X})", this.vJoyVersionDriver, this.vJoyVersionDll);
                return -1;
            }
        }

        public int Acquire(uint id)
        {
            // Device ID can only be in the range 1-16
            joyID = id;
            Report.bDevice = (byte)joyID;
            if (joyID <= 0 || joyID > 16) {
                LogFormat(LogLevels.ERROR, "Illegal device ID {0}\nExit!", joyID);
                return -1;
            }

            // Get the state of the requested device
            VjdStat status = Joystick.GetVJDStatus(joyID);
            switch (status) {
                case VjdStat.VJD_STAT_OWN:
                    LogFormat(LogLevels.DEBUG, "vJoy Device {0} is already owned by this feeder", joyID);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    LogFormat(LogLevels.DEBUG, "vJoy Device {0} is free", joyID);

                    break;
                case VjdStat.VJD_STAT_BUSY:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} is already owned by another feeder\nCannot continue", joyID);
                    return -3;
                case VjdStat.VJD_STAT_MISS:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} is not installed or disabled\nCannot continue", joyID);
                    return -4;
                default:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} general error\nCannot continue", joyID);
                    return -1;
            };


            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = Joystick.GetVJDButtonNumber(joyID);
            int ContPovNumber = Joystick.GetVJDContPovNumber(joyID);
            int DiscPovNumber = Joystick.GetVJDDiscPovNumber(joyID);

            // Print results
            LogFormat(LogLevels.DEBUG, "vJoy Device {0} capabilities:", joyID);
            LogFormat(LogLevels.DEBUG, "Numner of buttons\t\t{0}", nButtons);
            LogFormat(LogLevels.DEBUG, "Numner of Continuous POVs\t{0}", ContPovNumber);
            LogFormat(LogLevels.DEBUG, "Numner of Descrete POVs\t\t{0}", DiscPovNumber);

            // Check which axes are supported. Follow enum HID_USAGES
            UsedAxis.Clear();
            int i = 0;
            foreach (HID_USAGES toBeTested in Enum.GetValues(typeof(HID_USAGES))) {
                // Skip POV
                if (toBeTested == HID_USAGES.HID_USAGE_POV)
                    continue;
                var present = Joystick.GetVJDAxisExist(joyID, toBeTested);
                LogFormat(LogLevels.DEBUG, "Axis " + AllAxesInfo[i].Name + " \t\t{0}", present ? "Yes" : "No");
                if (present) {
                    AllAxesInfo[i].IsPresent = present;
                    // Retrieve min/max from vJoy
                    if (!Joystick.GetVJDAxisMin(joyID, toBeTested, ref AllAxesInfo[i].MinValue)) {
                        Log("Failed getting min value!");
                    }
                    if (!Joystick.GetVJDAxisMax(joyID, toBeTested, ref AllAxesInfo[i].MaxValue)) {
                        Log("Failed getting min value!");
                    }
                    Log(" Min= " + AllAxesInfo[i].MinValue + " Max=" + AllAxesInfo[i].MaxValue);

                    // Add to indexed list
                    var mapping = new UsedvJoyAxis();
                    mapping.RawAxisIndex = UsedAxis.Count;
                    mapping.vJoyAxisInfo = AllAxesInfo[i];
                    UsedAxis.Add(mapping);
                }
                i++;
            }

            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!Joystick.AcquireVJD(joyID)))) {
                LogFormat(LogLevels.ERROR, "Failed to acquire vJoy device number {0}.", joyID);
                return -1;
            } else {
                LogFormat(LogLevels.DEBUG, "Acquired: vJoy device number {0}.", joyID);
            }
            return (int)status;
        }

        public void Release()
        {
            Joystick.RelinquishVJD(joyID);
        }

        public int StartAndRegisterFFB(FFBManager ffb)
        {
            // Start FFB
#if FFB
            if (Joystick.IsDeviceFfb(joyID)) {
#if false // obsolete
                bool Ffbstarted = Joystick.FfbStart(joyID);
                if (!Ffbstarted) {
                    Console.WriteLine("Failed to start FFB on vJoy device number {0}.", joyID);
                    Console.ReadKey();
                    return -3;
                } else
                    Console.WriteLine("Started FFB on vJoy device number {0} - OK", joyID);
#endif

                // Register Generic callback function
                // At this point you instruct the Receptor which callback function to call with every FFB packet it receives
                // It is the role of the designer to register the right FFB callback function

                // Note from me:
                // Warning: the callback is called in the context of a thread started by vJoyInterface.dll
                // when opening the joystick. This thread blocks upon a new system event from the driver.
                // It is perfectly ok to do some work in it, but do not overload it to avoid
                // loosing/desynchronizing FFB packets from the third party application.
                FFBReceiver.RegisterBaseCallback(Joystick, joyID, ffb);
            }
#endif // FFB
            return 0;
        }



        public void PublishiReport()
        {
            // Feed the driver with the position packet
            // If it fails, wait then try to re-acquire device
            bool stt = false;
            try {
                stt = Joystick.UpdateVJD(joyID, ref Report);
            } catch (Exception ex) {
                LogFormat(LogLevels.DEBUG, "vJoy device number {0}, exception {1}", joyID, ex.Message);
                Thread.Sleep(100);
            }
            if (!stt) {
                LogFormat(LogLevels.DEBUG, "Feeding vJoy device number {0} failed - trying to re-enable device", joyID);

                // Add some delay before re-enabling vJoy
                int ok = Acquire(joyID);
                if (ok != 1) {
                    LogFormat(LogLevels.ERROR, "Cannot acquire device number {0} - try to restart this program or check your vJoy installation", joyID);
                    Thread.Sleep(100);
                }

            }
        }

        public int GetNumberOfButtons()
        {
            return Joystick.GetVJDButtonNumber(joyID);
        }

        public uint GetButtonsState()
        {
            return Report.Buttons;
        }
        public void UpodateFirst32Buttons(uint buttonStates32)
        {
            Report.Buttons = buttonStates32;
        }

        public void Set1Button(int button)
        {
            // Get vJoy bit to change using mapping
            UInt32 vJoybit = (UInt32)(1<<button);
            // Clear
            Report.Buttons |= vJoybit;
        }
        public void SetButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Set1Button(buttons[i]);
            }
        }
        public void Clear1Button(int button)
        {
            // Get vJoy bit to change using mapping
            UInt32 vJoybit = ~(UInt32)(1<<button);
            // Clear
            Report.Buttons &= vJoybit;
        }
        public void ClearButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Clear1Button(buttons[i]);
            }
        }
        public void Toggle1Button(int button)
        {
            // Get vJoy bit to change using mapping
            UInt32 vJoybit = (UInt32)(1<<button);
            // Toggle
            Report.Buttons ^= vJoybit;
        }
        public void ToggleButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Toggle1Button(buttons[i]);
            }
        }



        public uint[] GetMoreButtonsState()
        {
            uint[] allstates = new uint[4];
            int indexAsJoy = 0;
            allstates[indexAsJoy++] = Report.Buttons;
            allstates[indexAsJoy++] = Report.ButtonsEx1;
            allstates[indexAsJoy++] = Report.ButtonsEx2;
            allstates[indexAsJoy++] = Report.ButtonsEx3;
            return allstates;
        }
        public void UpodateMoreButtons(uint[] buttonStates128)
        {
            int indexAsJoy = 0;
            if (buttonStates128.Length > indexAsJoy++) Report.Buttons = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx1 = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx2 = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx3 = buttonStates128[indexAsJoy - 1];
        }



        public void UpdateAxes12bits(uint[] rawaxes12)
        {
            if (rawaxes12 == null)
                return;
            int indexIn12 = 0;
            int indexAsJoyUsed = 0;
            for (; indexAsJoyUsed < UsedAxis.Count; indexAsJoyUsed++) {
                // Check input array is long enough
                if (indexIn12 >= rawaxes12.Length)
                    break;

                UsedAxis[indexAsJoyUsed].vJoyAxisInfo.RawValue = rawaxes12[indexIn12];
                UsedAxis[indexAsJoyUsed].vJoyAxisInfo.CorrectedValue = UsedAxis[indexAsJoyUsed].CorrectionSegment12bits((uint)rawaxes12[indexIn12]);

                indexIn12++;
            }

            CopyAxesValuesToReport();
        }

        public void UpdateAxes16bits(uint[] rawaxes16)
        {
            if (rawaxes16 == null)
                return;
            int indexIn16 = 0;
            int indexAsJoyUsed = 0;
            for (; indexAsJoyUsed < UsedAxis.Count; indexAsJoyUsed++) {
                // Check input array is long enough
                if (indexIn16 >= rawaxes16.Length)
                    break;

                UsedAxis[indexAsJoyUsed].vJoyAxisInfo.RawValue = rawaxes16[indexIn16];
                UsedAxis[indexAsJoyUsed].vJoyAxisInfo.CorrectedValue = UsedAxis[indexAsJoyUsed].CorrectionSegment16bits((uint)rawaxes16[indexIn16]);
                indexIn16++;
            }

            CopyAxesValuesToReport();
        }

        protected void CopyAxesValuesToReport()
        {
            int indexAsAllvJoy = 0;
            // Fill in by order of activated axes, as defined enum HID_USAGES
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisX = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisY = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisZ = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisXRot = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisYRot = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisZRot = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Slider = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Dial = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;

            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Wheel = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Accelerator = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Brake = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Clutch = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Steering = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Aileron = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Rudder = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;
            if (AllAxesInfo[indexAsAllvJoy++].IsPresent) Report.Throttle = (int)AllAxesInfo[indexAsAllvJoy - 1].CorrectedValue;

        }

        //POV Hat Switch members
        //The interpretation of these members depends on the configuration of the vJoy device.
        //Continuous: Valid value for POV Hat Switch member is either 0xFFFFFFFF (neutral) or in the
        //range of 0 to 35999 .
        //Discrete: Only member bHats is used. The lowest nibble is used for switch #1, the second nibble
        //for switch #2, the third nibble for switch #3 and the highest nibble for switch #4.
        //Each nibble supports one of the following values:
        //0x0 North (forward)
        //0x1 East (right)
        //0x2 South (backwards)
        //0x3 West (Left)
        //0xF Neutral
        static byte[] pov = new byte[4];
        public void UpodateDiscretePOVs()
        {
            /*
                // Make 5-position POV Hat spin

                pov[0] = (byte)(((count / 20) + 0) % 4);
                pov[1] = (byte)(((count / 20) + 1) % 4);
                pov[2] = (byte)(((count / 20) + 2) % 4);
                pov[3] = (byte)(((count / 20) + 3) % 4);

                iReport.bHats = (uint)(pov[3] << 12) | (uint)(pov[2] << 8) | (uint)(pov[1] << 4) | (uint)pov[0];
                if ((count) > 550)
                    iReport.bHats = 0xFFFFFFFF; // Neutral state
            };
            */
            /*
            if (false) {
                // One byte per hat
                // 0xFF = neutral, 0 = UP, 1 = RIGHT, 2 = DOWN, 3 = LEFT
                pov[0] = (byte)(00);
                pov[1] = (byte)(00);
                pov[2] = (byte)(00);
                pov[3] = (byte)(00);

                Report.bHats = (uint)(pov[3] << 12) | (uint)(pov[2] << 8) | (uint)(pov[1] << 4) | (uint)pov[0];
            }*/
            // Neutral state
            Report.bHats = 0xFFFFFFFF;
        }


        public void UpodateContinuousPOV(uint angles_hundredthdeg)
        {
            // Neutral state is given by -1 = 0xFFFFFFFF
            if (angles_hundredthdeg == 0xFFFFFFFF) {
                Report.bHats = 0xFFFFFFFF; // Neutral state
            } else {
                Report.bHats = angles_hundredthdeg % 35901;
            }
        }
        public void UpodateContinuousPOVs(uint[] angles_hundredthdeg)
        {
            /*
            if (ContPovNumber > 0) {
                // Make Continuous POV Hat spin
                iReport.bHats = (count * 70);
                iReport.bHatsEx1 = (count * 70) + 3000;
                iReport.bHatsEx2 = (count * 70) + 5000;
                iReport.bHatsEx3 = 15000 - (count * 70);
                if ((count * 70) > 36000) {
                    iReport.bHats = 0xFFFFFFFF; // Neutral state
                    iReport.bHatsEx1 = 0xFFFFFFFF; // Neutral state
                    iReport.bHatsEx2 = 0xFFFFFFFF; // Neutral state
                    iReport.bHatsEx3 = 0xFFFFFFFF; // Neutral state
                };
            } 
            */
            /*
            // For continuous POV hat spin
            if (false) {
                // Map angle 0 to 360 degrees to 0..36000
                Report.bHats++;
                if (Report.bHats > 36000)
                    Report.bHats = 0;
            }
            */
            // Neutral state is given by -1 = 0xFFFFFFFF
            if (angles_hundredthdeg[0] == 0xFFFFFFFF) {
                Report.bHats = 0xFFFFFFFF; // Neutral state
            } else {
                Report.bHats = angles_hundredthdeg[0] % 36000;
            }
        }
    }
}
