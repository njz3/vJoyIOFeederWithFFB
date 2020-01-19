using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Utils
{
    /// <summary>
    /// Scan running processes for known executables.
    /// Start/kill processes;.
    /// </summary>
    public class ProcessAnalyzer
    {
        /// <summary>
        /// Search matching process name and title 
        /// </summary>
        /// <param name="namesAndTitle"></param>
        /// <returns></returns>
        public static List<Process> ScanProcessesForKnownNamesAndTitle(List<Tuple<string, string>> namesAndTitle)
        {
            List<Process> processes = new List<Process>();
            foreach (var name in namesAndTitle) {
                foreach (Process proc in Process.GetProcessesByName(name.Item1)) {
                    bool add = false;
                    var windowsTitle = name.Item2;
                    if (windowsTitle != null && windowsTitle != "") {
                        if (proc.MainWindowTitle.Contains(windowsTitle)) {
                            add = true;
                        }
                    } else {
                        add = true;
                    }
                    if (add)
                        processes.Add(proc);
                }
            }
            return processes;
        }
        
        /// <summary>
        /// Start a new process
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public static Process StartProcess(string command = "command.exe", string args = "")
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.UseShellExecute = true;
            process.Start();
            return process;
        }

        public static void KillAllProcesses(string name = "command.exe")
        {
            try {
                foreach (Process proc in Process.GetProcessesByName(name)) {
                    proc.Kill();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

    }


    /// <summary>
    /// Process manipulation for read/write access.
    /// /!\ write access needs admin privileges.
    /// Once found, this allow to read memory of process at known addresses
    /// to get information about which game/option is activated
    /// </summary>
    public class ProcessManipulation
    {
        #region Dll import
        protected const int PROCESS_WM_READ = 0x0010;
        protected const int PROCESS_VM_WRITE = 0x0020;
        protected const int PROCESS_VM_OPERATION = 0x0008;
        protected const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        [DllImport("kernel32.dll")]
        protected static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        protected static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        protected Process Process = null;
        protected IntPtr ProcessHandle;
        #endregion


        /// <summary>
        /// See https://codingvision.net/security/c-read-write-another-process-memory
        /// </summary>
        protected bool OpenProcess(int mode, string name = "", string title = null)
        {
            var nametitle = new Tuple<string, string>(name, title);
            var list = new List<Tuple<string, string>>();
            list.Add(nametitle);
            var found = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(list);
            if (found.Count == 0)
                return false;
            // Open first process with given access mode
            Process = found[0];
            ProcessHandle = OpenProcess(mode, false, this.Process.Id);
            if (ProcessHandle == null)
                return false;
            return true;
        }
        #region Public API
        public bool OpenProcessForRead(string name = "", string title = null)
        {
            return OpenProcess(PROCESS_WM_READ, name, title);
        }

        public bool OpenProcessForReadWrite(string name = "", string title = null)
        {
            return OpenProcess(PROCESS_ALL_ACCESS, name, title);
        }

        
        public bool Read(ulong address, byte[] buffer, int length)
        {
            int bytesRead = 0;
            var stt = ReadProcessMemory((int)ProcessHandle, (int)address, buffer, buffer.Length, ref bytesRead);
            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + bytesRead.ToString() + "bytes)");
            return stt;
        }
        public bool Write(ulong address, byte[] buffer, int length)
        {
            int bytesWritten = 0;
            var stt = WriteProcessMemory((int)ProcessHandle, (int)address, buffer, buffer.Length, ref bytesWritten);
            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + bytesWritten.ToString() + "bytes)");
            return stt;
        }
        #endregion
    }

}
