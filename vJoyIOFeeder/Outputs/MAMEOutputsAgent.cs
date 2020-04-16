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
                        return;
                }
            }

            GameProcessMessage(line);
        }


        char[] SplitChars = new char[] { ' ', '=', '\n', '\t' };
        protected void ProcessOutrun(string line)
        {
            if (line.Length<3)
                return;
            var tokens = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length<2)
                return;

            switch (tokens[0]) {
                case "pause":
                case "Bank_Motor_Direction":
                case "Bank_Motor_Speed":
                case @"Orientation(\\.\DISPLAY1)":
                    break;
                case "Vibration_motor": {
                        // Vibration
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<4);
                        } else {
                            this.LampsValue &= ~(int)(1<<4);
                        }
                    }
                    break;

                case "Start_lamp": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<2);
                        } else {
                            this.LampsValue &= ~(int)(1<<2);
                        }
                    }
                    break;
                case "Brake_lamp": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<3);
                        } else {
                            this.LampsValue &= ~(int)(1<<3);
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
                case "LampStart": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<2);
                        } else {
                            this.LampsValue &= ~(int)(1<<2);
                        }
                    }
                    break;
                case "LampView1": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<3);
                        } else {
                            this.LampsValue &= ~(int)(1<<3);
                        }
                    }
                    break;
                case "LampView2": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<4);
                        } else {
                            this.LampsValue &= ~(int)(1<<4);
                        }
                    }
                    break;
                case "LampView3": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<5);
                        } else {
                            this.LampsValue &= ~(int)(1<<5);
                        }
                    }
                    break;
                case "LampView4": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<6);
                        } else {
                            this.LampsValue &= ~(int)(1<<6);
                        }
                    }
                    break;
                case "LampLeader": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<7);
                        } else {
                            this.LampsValue &= ~(int)(1<<7);
                        }
                    }
                    break;
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
