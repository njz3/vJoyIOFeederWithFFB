using BackForceFeeder.Managers;
using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace BackForceFeeder.Utils
{
    public class OSUtilities
    {
        #region Assembly, version
        public static Version AssemblyVersion()
        {
            var assembly = typeof(BFFManager).Assembly;
            var version = assembly.GetName().Version;
            return version;
        }

        public static string AssemblyCopyright()
        {
            AssemblyCopyrightAttribute copyright =
                Assembly.GetExecutingAssembly().
                GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]
                as AssemblyCopyrightAttribute;
            return copyright.Copyright;
        }

        public static string AboutString()
        {
            var copyright = OSUtilities.AssemblyCopyright();
            var version = OSUtilities.AssemblyVersion();
            string text = "BackForceFeeder for Gamoover by B. Maurin (njz3)\n";
            text += copyright;
            text += "\nVersion " + version.ToString();
            text += "\nRunning mode is " + BFFManager.Config.Hardware.TranslatingModes.ToString();
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appname">Name of application, like "app". .lnk and .exe will be added automatically</param>
        /// <param name="description"></param>
        public static void CreateStartupShortcut(string appname, string description)
        {
            var shell = new WshShell();
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortCutLinkFilePath = Path.Combine(startupFolderPath, appname + ".lnk");
            var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);
            windowsApplicationShortcut.Description = description;
            windowsApplicationShortcut.WorkingDirectory = Application.StartupPath;
            windowsApplicationShortcut.TargetPath = Application.ExecutablePath;
            windowsApplicationShortcut.IconLocation = appname + ".exe, 0";
            windowsApplicationShortcut.Save();
        }

        public static void DeleteStartupShortcut(string appname)
        {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortCutLinkFilePath = Path.Combine(startupFolderPath, appname + ".lnk");

            // Remove shortcut
            if (System.IO.File.Exists(shortCutLinkFilePath)) {
                System.IO.File.Delete(shortCutLinkFilePath);
            }
        }
        #endregion

        #region Console input
        /// <summary>
        /// Determines whether the specified key is pressed in a console window.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>
        ///   <c>true</c> if the specified key is pressed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKeyPressed(ConsoleKey key)
        {
            return Console.KeyAvailable && Console.ReadKey(true).Key == key;
        }
        #endregion

        #region SendInput wrapper through Windows Input Simulator
        /// <summary>
        /// https://github.com/TChatzigiannakis/InputSimulatorPlus
        /// https://stackoverflow.com/questions/25987445/installed-inputsimulator-via-nuget-no-members-accessible
        /// </summary>
        static InputSimulator Simulator = new InputSimulator();

        public static void SendModifiedKeyStroke(VirtualKeyCode modifier, VirtualKeyCode keyCode)
        {
            Simulator.Keyboard.ModifiedKeyStroke(modifier, keyCode);
        }
        public static void SendAltF4()
        {
            SendModifiedKeyStroke(VirtualKeyCode.LMENU, VirtualKeyCode.F4);
        }

        /// <summary>
        /// simulate key press
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyPress(VirtualKeyCode keyCode)
        {
            Simulator.Keyboard.KeyPress(keyCode);
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(VirtualKeyCode keyCode)
        {
            Simulator.Keyboard.KeyDown(keyCode);
        }

        /// <summary>
        /// Release a key that is being hold down
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(VirtualKeyCode keyCode)
        {
            Simulator.Keyboard.KeyUp(keyCode);
        }
        #endregion

        #region Windows Messaging system

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
        #endregion

        #region DirectInput emulation with SendInput
        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        /****************************************************************************
         *
         *      DirectInput keyboard scan codes
         *
         ****************************************************************************/
        public enum DInputScanCodes : ushort
        {
            DIK_ESCAPE     =     0x01,
            DIK_1               = 0x02,
            DIK_2               = 0x03,
            DIK_3               = 0x04,
            DIK_4               = 0x05,
            DIK_5               = 0x06,
            DIK_6               = 0x07,
            DIK_7               = 0x08,
            DIK_8               = 0x09,
            DIK_9               = 0x0A,
            DIK_0               = 0x0B,
            DIK_MINUS           = 0x0C,    /* - on main keyboard */
            DIK_EQUALS          = 0x0D,
            DIK_BACK            = 0x0E,    /* backspace */
            DIK_TAB             = 0x0F,
            DIK_Q               = 0x10,
            DIK_W               = 0x11,
            DIK_E               = 0x12,
            DIK_R               = 0x13,
            DIK_T               = 0x14,
            DIK_Y               = 0x15,
            DIK_U               = 0x16,
            DIK_I               = 0x17,
            DIK_O               = 0x18,
            DIK_P               = 0x19,
            DIK_LBRACKET        = 0x1A,
            DIK_RBRACKET        = 0x1B,
            DIK_RETURN          = 0x1C,    /* Enter on main keyboard */
            DIK_LCONTROL        = 0x1D,
            DIK_A               = 0x1E,
            DIK_S               = 0x1F,
            DIK_D               = 0x20,
            DIK_F               = 0x21,
            DIK_G               = 0x22,
            DIK_H               = 0x23,
            DIK_J               = 0x24,
            DIK_K               = 0x25,
            DIK_L               = 0x26,
            DIK_SEMICOLON       = 0x27,
            DIK_APOSTROPHE      = 0x28,
            DIK_GRAVE           = 0x29,   /* accent grave */
            DIK_LSHIFT          = 0x2A,
            DIK_BACKSLASH       = 0x2B,
            DIK_Z               = 0x2C,
            DIK_X               = 0x2D,
            DIK_C               = 0x2E,
            DIK_V               = 0x2F,
            DIK_B               = 0x30,
            DIK_N               = 0x31,
            DIK_M               = 0x32,
            DIK_COMMA           = 0x33,
            DIK_PERIOD          = 0x34,    /* . on main keyboard */
            DIK_SLASH           = 0x35,    /* / on main keyboard */
            DIK_RSHIFT          = 0x36,
            DIK_MULTIPLY        = 0x37,    /* * on numeric keypad */
            DIK_LMENU           = 0x38,    /* left Alt */
            DIK_SPACE           = 0x39,
            DIK_CAPITAL         = 0x3A,
            DIK_F1              = 0x3B,
            DIK_F2              = 0x3C,
            DIK_F3              = 0x3D,
            DIK_F4              = 0x3E,
            DIK_F5              = 0x3F,
            DIK_F6              = 0x40,
            DIK_F7              = 0x41,
            DIK_F8              = 0x42,
            DIK_F9              = 0x43,
            DIK_F10             = 0x44,
            DIK_NUMLOCK         = 0x45,
            DIK_SCROLL          = 0x46,    /* Scroll Lock */
            DIK_NUMPAD7         = 0x47,
            DIK_NUMPAD8         = 0x48,
            DIK_NUMPAD9         = 0x49,
            DIK_SUBTRACT        = 0x4A,    /* - on numeric keypad */
            DIK_NUMPAD4         = 0x4B,
            DIK_NUMPAD5         = 0x4C,
            DIK_NUMPAD6         = 0x4D,
            DIK_ADD             = 0x4E,    /* + on numeric keypad */
            DIK_NUMPAD1         = 0x4F,
            DIK_NUMPAD2         = 0x50,
            DIK_NUMPAD3         = 0x51,
            DIK_NUMPAD0         = 0x52,
            DIK_DECIMAL         = 0x53,    /* . on numeric keypad */
            DIK_OEM_102         = 0x56,    /* <> or \| on RT 102-key keyboard (Non-U.S.) */
            DIK_F11             = 0x57,
            DIK_F12             = 0x58,
            DIK_F13             = 0x64,    /*                     (NEC PC98) */
            DIK_F14             = 0x65,    /*                     (NEC PC98) */
            DIK_F15             = 0x66,    /*                     (NEC PC98) */
            DIK_KANA            = 0x70,    /* (Japanese keyboard)            */
            DIK_ABNT_C1         = 0x73,    /* /? on Brazilian keyboard */
            DIK_CONVERT         = 0x79,    /* (Japanese keyboard)            */
            DIK_NOCONVERT       = 0x7B,    /* (Japanese keyboard)            */
            DIK_YEN             = 0x7D,    /* (Japanese keyboard)            */
            DIK_ABNT_C2         = 0x7E,    /* Numpad . on Brazilian keyboard */
            DIK_NUMPADEQUALS    = 0x8D,    /* = on numeric keypad (NEC PC98) */
            DIK_PREVTRACK       = 0x90,    /* Previous Track (DIK_CIRCUMFLEX on Japanese keyboard) */
            DIK_AT              = 0x91,    /*                     (NEC PC98) */
            DIK_COLON           = 0x92,    /*                     (NEC PC98) */
            DIK_UNDERLINE       = 0x93,    /*                     (NEC PC98) */
            DIK_KANJI           = 0x94,    /* (Japanese keyboard)            */
            DIK_STOP            = 0x95,    /*                     (NEC PC98) */
            DIK_AX              = 0x96,    /*                     (Japan AX) */
            DIK_UNLABELED       = 0x97,    /*                        (J3100) */
            DIK_NEXTTRACK       = 0x99,    /* Next Track */
            DIK_NUMPADENTER     = 0x9C,    /* Enter on numeric keypad */
            DIK_RCONTROL        = 0x9D,
            DIK_MUTE            = 0xA0,    /* Mute */
            DIK_CALCULATOR      = 0xA1,    /* Calculator */
            DIK_PLAYPAUSE       = 0xA2,    /* Play / Pause */
            DIK_MEDIASTOP       = 0xA4,    /* Media Stop */
            DIK_VOLUMEDOWN      = 0xAE,    /* Volume - */
            DIK_VOLUMEUP        = 0xB0,    /* Volume + */
            DIK_WEBHOME         = 0xB2,    /* Web home */
            DIK_NUMPADCOMMA     = 0xB3,    /* , on numeric keypad (NEC PC98) */
            DIK_DIVIDE          = 0xB5,    /* / on numeric keypad */
            DIK_SYSRQ           = 0xB7,
            DIK_RMENU           = 0xB8,    /* right Alt */
            DIK_PAUSE           = 0xC5,    /* Pause */
            DIK_HOME            = 0xC7,    /* Home on arrow keypad */
            DIK_UP              = 0xC8,    /* UpArrow on arrow keypad */
            DIK_PRIOR           = 0xC9,    /* PgUp on arrow keypad */
            DIK_LEFT            = 0xCB,    /* LeftArrow on arrow keypad */
            DIK_RIGHT           = 0xCD,    /* RightArrow on arrow keypad */
            DIK_END             = 0xCF,    /* End on arrow keypad */
            DIK_DOWN            = 0xD0,    /* DownArrow on arrow keypad */
            DIK_NEXT            = 0xD1,    /* PgDn on arrow keypad */
            DIK_INSERT          = 0xD2,    /* Insert on arrow keypad */
            DIK_DELETE          = 0xD3,    /* Delete on arrow keypad */
            DIK_LWIN            = 0xDB,    /* Left Windows key */
            DIK_RWIN            = 0xDC,    /* Right Windows key */
            DIK_APPS            = 0xDD,    /* AppMenu key */
            DIK_POWER           = 0xDE,    /* System Power */
            DIK_SLEEP           = 0xDF,    /* System Sleep */
            DIK_WAKE            = 0xE3,    /* System Wake */
            DIK_WEBSEARCH       = 0xE5,    /* Web Search */
            DIK_WEBFAVORITES    = 0xE6,    /* Web Favorites */
            DIK_WEBREFRESH      = 0xE7,    /* Web Refresh */
            DIK_WEBSTOP         = 0xE8,    /* Web Stop */
            DIK_WEBFORWARD      = 0xE9,    /* Web Forward */
            DIK_WEBBACK         = 0xEA,    /* Web Back */
            DIK_MYCOMPUTER      = 0xEB,    /* My Computer */
            DIK_MAIL            = 0xEC,    /* Mail */
            DIK_MEDIASELECT     = 0xED,    /* Media Select */
        }

        static void SendDInputKeypress(DInputScanCodes scankey)
        {
            INPUT[] keyPress = new INPUT[]            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (ushort)scankey,
                            dwFlags = KEYEVENTF_SCANCODE,
                            dwExtraInfo = IntPtr.Zero,
                        }
                    }
                },
            };

            // Key DOWN
            if (SendInput((uint)keyPress.Length, keyPress, Marshal.SizeOf(typeof(INPUT))) == 0) {
                Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString());
            }
            Thread.Sleep(50); // MAME Emulator requires this

            // key UP
            keyPress[0].u.ki.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_SCANCODE;
            if (SendInput((uint)keyPress.Length, keyPress, Marshal.SizeOf(typeof(INPUT))) == 0) {
                Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString());
            }
            Thread.Sleep(50); // MAME Emulator requires this
        }

        /// <summary>
        /// Scankey:
        ///  https://gist.github.com/tracend/912308
        /// </summary>
        /// <param name="scanKey"></param>
        public static void SendKeybDInputDown(DInputScanCodes scankey)
        {
            INPUT[] keyPress = new INPUT[]            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (ushort)scankey,
                            dwFlags = KEYEVENTF_SCANCODE,
                            dwExtraInfo = IntPtr.Zero,
                        }
                    }
                },
            };

            // Key DOWN
            if (SendInput((uint)keyPress.Length, keyPress, Marshal.SizeOf(typeof(INPUT))) == 0) {
                Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString());
            }
        }

        /// <summary>
        /// Scankey:
        ///  https://gist.github.com/tracend/912308
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeybDInputUp(DInputScanCodes scankey)
        {
            INPUT[] keyPress = new INPUT[]            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (ushort)scankey,
                            dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_SCANCODE,
                            dwExtraInfo = IntPtr.Zero,
                        }
                    }
                },
            };
            if (SendInput((uint)keyPress.Length, keyPress, Marshal.SizeOf(typeof(INPUT))) == 0) {
                Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString());
            }
        }
        #endregion
    }
}
