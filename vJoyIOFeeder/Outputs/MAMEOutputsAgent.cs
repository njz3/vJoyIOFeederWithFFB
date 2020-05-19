using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.Outputs
{
    /// <summary>
    /// MAME (and Supermodel) abstract class for output agents
    /// </summary>
    public abstract class MAMEOutputsAgent : Outputs
    {

        public MAMEOutputsAgent() :
            base()
        {
        }

        public void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[MAMEOutput] " + text, level);
        }

        public void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[MAMEOutput] " + text, args);
        }




        protected bool Running = false;
        protected Thread ManagerThread = null;

        public override void Start()
        {
            // Check already running
            if (Running) return;
            if (ManagerThread != null) Stop();

            ManagerThread = new Thread(ManagerThreadMethod);
            Running = true;
            ManagerThread.Name = "vJoy MAME Output";
            ManagerThread.Priority = ThreadPriority.BelowNormal;
            ManagerThread.IsBackground = true;
            ManagerThread.SetApartmentState(ApartmentState.STA);
            ManagerThread.Start();
        }

        public override void Stop()
        {
            if (!Running) return;

            Running = false;
            if (ManagerThread == null)
                return;
            Thread.Sleep(100);
            ManagerThread.Join();
            ManagerThread = null;
        }

        protected abstract void ManagerThreadMethod();

        protected delegate void SpecialProcessMessage(string line);
        protected SpecialProcessMessage GameProcessMessage;

        protected void ProcessMessage(string line)
        {
            if (GameProcessMessage==null) {
                switch (GameProfile) {
                    case "outrun": // Outrun
                    case "orunnersu": // Outrunnner (US)
                    case "mt_tout": // Turbo outrun (Mega-Tech)
                        GameProcessMessage = ProcessOutrun;
                        break;
                    case "lemans24":
                    case "daytona2":
                    case "scud":
                        GameProcessMessage = ProcessModel3;
                        break;
                    case "vr":
                        GameProcessMessage = ProcessVirtuaRacing;
                        break;
                    default:
                        // Unknown game
                        GameProcessMessage = ProcessCommonMAME;
                        return;
                }
            }

            GameProcessMessage(line);
        }


        char[] SplitChars = new char[] { ' ', '=', '\n', '\t' };

        protected void ProcessCommonMAME(string line)
        {
            if (line.Length<3)
                return;
            var tokens = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length<2)
                return;

            switch (tokens[0]) {
                case "pause":
                    // Game pause
                    break;
                case @"Orientation(\\.\DISPLAY1)":
                    // Screen orientation
                    break;
                case "LampStart":
                case "Start_lamp": {
                        // Start button
                        // 1 bit, pos 2 (base 0)
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<2);
                        } else {
                            this.LampsValue &= ~(int)(1<<2);
                        }
                    }
                    break;
                // Single Bit base 0
                case "out0":
                case "out1":
                case "out2":
                case "out3":
                case "out4":
                case "out5":
                case "out6":
                case "out7":
                case "led0":
                case "led1":
                case "led2":
                case "led3":
                case "led4":
                case "led5":
                case "led6":
                case "led7":
                case "cpuled0":
                case "cpuled1":
                case "cpuled2":
                case "cpuled3":
                case "cpuled4":
                case "cpuled5":
                case "cpuled6":
                case "cpuled7":
                case "lamp0":
                case "lamp1":
                case "lamp2":
                case "lamp3":
                case "lamp4":
                case "lamp5":
                case "lamp6":
                case "lamp7":
                // 8 Bits, pos 2 to 9 (base 0)
                case "LampView1":
                case "LampView2":
                case "LampView3":
                case "LampView4": {
                        // 4 Bits, pos 3 to 6 (base 0)
                        int.TryParse(tokens[0].Substring(tokens[0].Length-1), out var bit);
                        // All shifted by 2 to avoid replacing PWM dir
                        bit +=2;
                        // Vibration
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<bit);
                        } else {
                            this.LampsValue &= ~(int)(1<<bit);
                        }
                        break;
                    }
                case "Brake_lamp":
                // Car rear "brake" light in Outrun
                case "LampLeader": {
                        // 1 bit, pos 7 (base 0)
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<7);
                        } else {
                            this.LampsValue &= ~(int)(1<<7);
                        }
                    }
                    break;
            }
        }
        protected void ProcessOutrun(string line)
        {
            if (line.Length<3)
                return;
            var tokens = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length<2)
                return;
            ProcessCommonMAME(line);
            switch (tokens[0]) {
                // Outrun
                case "Bank_Motor_Direction":
                case "Bank_Motor_Speed":
                    break;
                case "Vibration_motor": {
                        // Driveboard: 1 bit, pos 0 (base 0)
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.DriveValue |= (int)(1<<0);
                        } else {
                            this.DriveValue &= ~(int)(1<<0);
                        }
                    }
                    break;
                // Outrunners (US): two machines MA and MB
                case "MA_Steering_Wheel_motor":
                case "MB_Steering_Wheel_motor": {
                        // Driveboard: 1 bit, pos 0 or 4 (base 0)
                        int.TryParse(tokens[1], out int result);
                        int pos = tokens[0].StartsWith("MA") ? 0 : 4;
                        if (result!=0) {
                            this.DriveValue |= (int)(1<<pos);
                        } else {
                            this.DriveValue &= ~(int)(1<<pos);
                        }
                    }
                    break;
                case "MA_Check_Point_lamp":
                case "MB_Check_Point_lamp": {
                        // 1 bit, pos 7 (base 0)
                        int.TryParse(tokens[1], out int result);
                        int pos = tokens[0].StartsWith("MA") ? 0 : 4;
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<pos);
                        } else {
                            this.LampsValue &= ~(int)(1<<pos);
                        }
                    }
                    break;
                case "MA_Race_Leader_lamp":
                case "MB_Race_Leader_lamp": {
                        // 1 bit, pos 7 (base 0)
                        int.TryParse(tokens[1], out int result);
                        int pos = tokens[0].StartsWith("MA") ? 1 : 5;
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<pos);
                        } else {
                            this.LampsValue &= ~(int)(1<<pos);
                        }
                    }
                    break;
                case "MA_DJ_Music_lamp":
                case "MB_DJ_Music_lamp": {
                        // 1 bit, pos 7 (base 0)
                        int.TryParse(tokens[1], out int result);
                        int pos = tokens[0].StartsWith("MA") ? 2 : 6;
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<pos);
                        } else {
                            this.LampsValue &= ~(int)(1<<pos);
                        }
                    }
                    break;
                case "MA_<<_>>_lamp":
                case "MB_<<_>>_lamp": {
                        // 1 bit, pos 7 (base 0)
                        int.TryParse(tokens[1], out int result);
                        int pos = tokens[0].StartsWith("MA") ? 3 : 7;
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<pos);
                        } else {
                            this.LampsValue &= ~(int)(1<<pos);
                        }
                    }
                    break;
                // Turbo Outrun (Mega-Tech)
                case "Alarm_sound": {
                        // Driveboard: 1 bit, pos 0 (base 0)
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.DriveValue |= (int)(1<<0);
                        } else {
                            this.DriveValue &= ~(int)(1<<0);
                        }
                    }
                    break;
                case "Flash_screen": {
                        // Driveboard: 1 bit, pos 1 (base 0)
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.DriveValue |= (int)(1<<1);
                        } else {
                            this.DriveValue &= ~(int)(1<<1);
                        }
                    }
                    break;
            }
        }
        protected void ProcessVirtuaRacing(string line)
        {
            if (line.Length<3)
                return;
            var tokens = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length<2)
                return;
            ProcessCommonMAME(line);

            switch (tokens[0]) {
                case "digit0": {
                        if (Converting.HexStrToInt(tokens[1], out int result)) {
                            this.DriveValue = result;
                        }
                    }
                    break;
                case "digit1": {
                        if (Converting.HexStrToInt(tokens[1], out int result)) {
                            this.LampsValue = result;
                        }
                    }
                    break;
            }
        }

        protected void ProcessModel3(string line)
        {
            if (line.Length<3)
                return;
            var tokens = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length<2)
                return;
            ProcessCommonMAME(line);
            // Lamps Coin1 Coin2 Start Red Blue Yellow Green Leader
            switch (tokens[0]) {
                case "RawLamps": {
                        if (Converting.HexStrToInt(tokens[1], out int result)) {
                            this.LampsValue = result;
                        }
                    }
                    break;
                case "RawDrive": {
                        if (Converting.HexStrToInt(tokens[1], out int result)) {
                            this.DriveValue = result;
                        }
                    }
                    break;
            }
        }
    }

}
