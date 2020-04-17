using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.Outputs
{
    /// <summary>
    /// Raw memory output agent.
    /// Use process memory read/write.
    /// Code converted from M2DUMP, M13DUMP, DaytonaUSB
    /// </summary>
    public class RawMemoryOutputsAgent : Outputs
    {
        protected ulong DriveAddr;
        protected ulong LampAddr;
        protected ulong ProfileAddr;

        protected ProcessManipulation GameProcess;




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


        bool detected = false;
        protected void ManagerThreadMethod()
        {
            detected = false;
            while (Running) {
                if (!detected) {
                    if (ScanForKnownGameEmulator())
                        detected = true;

                } else {
                    GetLampsData();
                    GetDriveData();
                }
                Thread.Sleep(32);
            }
            Logger.Log("[MAMENetOutput] TCP connection terminated", LogLevels.INFORMATIVE);
        }

        public virtual bool ScanForKnownGameEmulator()
        {
            if (GameProcess != null)
                GameProcess.CloseProcess();
            GameProcess = new ProcessManipulation();
            return true;
        }



        public byte GetDriveData()
        {
            var stt = GameProcess.ReadByte(DriveAddr, out byte value);
            if (!stt) detected = false;
            this.DriveValue = value;
            return value;
        }

        public byte GetLampsData()
        {
            var stt = GameProcess.ReadByte(LampAddr, out byte value);
            if (!stt) detected = false;
            this.LampsValue = value;
            return value;
        }

    }
}
