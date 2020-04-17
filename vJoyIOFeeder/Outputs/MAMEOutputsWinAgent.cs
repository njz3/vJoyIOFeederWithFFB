using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using vJoyIOFeeder.Utils;


namespace vJoyIOFeeder.Outputs
{

    /// <summary>
    ///  Constants for MAME windows output system (MAMEHooker)
    /// </summary>
    public static class WinMsgUtils
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        public static string OUTPUT_WINDOW_CLASS = "MAMEOutput";
        public static string OUTPUT_WINDOW_NAME = "MAMEOutput";

        //
        // These messages are sent by MAME:
        //

        // OM_MAME_START: broadcast when MAME initializes
        //      WPARAM = HWND of MAME's output window
        //      LPARAM = unused
        public static string OM_MAME_START = "MAMEOutputStart";

        // OM_MAME_STOP: broadcast when MAME shuts down
        //      WPARAM = HWND of MAME's output window
        //      LPARAM = unused
        public static string OM_MAME_STOP = "MAMEOutputStop";

        // OM_MAME_UPDATE_STATE: sent to registered clients when the state
        // of an output changes
        //      WPARAM = ID of the output
        //      LPARAM = new value for the output
        public static string OM_MAME_UPDATE_STATE = "MAMEOutputUpdateState";


        //
        // These messages are sent by external clients to MAME:
        //

        // OM_MAME_REGISTER_CLIENT: sent to MAME to register a client
        //      WPARAM = HWND of client's listener window
        //      LPARAM = client-specified ID (must be unique;
        public static string OM_MAME_REGISTER_CLIENT = "MAMEOutputRegister";

        // OM_MAME_UNREGISTER_CLIENT: sent to MAME to unregister a client
        //      WPARAM = HWND of client's listener window
        //      LPARAM = client-specified ID (must match registration;
        public static string OM_MAME_UNREGISTER_CLIENT = "MAMEOutputUnregister";

        // OM_MAME_GET_ID_STRING: requests the string associated with a
        // given ID. ID=0 is always the name of the game. Other IDs are
        // only discoverable from a OM_MAME_UPDATE_STATE message. The
        // result will be sent back as a WM_COPYDATA message with MAME's
        // output window as the sender, dwData = the ID of the string,
        // and lpData pointing to a NULL-terminated string.
        //      WPARAM = HWND of client's listener window
        //      LPARAM = ID you wish to know about
        public static string OM_MAME_GET_ID_STRING = "MAMEOutputGetIDString";

        public static uint CLIENT_ID = (('D' << 24) | ('U' << 16) | ('M' << 8) | ('P' << 0));


