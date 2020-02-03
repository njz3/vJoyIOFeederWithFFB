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
    /// M2 emulator output agent.
    /// Use process memory read/write.
    /// Code converted from M2DUMP, M13DUMP, DaytonaUSB
    /// </summary>
    public class M2OutputAgent : IOutput
    {
        ulong DriveAddr;
        ulong LampAddr;
        ulong ProfileAddr;

        ProcessManipulation M2EmulatorProcess;
        ProcessManipulation SupermodeEmulatorProcess;
        ProcessManipulation MAMEProcess;
        public override void Start()
        {
            ScanForM2Emulator();
        }
        public override void Stop()
        {
            
        }


        public void ScanForM2Emulator()
        {
            if (M2EmulatorProcess != null)
                M2EmulatorProcess.CloseProcess();
            M2EmulatorProcess = new ProcessManipulation();

            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            list.Add(new Tuple<string, string>("m2emulator", ""));
            var procs = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(list);
            M2EmulatorProcess.OpenProcess(ProcessManipulation.ProcessAccess.PROCESS_VM_WRITE, procs[0]);
            /*
            // Daytona USA (Saturn Ads)
            DWORD address = 0x0057285B; //v1.1 M2Emu TXaddressVR =0x005AA888;
            DWORD addressVR =0x005AA888;

            */
        }



        public byte GetDriveData()
        {
            M2EmulatorProcess.ReadByte(DriveAddr, out byte value);
            this.DriveValue = value;
            return value;
        }

        public byte GetLampsData()
        {
            M2EmulatorProcess.ReadByte(LampAddr, out byte value);
            this.LampsValue = value;
            return value;
        }


    }
}
