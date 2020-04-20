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

            List<Tuple<string, string>> namesAndTitle = new List<Tuple<string, string>>();

            var cs = vJoyManager.Config.CurrentControlSet;
            namesAndTitle.Add(new Tuple<string, string>(cs.ProcessDescriptor.ProcessName, cs.ProcessDescriptor.MainWindowTitle));

            // Scan processes and main windows title
            var found = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(namesAndTitle, true, false);
            // Store detected profile
            if (found.Count==0) {
                return false;
            }
            // Pick first
            var proc = found[0].Item1;
            string gamename = proc.MainWindowTitle;
            Log("Found " + proc.ProcessName + " main window " + gamename + " matched with current control set " + cs.UniqueName, LogLevels.IMPORTANT);

            // Known game?
            if (!DetectGameFromMainWindowTitle(gamename)) {
                return false;
            }

            GameProcess.OpenProcess(ProcessManipulation.ProcessAccess.PROCESS_WM_READ, proc);

            FillAddressFromGame();

            return true;
        }

        ulong address = 0;
        ulong addressVR = 0;
        bool DetectGameFromMainWindowTitle(string name)
        {
            address = addressVR = 0;
            switch (name) {
                case "Daytona USA (Saturn Ads)":
                    // Daytona USA (Saturn Ads)
                    address = 0x0057285B- 0x400000; // Base address 0x400000
                                                    //v1.1 M2Emu TXaddressVR =0x005AA888;
                    addressVR = 0x005AA888- 0x400000;
                    break;
                case "Indianapolis 500 (Rev A, Twin, Newer rev)":
                case "Over Rev":
                case "Over Rev (Model 2B)":
                case "Sega Rally Championship":
                case "Sega Touring Car Championship":
                case "Super GT 24h":
                    address = 0x0057285B - 0x400000;
                    addressVR = 0x00574CF0 -0x400000;
                    break;
            }


            if (addressVR!=0) {
                this.GameProfile = name;
                return true;
            } else {
                return false;
            }
        }

        bool FillAddressFromGame()
        {
            if (addressVR==0) {
                return false;
            }
            if (this.GameProfile=="Daytona USA (Saturn Ads)") {
                GameProcess.ReadUInt32(addressVR + (ulong)GameProcess.BaseAddress, out var newaddressVR);
                addressVR = newaddressVR + 0x100;
                GameProcess.ReadUInt32(addressVR + (ulong)GameProcess.BaseAddress, out newaddressVR);
                addressVR = newaddressVR + 0x824;  //Offset
            } else {
                address = address + (ulong)GameProcess.BaseAddress;
                addressVR = addressVR + (ulong)GameProcess.BaseAddress;
            }
            this.DriveAddr = address;
            this.LampAddr = addressVR;
            return true;
        }
    }
}
