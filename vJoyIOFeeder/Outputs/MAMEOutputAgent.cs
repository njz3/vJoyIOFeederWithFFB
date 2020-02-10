using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    public abstract class MAMEOutputAgent : IOutput
    {

        public MAMEOutputAgent() :
            base()
        {
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
                            this.LampsValue |= (int)(1<<2);
                        } else {
                            this.LampsValue &= ~(int)(1<<2);
                        }
                    }
                    break;

                case "Start_lamp": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<0);
                        } else {
                            this.LampsValue &= ~(int)(1<<0);
                        }
                    }
                    break;
                case "Brake_lamp": {
                        int.TryParse(tokens[1], out int result);
                        if (result!=0) {
                            this.LampsValue |= (int)(1<<1);
                        } else {
                            this.LampsValue &= ~(int)(1<<1);
                        }
                    }
                    break;
            }
        }


    }

}
