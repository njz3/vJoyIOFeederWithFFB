#define CONSOLE_DUMP
#define FFB

// From vJoy SDK example

using System;
using System.Threading;

// Don't forget to add this
using vJoyInterfaceWrap;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.vJoyIOFeederAPI
{
    // Singleton/static
    public class vJoyFeeder
    {

        // Declaring one joystick (Device id 1) and a position structure. 
        public vJoy.JoystickState Report;
        public vJoyFFBReceiver FFBReceiver;
        public uint joyID {
            get;
            protected set;
        }
        protected vJoy Joystick;

        /// <summary>
        /// Maximum supported by vJoy : 8 axes
        /// X Y Z RX RY RZ SL0 SL1
        /// </summary>
        public struct vJoyAxisInfos
        {
            public bool IsPresent;
            public long CurrentValue;
            public long MinValue;
            public long MaxValue;

            // For full correction
            public double Offset;
            public double Min;
            public double Max;
            public double Slope;
            public double Exp;
            public void ResetCorrectionFactors()
            {
                Offset = 0.0;
                Min = 0.0;
                Max = 1.0;
                Slope = 1.0;
                Exp = 1.0;
            }

            public double Normalize12b(uint axe12bits)
            {
                // Scale input to 0.0 ... 1.0
                return (double)(axe12bits) * (1.0 / (double)0xFFF);
            }

            public double Normalize16b(uint axe16bits)
            {
                // Scale input to 0.0 ... 1.0
                return (double)(axe16bits) * (1.0 / (double)0xFFFF);
            }

            public int CorrectionMinMax(double scaled_f)
            {
                // get back into axis range
                var finalvalue = (int)(MinValue + (MaxValue - MinValue) * scaled_f);
                return finalvalue;
            }

            public int CorrectionMed(double scaled_f)
            {
                var med = (MaxValue + MinValue) / 2;
                // get back into axis range
                var finalvalue = (int)(med + (MaxValue - med) * scaled_f);
                return finalvalue;
            }
            public int FullCorrection(double scaled_f)
            {
                // Shift by offset and apply slope to get linear correction 
                double linear = Math.Max(0, scaled_f - Offset) * Slope;
                // Apply geometric correction and add min value
                double pow = Math.Pow(linear, Exp) + Min;
                // Apply saturations
                double saturated = Math.Min(Max, Math.Max(Min, pow));
                // get back into axis range
                var finalvalue = (int)(MinValue + (MaxValue - MinValue) * saturated);
                return finalvalue;
            }
            public int FullCorrection12(uint axe12bits)
            {
                return FullCorrection(Normalize12b(axe12bits));
            }
            public int FullCorrection16(uint axe16bits)
            {
                return FullCorrection(Normalize16b(axe16bits));
            }
        }

        public vJoyAxisInfos[] AxesInfo = new vJoyAxisInfos[8];

        public vJoyFeeder()
        {
            // Create one joystick object and a position structure.
            Joystick = new vJoy();
            Report = new vJoy.JoystickState();
            FFBReceiver = new vJoyFFBReceiver();
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
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = Joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match) {
                LogFormat(LogLevels.DEBUG, "Version of Driver Matches DLL Version ({0:X})", DllVer);
                return 1;
            } else {
                LogFormat(LogLevels.ERROR, "Version of Driver ({0:X}) does NOT match DLL Version ({1:X})", DrvVer, DllVer);
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

            // Check which axes are supported. Follow enum HID_USAGES, up to 8
            for (int i = 0; i < AxesInfo.Length; i++) {
                HID_USAGES toBeTested = (HID_USAGES)HID_USAGES.HID_USAGE_X + i;
                var present = Joystick.GetVJDAxisExist(joyID, toBeTested);
                LogFormat(LogLevels.DEBUG, "Axis " + toBeTested.ToString() + " \t\t{0}", present ? "Yes" : "No");
                if (present) {
                    AxesInfo[i].IsPresent = present;
                    AxesInfo[i].ResetCorrectionFactors();
                    // Retrieve min/max from vJoy
                    if (!Joystick.GetVJDAxisMin(joyID, toBeTested, ref AxesInfo[i].MinValue)) {
                        Log("Failed getting min value!");
                    }
                    if (!Joystick.GetVJDAxisMax(joyID, toBeTested, ref AxesInfo[i].MaxValue)) {
                        Log("Failed getting min value!");
                    }
                    Log(" Min= " + AxesInfo[i].MinValue + " Max=" + AxesInfo[i].MaxValue);
                }
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

        public int StartAndRegisterFFB(IFFBManager ffb)
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
                FFBReceiver.RegisterBaseCallback(Joystick, ffb);
            }
#endif // FFB
            return 0;
        }



        public void PublishiReport()
        {
            // Feed the driver with the position packet
            // If it fails, wait then try to re-acquire device
            if (!Joystick.UpdateVJD(joyID, ref Report)) {
                LogFormat(LogLevels.DEBUG, "Feeding vJoy device number {0} failed - trying to re-enable device", joyID);

                // Add some delay before re-enabling vJoy
                var stt = Acquire(joyID);
                if (stt != 1) {
                    LogFormat(LogLevels.ERROR, "Cannot acquire device number {0} - try to restart this program", joyID);
                    //Console.ReadKey(true);
                }

            }
        }

        public void UpodateFirst32Buttons(uint buttonStates32)
        {
            Report.Buttons = buttonStates32;
        }

        public void UpodateMoreButtons(uint[] buttonStates128)
        {
            int indexAsJoy = 0;
            if (buttonStates128.Length > indexAsJoy++) Report.Buttons = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx1 = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx2 = buttonStates128[indexAsJoy - 1];
            if (buttonStates128.Length > indexAsJoy++) Report.ButtonsEx3 = buttonStates128[indexAsJoy - 1];
        }




        public void UpdateAxes12(uint[] axes12)
        {
            if (axes12 == null)
                return;
            int indexIn12 = 0;
            int indexAsJoy = 0;
            for (; indexAsJoy < AxesInfo.Length; indexAsJoy++) {
                if (AxesInfo[indexAsJoy].IsPresent && axes12.Length > indexIn12) {
                    AxesInfo[indexAsJoy].CurrentValue = AxesInfo[indexAsJoy].FullCorrection12(axes12[indexIn12++]);
                }
            }

            CopyAxesValuesToReport();
        }

        public void UpdateAxes16(uint[] axes16)
        {
            if (axes16 == null)
                return;
            int indexIn16 = 0;
            int indexAsJoy = 0;
            for (; indexAsJoy < AxesInfo.Length; indexAsJoy++) {
                if (AxesInfo[indexAsJoy].IsPresent && axes16.Length > indexIn16) {
                    AxesInfo[indexAsJoy].CurrentValue = AxesInfo[indexAsJoy].FullCorrection16(axes16[indexIn16++]);
                }
            }

            CopyAxesValuesToReport();
        }

        protected void CopyAxesValuesToReport()
        {
            int indexAsJoy = 0;
            // Fill in by order of activated axes, as defined enum HID_USAGES
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisX = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisY = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisZ = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisXRot = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisYRot = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.AxisZRot = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.Slider = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
            if (AxesInfo[indexAsJoy++].IsPresent) Report.Dial = (int)AxesInfo[indexAsJoy - 1].CurrentValue;
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
            if (false) {
                // One byte per hat
                // 0xFF = neutral, 0 = UP, 1 = RIGHT, 2 = DOWN, 3 = LEFT
                pov[0] = (byte)(00);
                pov[1] = (byte)(00);
                pov[2] = (byte)(00);
                pov[3] = (byte)(00);

                Report.bHats = (uint)(pov[3] << 12) | (uint)(pov[2] << 8) | (uint)(pov[1] << 4) | (uint)pov[0];
            }
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
            // For continuous POV hat spin
            if (false) {
                // Map angle 0 to 360 degrees to 0..36000
                Report.bHats++;
                if (Report.bHats > 36000)
                    Report.bHats = 0;
            }
            // Neutral state is given by -1 = 0xFFFFFFFF
            if (angles_hundredthdeg[0] == 0xFFFFFFFF) {
                Report.bHats = 0xFFFFFFFF; // Neutral state
            } else {
                Report.bHats = angles_hundredthdeg[0] % 36000;
            }
        }
    }
}
