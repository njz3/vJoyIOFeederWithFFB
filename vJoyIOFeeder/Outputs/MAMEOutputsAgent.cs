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




        protected bool Running = true;
        protected Thread ManagerThread = null;

        public override void Start()
        {
            if (ManagerThread != null) {
                Stop();
            }

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
                    case "outrun":
                        GameProcessMessage = ProcessOutrun;
                        break;
                    case "lemans24":
                    case "daytona2":
                    case "scud":
                        GameProcessMessage = ProcessModel3;
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
                case "Bank_Motor_Direction":
                case "Bank_Motor_Speed":
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
                        if (int.TryParse(tokens[1], NumberStyles.AllowHexSpecifier|NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)) {

                        }
                    }
                    break;
                case "digit1": {
                        if (int.TryParse(tokens[1], NumberStyles.AllowHexSpecifier|NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)) {
                            this.DriveValue = result;
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
            // Lamps Coin1 Coin2 Start Red Blue Yellow Green Leader
            switch (tokens[0]) {
                case "RawLamps": {
                        if (int.TryParse(tokens[1], NumberStyles.AllowHexSpecifier|NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)) {
                            this.LampsValue = result;
                        }
                    }
                    break;
                case "RawDrive": {
                        if (int.TryParse(tokens[1], NumberStyles.AllowHexSpecifier|NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)) {
                            this.DriveValue = result;
                        }
                    }
                    break;
            }
        }
    }

}
