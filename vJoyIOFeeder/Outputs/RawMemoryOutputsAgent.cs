using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public override void Start()
        {
            ScanForKnownGameEmulator();
        }
        public override void Stop()
        {
            
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
            GameProcess.ReadByte(DriveAddr, out byte value);
            this.DriveValue = value;
            return value;
        }

        public byte GetLampsData()
        {
            GameProcess.ReadByte(LampAddr, out byte value);
            this.LampsValue = value;
            return value;
        }

    }
}
