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
                Logger.Log("KillAllProcesses failed with " + ex.Message, LogLevels.IMPORTANT);
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
        /// <summary>
        /// See https://www.pinvoke.net/default.aspx/kernel32.OpenProcess
        /// </summary>
        [Flags]
        enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        public enum ProcessAccess : uint
        {
            PROCESS_WM_READ = (uint)ProcessAccessFlags.VirtualMemoryRead,
            PROCESS_VM_WRITE = (uint)ProcessAccessFlags.VirtualMemoryWrite,
            PROCESS_VM_OPERATION = (uint)ProcessAccessFlags.VirtualMemoryOperation,
        }

        /// <summary>
        /// Name of the Windows Multimedia library that manage tick period
        /// </summary>
        const string LIBKERNEL32 = "kernel32.dll";

        /// <summary>
        /// https://www.pinvoke.net/default.aspx/kernel32.OpenProcess
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport(LIBKERNEL32)]
        protected static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        /// <summary>
        /// http://www.pinvoke.net/default.aspx/kernel32.CloseHandle
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport(LIBKERNEL32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// https://www.pinvoke.net/default.aspx/kernel32.ReadProcessMemory
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="dwSize"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <returns></returns>
        [DllImport(LIBKERNEL32, SetLastError = true)]
        protected static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        /// <summary>
        /// http://www.pinvoke.net/default.aspx/kernel32/WriteProcessMemory.html
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="dwSize"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <returns></returns>
        [DllImport(LIBKERNEL32, SetLastError = true)]
        protected static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        protected Process Process = null;
        protected IntPtr ProcessHandle;
        #endregion

        #region Public API

        /// <summary>
        /// See https://codingvision.net/security/c-read-write-another-process-memory
        /// </summary>
        public bool OpenProcess(ProcessAccess mode, Process process)
        {
            // Open first process with given access mode
            Process = process;
            ProcessHandle = OpenProcess((uint)mode, false, this.Process.Id);
            if (ProcessHandle == null)
                return false;
            return true;
        }

        public bool OpenProcess(ProcessAccess mode, string name = "", string title = null)
        {
            var nametitle = new Tuple<string, string>(name, title);
            var list = new List<Tuple<string, string>>();
            list.Add(nametitle);
            var found = ProcessAnalyzer.ScanProcessesForKnownNamesAndTitle(list);
            if (found.Count == 0)
                return false;
            return OpenProcess(mode, found[0]);
        }

        public bool OpenProcessForRead(string name = "", string title = null)
        {
            return OpenProcess(ProcessAccess.PROCESS_WM_READ, name, title);
        }


        public bool OpenProcessForReadWrite(string name = "", string title = null)
        {
            return OpenProcess(ProcessAccess.PROCESS_VM_OPERATION, name, title);
        }

        public void CloseProcess()
        {
            CloseHandle(this.ProcessHandle);
        }

        // Generic variants
        public bool Wpm<T>(IntPtr lpBaseAddress, T value) where T : struct
        {
            var buffer = new T[Marshal.SizeOf<T>()];
            buffer[0] = value;
            return WriteProcessMemory(ProcessHandle, lpBaseAddress, buffer, Marshal.SizeOf<T>(), out var bytesread);
        }
        public bool Rpm<T>(IntPtr lpBaseAddress, out T value) where T : struct
        {
            T[] buffer = new T[Marshal.SizeOf<T>()];
            var stt = ReadProcessMemory(ProcessHandle, lpBaseAddress, buffer, Marshal.SizeOf<T>(), out var bytesread);
            value = buffer[0]; // [0] would be faster, but First() is safer. E.g. of buffer[0] ?? default(T)
            return stt;
        }

        // Single type variants
        public bool Read(ulong address, byte[] buffer, int length)
        {
            var stt = ReadProcessMemory(ProcessHandle, (IntPtr)address, buffer, buffer.Length, out var bytesRead);
            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + ((ulong)bytesRead).ToString() + "bytes)");
            return stt;
        }
        public bool Write(ulong address, byte[] buffer, int length)
        {
            var stt = WriteProcessMemory(ProcessHandle, (IntPtr)address, buffer, buffer.Length, out var bytesWritten);
            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + ((ulong)bytesWritten).ToString() + "bytes)");
            return stt;
        }
        public bool ReadByte(ulong address, out byte value)
        {
            return Rpm<byte>((IntPtr)address, out value);
        }
        public bool WriteByte(ulong address, byte value)
        {
            return Wpm<byte>((IntPtr)address, value);
        }
        public bool ReadUInt32(ulong address, out UInt32 value)
        {
            return Rpm<UInt32>((IntPtr)address, out value);
        }
        public bool WriteUInt32(ulong address, UInt32 value)
        {
            return Wpm<UInt32>((IntPtr)address, value);
        }
        #endregion
    }

}
