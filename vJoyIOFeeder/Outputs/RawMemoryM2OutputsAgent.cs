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
    public class RawMemoryM2OutputsAgent : RawMemoryOutputsAgent
    {

        public override bool ScanForKnownGameEmulator()
        {
            if (GameProcess != null)
                GameProcess.CloseProcess();
            GameProcess = new ProcessManipulation();
            var procs = Process.GetProcessesByName("emulator.exe");
            if (procs.Length==0) {
                procs = Process.GetProcessesByName("emulator_multicpu.exe");
                if (procs.Length==0) {
                    return false;
                }
            }

            GameProcess.OpenProcess(ProcessManipulation.ProcessAccess.PROCESS_WM_READ, procs[0]);
            GameProcess.ReadUInt32(0x005AA888, out uint val);

            ulong address = 0x0057285B;
            ulong addressVR =0x00574CF0;

            /*
            // Daytona USA (Saturn Ads)
            DWORD address = 0x0057285B; //v1.1 M2Emu TXaddressVR =0x005AA888;
            DWORD addressVR =0x005AA888;

            */
            return true;
        }


    }
}
