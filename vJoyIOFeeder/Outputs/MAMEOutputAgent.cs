using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// MAME and Supermodel output agent.
    /// Use Windows messaging system
    /// Code converted from M2DUMP, M13DUMP, DaytonaUSB
    /// </summary>
    public class MAMEOutputAgent : IOutput
    {
        MAMEOutputWindow FormToGetMessages;

        public MAMEOutputAgent()
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
            ManagerThread.Name = "vJoy Output";
            ManagerThread.Priority = ThreadPriority.AboveNormal;
            ManagerThread.Start();
        }

        public override void Stop()
        {
            FormToGetMessages.Close();

            Running = false;
            if (ManagerThread == null)
                return;
            Thread.Sleep(100);
            ManagerThread.Join();
            ManagerThread = null;

        }

        protected void ManagerThreadMethod()
        {
            FormToGetMessages = new MAMEOutputWindow();
            FormToGetMessages.Show();
            FormToGetMessages.RegisterMAME();


            Application.Run(FormToGetMessages);

            Logger.Log("[MAMEOutput] Windows terminated", LogLevels.INFORMATIVE);
        }
    }


    /// <summary>
    /// Fake form to send/receive Windows messages
    /// Valid for MAME and Supermodel
    /// </summary>
    public class MAMEOutputWindow : Form
    {
        public MAMEOutputWindow() :
            base()
        {
        }


        public int DriveValue { get; protected set; }
        public int LampsValue { get; protected set; }

        protected static void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[MAMEOutput] " + text, level);
        }

        protected static void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[MAMEOutput] " + text, args);
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        string OUTPUT_WINDOW_CLASS = "MAMEOutput";
        string OUTPUT_WINDOW_NAME = "MAMEOutput";

        //
        // These messages are sent by MAME:
        //

        // OM_MAME_START: broadcast when MAME initializes
        //      WPARAM = HWND of MAME's output window
        //      LPARAM = unused
        string OM_MAME_START = "MAMEOutputStart";

        // OM_MAME_STOP: broadcast when MAME shuts down
        //      WPARAM = HWND of MAME's output window
        //      LPARAM = unused
        string OM_MAME_STOP = "MAMEOutputStop";

        // OM_MAME_UPDATE_STATE: sent to registered clients when the state
        // of an output changes
        //      WPARAM = ID of the output
        //      LPARAM = new value for the output
        string OM_MAME_UPDATE_STATE = "MAMEOutputUpdateState";


        //
        // These messages are sent by external clients to MAME:
        //

        // OM_MAME_REGISTER_CLIENT: sent to MAME to register a client
        //      WPARAM = HWND of client's listener window
        //      LPARAM = client-specified ID (must be unique;
        string OM_MAME_REGISTER_CLIENT = "MAMEOutputRegister";

        // OM_MAME_UNREGISTER_CLIENT: sent to MAME to unregister a client
        //      WPARAM = HWND of client's listener window
        //      LPARAM = client-specified ID (must match registration;
        string OM_MAME_UNREGISTER_CLIENT = "MAMEOutputUnregister";

        // OM_MAME_GET_ID_STRING: requests the string associated with a
        // given ID. ID=0 is always the name of the game. Other IDs are
        // only discoverable from a OM_MAME_UPDATE_STATE message. The
        // result will be sent back as a WM_COPYDATA message with MAME's
        // output window as the sender, dwData = the ID of the string,
        // and lpData pointing to a NULL-terminated string.
        //      WPARAM = HWND of client's listener window
        //      LPARAM = ID you wish to know about
        string OM_MAME_GET_ID_STRING = "MAMEOutputGetIDString";

        static IntPtr mame_target;
        static IntPtr listener_hwnd;
        static uint om_mame_start;
        static uint om_mame_stop;
        static uint om_mame_update_state;
        static uint om_mame_register_client;
        static uint om_mame_unregister_client;
        static uint om_mame_get_id_string;
        static uint CLIENT_ID = (('D' << 24) | ('U' << 16) | ('M' << 8) | ('P' << 0));


        public void RegisterMAME()
        {
            om_mame_start = RegisterWindowMessage(OM_MAME_START);
            if (om_mame_start == 0)
                throw new Exception("error");
            om_mame_stop = RegisterWindowMessage(OM_MAME_STOP);
            if (om_mame_stop == 0)
                throw new Exception("error");
            om_mame_update_state = RegisterWindowMessage(OM_MAME_UPDATE_STATE);
            if (om_mame_update_state == 0)
                throw new Exception("error");
            om_mame_register_client = RegisterWindowMessage(OM_MAME_REGISTER_CLIENT);
            if (om_mame_register_client == 0)
                throw new Exception("error");
            om_mame_unregister_client = RegisterWindowMessage(OM_MAME_UNREGISTER_CLIENT);
            if (om_mame_unregister_client == 0)
                throw new Exception("error");
            om_mame_get_id_string = RegisterWindowMessage(OM_MAME_GET_ID_STRING);
            if (om_mame_get_id_string == 0)
                throw new Exception("error");

            listener_hwnd = Process.GetCurrentProcess().MainWindowHandle;
            // see if MAME or supermodel is already running
            var otherwnd = FindWindow(OUTPUT_WINDOW_CLASS, OUTPUT_WINDOW_NAME);
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
                handle_mame_start(msg);
            }
        }
        Dictionary<IntPtr, string> MapID = new Dictionary<IntPtr, string>();
        void reset_id_to_outname_cache()
        {
            MapID.Clear();
        }
        string map_id_to_outname(IntPtr id)
        {
            // see if we have an entry in our map
            if (MapID.ContainsKey(id)) {
                return MapID[id];
            }
            // no entry yet; we have to ask
            SendMessage(mame_target, (int)om_mame_get_id_string, listener_hwnd, id);

            // now see if we have the entry in our map
            if (MapID.ContainsKey(id)) {
                return MapID[id];
            }
            // if not, use an empty string
            return "";
        }
        int handle_mame_start(Message msg)
        {
            mame_target = msg.WParam;

            reset_id_to_outname_cache();

            // register ourselves as a client
            PostMessage(mame_target, om_mame_register_client, listener_hwnd, (IntPtr)CLIENT_ID);

            // get the game name
            map_id_to_outname(IntPtr.Zero);

            return 0;
        }
        int handle_mame_stop(Message msg)
        {
            // ignore if this is not the instance we care about
            if (mame_target != msg.WParam)
                return 1;

            // clear our target out
            mame_target = IntPtr.Zero;
            return 0;
        }


        int handle_update_state(Message msg)
        {
            output_set_state(map_id_to_outname(msg.WParam), (int)msg.LParam);
            return 0;
        }

        void output_set_state(string outname, Int32 state)
        {
            var log = outname + "=" + state.ToString("X");
            Log(log);
            Console.WriteLine(log);
            switch (outname) {
                // ================================================================
                // SuperModel3 Section
                // RawDrive: FFB
                // RawLamps: Lamps
                // ================================================================

                case "RawDrive":
                    DriveValue = state;
                    break;
                case "RawLamps":
                    LampsValue = state;
                    break;
                // ================================================================
                // MAME Section
                // digit0: FFB
                // digit1: Lamps
                // ================================================================

                case "digit0":
                    DriveValue = state;
                    break;
                case "digit1":
                    LampsValue = state;
                    break;
                case "cpuled1":
                    break;
            }
        }
        const uint WM_COPYDATA = 0x004A;
        [StructLayout(LayoutKind.Sequential)]
        struct COPYDATASTRUCT
        {
            public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
            public int cbData;       // The count of bytes in the message.
            public IntPtr lpData;    // The address of the message.
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct copydata_id_string
        {
            public UInt32 id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string name;
        }

        public string GameName = "";
        int handle_copydata(Message msg)
        {
            var m = Marshal.PtrToStructure<COPYDATASTRUCT>(msg.LParam);
            var entry = Marshal.PtrToStructure<copydata_id_string>(m.lpData);
            MapID.Add((IntPtr)entry.id, entry.name);
            if (entry.id == 0) {
                GameName = entry.name;
                Log("Game detected: " + GameName, LogLevels.INFORMATIVE);
                Console.WriteLine(" Drive Board    Lamps    Coin1 Coin2 Start Red  Blue Yellow Green Leader");
                Console.WriteLine(" -----------    -----    -----------------------------------------------");
            }
            return 0;
        }

        protected override void WndProc(ref Message msg)
        {
            var message = msg.Msg;
            // OM_MAME_START: register ourselves with the new MAME (first instance only)
            if (message == om_mame_start)
                handle_mame_start(msg);

            // OM_MAME_STOP: no need to unregister, just note that we've stopped caring and reset the LEDs
            else if (message == om_mame_stop)
                handle_mame_stop(msg);

            // OM_MAME_UPDATE_STATE: update the state of this item if we care
            else if (message == om_mame_update_state) {
                handle_update_state(msg);
            }

            // WM_COPYDATA: extract the string and create an ID map entry
            else if (message == WM_COPYDATA)
                handle_copydata(msg);

            // everything else is default
            else
                base.WndProc(ref msg);
        }
    }

}
