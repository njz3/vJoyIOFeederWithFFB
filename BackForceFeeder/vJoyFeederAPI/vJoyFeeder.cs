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
using BackForceFeeder.Inputs;

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

        /// <summary>
        /// vJoy device report that will be used to update driver's values.
        /// </summary>
        public vJoy.JoystickState Report;

        /// <summary>
        /// FFB packet receiver
        /// </summary>
        public vJoyFFBReceiver FFBReceiver;

        /// <summary>
        /// vJoy device ID
        /// </summary>
        public uint vJoyDevID { get; protected set; }
        /// <summary>
        /// vJoy device pointer (wrapper)
        /// </summary>
        protected vJoy Joystick;

        public UInt32 vJoyVersionDll = 0;
        public UInt32 vJoyVersionDriver = 0;
        public bool vJoyVersionMatch = false;

        /// <summary>
        /// Default list of axes for HID device.
        /// Not all will be used
        /// </summary>
        protected List<vJoyAxisInfos> HIDAxesInfo = new List<vJoyAxisInfos>(MAX_AXES_VJOY);

        /// <summary>
        /// Actual list of vJoy configured axes.
        /// Only accessible through SafeGetvJoyAxisInfo()
        /// </summary>
        protected List<vJoyAxisInfos> ConfiguredvJoyAxes = new List<vJoyAxisInfos>(MAX_AXES_VJOY);

        /// <summary>
        /// Number of configured axes
        /// </summary>
        public int NbAxes {
            get {
                return this.ConfiguredvJoyAxes.Count;
            }
        }

        /// <summary>
        /// Number of configured buttons
        /// </summary>
        public int NbButtons { get; protected set; }

        public vJoyAxisInfos SafeGetvJoyAxisInfo(int selectedvJoyAxis)
        {
            if (selectedvJoyAxis<0 || selectedvJoyAxis>=this.ConfiguredvJoyAxes.Count)
                return null;
            return this.ConfiguredvJoyAxes[selectedvJoyAxis];
        }

        public vJoyFeeder()
        {
            NbButtons = -1;
            // Create one joystick object and a position structure.
            Joystick = new vJoy();
            Report = new vJoy.JoystickState();
            FFBReceiver = new vJoyFFBReceiver();

            // Prepare list of vJoy's axes configuration. This is done
            // only one time after retreiving vJoy's information

            // Fill all axes
            HIDAxesInfo.Clear();
            foreach (HID_USAGES toBeTested in Enum.GetValues(typeof(HID_USAGES))) {
                // Skip POV
                if (toBeTested == HID_USAGES.HID_USAGE_POV)
                    continue;
                var name = toBeTested.ToString().Replace("HID_USAGE_", "");
                var axisinfo = new vJoyAxisInfos();
                axisinfo.HID_Usage = toBeTested;
                axisinfo.Name = name;
                HIDAxesInfo.Add(axisinfo);
            }

            // Fill OnlyUsedInfo by copying reference from AllAxesInfo once done
            ConfiguredvJoyAxes.Clear();
        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[VJOYFEEDER] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[VJOYFEEDER] " + text, args);
        }

        /// <summary>
        /// Enable vJoy
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Acquire a vJoy device ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Acquire(uint id)
        {
            // Device ID can only be in the range 1-16
            vJoyDevID = id;
            Report.bDevice = (byte)vJoyDevID;
            if (vJoyDevID <= 0 || vJoyDevID > 16) {
                LogFormat(LogLevels.ERROR, "Illegal device ID {0}\nExit!", vJoyDevID);
                return -1;
            }

            // Get the state of the requested device
            VjdStat status = Joystick.GetVJDStatus(vJoyDevID);
            switch (status) {
                case VjdStat.VJD_STAT_OWN:
                    LogFormat(LogLevels.DEBUG, "vJoy Device {0} is already owned by this feeder", vJoyDevID);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    LogFormat(LogLevels.DEBUG, "vJoy Device {0} is free", vJoyDevID);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} is already owned by another feeder\nCannot continue", vJoyDevID);
                    return -3;
                case VjdStat.VJD_STAT_MISS:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} is not installed or disabled\nCannot continue", vJoyDevID);
                    return -4;
                default:
                    LogFormat(LogLevels.ERROR, "vJoy Device {0} general error\nCannot continue", vJoyDevID);
                    return -1;
            };


            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            this.NbButtons = Joystick.GetVJDButtonNumber(vJoyDevID);
            int ContPovNumber = Joystick.GetVJDContPovNumber(vJoyDevID);
            int DiscPovNumber = Joystick.GetVJDDiscPovNumber(vJoyDevID);

            // Print results
            LogFormat(LogLevels.DEBUG, "vJoy Device {0} capabilities:", vJoyDevID);
            LogFormat(LogLevels.DEBUG, "Numner of buttons\t\t{0}", this.NbButtons);
            LogFormat(LogLevels.DEBUG, "Numner of Continuous POVs\t{0}", ContPovNumber);
            LogFormat(LogLevels.DEBUG, "Numner of Descrete POVs\t\t{0}", DiscPovNumber);

            // Check which axes are supported. Follow enum HID_USAGES
            ConfiguredvJoyAxes.Clear();
            int i = 0;
            foreach (HID_USAGES toBeTested in Enum.GetValues(typeof(HID_USAGES))) {
                // Skip POV
                if (toBeTested == HID_USAGES.HID_USAGE_POV)
                    continue;
                var present = Joystick.GetVJDAxisExist(vJoyDevID, toBeTested);
                LogFormat(LogLevels.DEBUG, "Axis " + HIDAxesInfo[i].Name + " \t\t{0}", present ? "Yes" : "No");
                if (present) {
                    HIDAxesInfo[i].IsPresent = present;
                    // Retrieve min/max from vJoy
                    if (!Joystick.GetVJDAxisMin(vJoyDevID, toBeTested, ref HIDAxesInfo[i].MinValue)) {
                        Log("Failed getting min value!");
                    }
                    if (!Joystick.GetVJDAxisMax(vJoyDevID, toBeTested, ref HIDAxesInfo[i].MaxValue)) {
                        Log("Failed getting min value!");
                    }
                    Log(" Min= " + HIDAxesInfo[i].MinValue + " Max=" + HIDAxesInfo[i].MaxValue);

                    // Add to indexed list of configured axes
                    ConfiguredvJoyAxes.Add(HIDAxesInfo[i]);
                }
                i++;
            }

            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!Joystick.AcquireVJD(vJoyDevID)))) {
                LogFormat(LogLevels.ERROR, "Failed to acquire vJoy device number {0}.", vJoyDevID);
                return -1;
            } else {
                LogFormat(LogLevels.DEBUG, "Acquired: vJoy device number {0}.", vJoyDevID);
            }
            return (int)status;
        }

        /// <summary>
        /// Release a previously acquired vJoy device ID
        /// </summary>
        public void Release()
        {
            Joystick.RelinquishVJD(vJoyDevID);
        }

        /// <summary>
        /// Register a FFB callback for current vJoy device ID
        /// </summary>
        /// <param name="ffb"></param>
        /// <returns></returns>
        public int StartAndRegisterFFB(FFBManager ffb)
        {
            // Start FFB
#if FFB
            if (Joystick.IsDeviceFfb(vJoyDevID)) {
                // Register Generic callback function
                // At this point you instruct the Receptor which callback function to call with every FFB packet it receives
                // It is the role of the designer to register the right FFB callback function

                // Note from me:
                // Warning: the callback is called in the context of a thread started by vJoyInterface.dll
                // when opening the joystick. This thread blocks upon a new system event from the driver.
                // It is perfectly ok to do some work in it, but do not overload it to avoid
                // loosing/desynchronizing FFB packets from the third party application.
                FFBReceiver.RegisterBaseCallback(Joystick, vJoyDevID, ffb);
            }
#endif // FFB
            return 0;
        }


        /// <summary>
        /// Update vJoy device at driver side.
        /// This call is almost intantaneous, but we add a sleep of 100ms in 
        /// case of an error
        /// </summary>
        public void PublishReport()
        {
            // Feed the driver with the position packet
            // If it fails, wait then try to re-acquire device
            bool stt = false;
            try {
                stt = Joystick.UpdateVJD(vJoyDevID, ref Report);
            } catch (Exception ex) {
                LogFormat(LogLevels.DEBUG, "vJoy device number {0}, exception {1}", vJoyDevID, ex.Message);
                Thread.Sleep(100);
            }
            if (!stt) {
                LogFormat(LogLevels.DEBUG, "Feeding vJoy device number {0} failed - trying to re-enable vJoy device", vJoyDevID);

                // Add some delay before re-enabling vJoy
                int ok = Acquire(vJoyDevID);
                if (ok != 1) {
                    LogFormat(LogLevels.ERROR, "Cannot acquire device number {0} - try to restart this program or check your vJoy installation", vJoyDevID);
                    Thread.Sleep(100);
                }

            }
        }

        #region Buttons
        public void GetButtonsStates(ref UInt64 btn0_63, ref UInt64 btn64_127)
        {
            btn0_63   = ((UInt64)Report.ButtonsEx1<<32) | (UInt64)Report.Buttons;
            btn64_127 = ((UInt64)Report.ButtonsEx3<<32) | (UInt64)Report.ButtonsEx2;
        }
        public void UpodateAllButtons(UInt64 btn0_63, UInt64 btn64_127)
        {
            Report.Buttons    = (UInt32)(btn0_63 & 0xFFFF);
            Report.ButtonsEx1 = (UInt32)(btn0_63 >> 32);
            Report.ButtonsEx2 = (UInt32)(btn64_127 & 0xFFFF);
            Report.ButtonsEx3 = (UInt32)(btn64_127 >> 32);
        }


        /// <summary>
        /// Set a button given its index (0..127) using a OR
        /// </summary>
        /// <param name="btnidx">0..127</param>
        public void Set1Button(int btnidx)
        {
            // Get 32bit word index 0..3
            int wd = (btnidx>>5) & 0x3;
            // Get bit index within word
            int bit = btnidx & 0x1F;
            // Get actual bit value to change in the word
            UInt32 vJoybit = (UInt32)(1<<btnidx);
            // Set
            switch (wd) {
                case 0:
                    Report.Buttons |= vJoybit;
                    break;
                case 1:
                    Report.ButtonsEx1 |= vJoybit;
                    break;
                case 2:
                    Report.ButtonsEx2 |= vJoybit;
                    break;
                case 3:
                    Report.ButtonsEx3 |= vJoybit;
                    break;
            }
        }
        public void SetButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Set1Button(buttons[i]);
            }
        }

        /// <summary>
        /// Clear a button given its index (0..127) using a AND
        /// </summary>
        /// <param name="btnidx"></param>
        public void Clear1Button(int btnidx)
        {
            // Get 32bit word index 0..3
            int wd = (btnidx>>5) & 0x3;
            // Get bit index within word
            int bit = btnidx & 0x1F;
            // Get actual mask value to change in the word
            UInt32 vJoybit = ~(UInt32)(1<<bit);
            // Set
            switch (wd) {
                case 0:
                    Report.Buttons &= vJoybit;
                    break;
                case 1:
                    Report.ButtonsEx1 &= vJoybit;
                    break;
                case 2:
                    Report.ButtonsEx2 &= vJoybit;
                    break;
                case 3:
                    Report.ButtonsEx3 &= vJoybit;
                    break;
            }
        }
        public void ClearButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Clear1Button(buttons[i]);
            }
        }
        /// <summary>
        /// TOggle a button given its index (0..127) using a XOR
        /// </summary>
        /// <param name="btnidx"></param>
        public void Toggle1Button(int btnidx)
        {
            // Get 32bit word index 0..3
            int wd = (btnidx>>5) & 0x3;
            // Get bit index within word
            int bit = btnidx & 0x1F;
            // Get actual bit value to change in the word
            UInt32 vJoybit = (UInt32)(1<<bit);
            // Set
            switch (wd) {
                case 0:
                    Report.Buttons ^= vJoybit;
                    break;
                case 1:
                    Report.ButtonsEx1 ^= vJoybit;
                    break;
                case 2:
                    Report.ButtonsEx2 ^= vJoybit;
                    break;
                case 3:
                    Report.ButtonsEx3 ^= vJoybit;
                    break;
            }
        }
        public void ToggleButtons(List<int> buttons)
        {
            for (int i = 0; i<buttons.Count; i++) {
                Toggle1Button(buttons[i]);
            }
        }
        #endregion

        #region Axes
        /// <summary>
        /// Update a vJoy axis given 12bits value.
        /// Axis mapping and scaling is performed here.
        /// </summary>
        /// <param name="axisindex">axis index (0..NbAxis)</param>
        /// <param name="axis_pct">value as a percent 0..1.0</param>
        public void Update1Axis(int axisindex, double axis_pct)
        {
            // Check input array is long enough
            if (axisindex >= ConfiguredvJoyAxes.Count)
                return;
            var axis = ConfiguredvJoyAxes[axisindex];
            axis.AxisValue_pct = axis_pct;

            CopyAxesValuesToReport();
        }

        /// <summary>
        /// Update vJoy axes given 12bits values.
        /// Axis mapping and scaling is performed here.
        /// </summary>
        /// <param name="axes_pct"></param>
        public void UpdateAllAxes(double[] axes_pct)
        {
            if (axes_pct == null)
                return;
            int indexIn = 0;
            int indexAsJoyUsed = 0;
            for (; indexAsJoyUsed < ConfiguredvJoyAxes.Count; indexAsJoyUsed++) {
                // Check input array is long enough
                if (indexIn >= axes_pct.Length)
                    break;
                var axis = ConfiguredvJoyAxes[indexAsJoyUsed];
                axis.AxisValue_pct = axes_pct[indexIn];
                indexIn++;
            }

            CopyAxesValuesToReport();
        }

        protected void CopyAxesValuesToReport()
        {
            int indexAsAllvJoy = 0;
            // Fill in by order of activated axes, as defined enum HID_USAGES
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisX = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisY = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisZ = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisXRot = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisYRot = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.AxisZRot = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Slider = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Dial = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;

            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Wheel = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Accelerator = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Brake = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Clutch = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Steering = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Aileron = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Rudder = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;
            if (HIDAxesInfo[indexAsAllvJoy++].IsPresent) Report.Throttle = (int)HIDAxesInfo[indexAsAllvJoy - 1].AxisValue_cts;

        }
        #endregion

        #region POV Hat Switch members
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
        #endregion
    }
}