        public const int WM_COPYDATA = 0x004A;
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
            public int cbData;       // The count of bytes in the message.
            public IntPtr lpData;    // The address of the message.
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct COPYDATAIDSTRING
        {
            public UInt32 id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string name;
        }

    }



    /// <summary>
    /// MAME and Supermodel output agent.
    /// Use Windows messaging system
    /// Code converted from M2DUMP, M13DUMP, DaytonaUSB
    /// </summary>
    public class MAMEOutputsWinAgent : MAMEOutputsAgent
    {
        static HiddenMAMEOutputWindow FormToGetMessages;

        public MAMEOutputsWinAgent() :
            base()
        {
        }

        public override void Stop()
        {
            if (!Running) return;

            // Leave current Application.Run() loop on the underlying thread
            if (FormToGetMessages!=null)
                FormToGetMessages.Invoke(new Action(() => {
                    Application.ExitThread();
                }));

            base.Stop();
        }
        public void OutputSetState(string outname, Int32 state)
        {
            var log = outname + "=" + state.ToString("X");
            Log(log);
            Console.WriteLine(log);

            this.ProcessMessage(outname + "=" + state);

            switch (outname) {
                // ================================================================
                // SuperModel3 Section
                // RawDrive: FFB
                // RawLamps: Lamps
                // ================================================================

                case "RawDrive":
                    this.DriveValue = state;
                    break;
                case "RawLamps":
                    this.LampsValue = state;
                    break;

                // ================================================================
                // MAME Section
                // digit0: FFB
                // digit1: Lamps
                // ================================================================

                case "digit0":
                    this.DriveValue = state;
                    break;
                case "digit1":
                    this.LampsValue = state;
                    break;
                case "cpuled1":
                    break;
            }
        }
        

        protected override void ManagerThreadMethod()
        {
            FormToGetMessages = new HiddenMAMEOutputWindow(this);
            FormToGetMessages.RegisterMAMEMessages();
            // Enter message pump, until Application.ExitThread() will be called on same execution context
            Application.Run();

            Log("Windows terminated", LogLevels.INFORMATIVE);
        }


        /// <summary>
        /// Fake form to send/receive Windows messages
        /// Valid for MAME and Supermodel
        /// </summary>
        public class HiddenMAMEOutputWindow : Form
        {
            protected MAMEOutputsWinAgent Agent;

            public HiddenMAMEOutputWindow(MAMEOutputsWinAgent agent) :
                base()
            {
                this.Agent = agent;
                this.Name = "MAME Output Receiver";
            }

            protected void Log(string text, LogLevels level = LogLevels.DEBUG)
            {
                Logger.Log("[MAMEWinOutput] " + text, level);
            }

            protected void LogFormat(LogLevels level, string text, params object[] args)
            {
                Logger.LogFormat(level, "[MAMEWinOutput] " + text, args);
            }

            void SetOutputState(string outname, Int32 state)
            {
                var log = outname + "=" + state.ToString("X");
                Log(log);
                Console.WriteLine(log);

                Agent.ProcessMessage(outname + "=" + state);

                switch (outname) {
                    // ================================================================
                    // SuperModel3 Section
                    // RawDrive: FFB
                    // RawLamps: Lamps
                    // ================================================================

                    case "RawDrive":
                        Agent.DriveValue = state;
                        break;
                    case "RawLamps":
                        Agent.LampsValue = state;
                        break;

                    // ================================================================
                    // MAME Section
                    // digit0: FFB
                    // digit1: Lamps
                    // ================================================================

                    case "digit0":
                        Agent.DriveValue = state;
                        break;
                    case "digit1":
                        Agent.LampsValue = state;
                        break;
                    case "cpuled1":
                        break;
                }
            }

            void SetGameInfo(string gamename)
            {
                Log("Game detected: " + gamename, LogLevels.INFORMATIVE);
                Agent.GameProfile = gamename;
                Console.WriteLine(" Game detected:" + gamename);
                Console.WriteLine(" Drive Board    Lamps    Coin1 Coin2 Start Red  Blue Yellow Green Leader");
                Console.WriteLine(" -----------    -----    -----------------------------------------------");
            }

            #region MAME Output message mechanism

            IntPtr HandleToMAMEWindow;
            IntPtr HandleOfListener;

            int OM_MAME_START;
            int OM_MAME_STOP;
            int OM_MAME_UPDATE_START;
            int OM_MAME_REGISTER_CLIENT;
            int OM_MAME_UNREGISTER_CLIENT;
            int OM_MAME_GET_ID_STRING;


            public void RegisterMAMEMessages()
            {
                // Retrieve our handle (needed when registering to MAME)
                HandleOfListener = this.Handle;

                // Register MAME messages
                OM_MAME_START = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_START);
                if (OM_MAME_START == 0)
                    throw new Exception("error");
                OM_MAME_STOP = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_STOP);
                if (OM_MAME_STOP == 0)
                    throw new Exception("error");
                OM_MAME_UPDATE_START = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_UPDATE_STATE);
                if (OM_MAME_UPDATE_START == 0)
                    throw new Exception("error");
                OM_MAME_REGISTER_CLIENT = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_REGISTER_CLIENT);
                if (OM_MAME_REGISTER_CLIENT == 0)
                    throw new Exception("error");
                OM_MAME_UNREGISTER_CLIENT = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_UNREGISTER_CLIENT);
                if (OM_MAME_UNREGISTER_CLIENT == 0)
                    throw new Exception("error");
                OM_MAME_GET_ID_STRING = WinMsgUtils.RegisterWindowMessage(WinMsgUtils.OM_MAME_GET_ID_STRING);
                if (OM_MAME_GET_ID_STRING == 0)
                    throw new Exception("error");


                // see if MAME or supermodel is already running
                var otherwnd = WinMsgUtils.FindWindow(WinMsgUtils.OUTPUT_WINDOW_CLASS, WinMsgUtils.OUTPUT_WINDOW_NAME);
                // If not found, try to found supermodel by process names
                if (otherwnd == IntPtr.Zero) {
                    var procs = Process.GetProcessesByName("supermodel");
                    if (procs.Length>0) {
                        procs[0].Refresh();
                        otherwnd = procs[0].MainWindowHandle;
                        //otherwnd = FindWindowEx(otherwnd, IntPtr.Zero, OUTPUT_WINDOW_NAME, IntPtr.Zero);
                    }
                }
                if (otherwnd != IntPtr.Zero) {
                    Message msg = new Message();
                    msg.WParam = otherwnd;
                    msg.LParam = IntPtr.Zero;
                    HandleMAMEStart(msg);
                }
            }

            Dictionary<IntPtr, string> MapIDToName = new Dictionary<IntPtr, string>();

            void Reset_id_to_outname_cache()
            {
                MapIDToName.Clear();
            }
            string Map_id_to_outname(IntPtr id)
            {
                // see if we have an entry in our map
                if (MapIDToName.ContainsKey(id)) {
                    return MapIDToName[id];
                }
                // no entry yet; we have to ask
                WinMsgUtils.SendMessage(HandleToMAMEWindow, OM_MAME_GET_ID_STRING, HandleOfListener, id);

                // now see if we have the entry in our map
                if (MapIDToName.ContainsKey(id)) {
                    return MapIDToName[id];
                }
                // if not, use an empty string
                return "";
            }


            int HandleMAMEStart(Message msg)
            {
                HandleToMAMEWindow = msg.WParam;

                Reset_id_to_outname_cache();

                // register ourselves as a client
                WinMsgUtils.PostMessage(HandleToMAMEWindow, OM_MAME_REGISTER_CLIENT, HandleOfListener, (IntPtr)WinMsgUtils.CLIENT_ID);
                //WinMsgUtils.SendMessage(mame_target, om_mame_register_client, listener_hwnd, (IntPtr)WinMsgUtils.CLIENT_ID);

                // get the game name
                Map_id_to_outname(IntPtr.Zero);

                return 0;
            }
            int HandleMAMEStop(Message msg)
            {
                // ignore if this is not the instance we care about
                if (HandleToMAMEWindow != msg.WParam)
                    return 1;

                // clear our target out
                HandleToMAMEWindow = IntPtr.Zero;
                return 0;
            }

            int HandleUpdateState(Message msg)
            {
                SetOutputState(Map_id_to_outname(msg.WParam), (int)msg.LParam);
                return 0;
            }

            
            int HandleCopydata(Message msg)
            {
                var m = Marshal.PtrToStructure<WinMsgUtils.COPYDATASTRUCT>(msg.LParam);
                var entry = Marshal.PtrToStructure<WinMsgUtils.COPYDATAIDSTRING>(m.lpData);

                MapIDToName.Add((IntPtr)entry.id, entry.name);
                if (entry.id == 0) {
                    SetGameInfo(entry.name);
                }
                return 0;
            }


            protected override void WndProc(ref Message msg)
            {
                var message = msg.Msg;
                // OM_MAME_START: register ourselves with the new MAME (first instance only)
                if (message == OM_MAME_START) {
                    HandleMAMEStart(msg);
                    
                }
                // OM_MAME_STOP: no need to unregister, just note that we've stopped caring and reset the LEDs
                else if (message == OM_MAME_STOP) {
                HandleMAMEStop(msg);

                }
                // OM_MAME_UPDATE_STATE: update the state of this item if we care
                else if (message == OM_MAME_UPDATE_START) {
                HandleUpdateState(msg);
                }
                // WM_COPYDATA: extract the string and create an ID map entry
                else if (message == WinMsgUtils.WM_COPYDATA) {
                    HandleCopydata(msg);
                }
                // everything else is default
                else {
                    base.WndProc(ref msg);
                }
            }
            #endregion
        }

    }

}
