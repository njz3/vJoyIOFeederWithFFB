using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

namespace BackForceFeeder.Configuration
{

    public enum ShifterDecoderMap : uint
    {
        No = 0,

        HShifterLeftRight,
        HShifterUp,
        HShifterDown,

        SequencialUp,
        SequencialDown,
    }
    [Flags]
    public enum KeyEmulationAPI : uint
    {
        SendInput = 1<<0,
        DInput = 1<<1,
        SendInputAndDInput = SendInput | DInput,
    }
    public enum KeyStrokes : uint
    {
        No = 0,

        AltF4,
        ESC,
        ENTER,
        TAB,
        LCTRL,
        RCTRL,
        LSHIFT,
        RSHIFT,
        LALT,
        RALT,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NUM0,
        NUM1,
        NUM2,
        NUM3,
        NUM4,
        NUM5,
        NUM6,
        NUM7,
        NUM8,
        NUM9,
        NUMPAD_0,
        NUMPAD_1,
        NUMPAD_2,
        NUMPAD_3,
        NUMPAD_4,
        NUMPAD_5,
        NUMPAD_6,
        NUMPAD_7,
        NUMPAD_8,
        NUMPAD_9,
        NUMPAD_DECIMAL,

    }

    [Serializable]
    public class RawInputDB :
        ICloneable
    {
        public List<int> MappedvJoyBtns;
        public bool IsInvertedLogic = false;
        public bool IsToggle = false;
        public bool IsAutoFire = false;
        public bool IsSequencedvJoy = false;
        public bool IsNeutralFirstBtn = false;
        public bool IsKeyStroke = false;
        public KeyEmulationAPI KeyAPI = KeyEmulationAPI.DInput;


        /// <summary>
        /// 0: not part of shifter decoder
        /// </summary>
        public ShifterDecoderMap ShifterDecoder = ShifterDecoderMap.No;
        public KeyStrokes KeyStroke = KeyStrokes.No;

        [NonSerialized]
        public int SequenceCurrentToSet = 0;

        public RawInputDB()
        {
            MappedvJoyBtns = new List<int>(1);
        }
        public object Clone()
        {
            var obj = BackForceFeeder.Utils.Files.DeepCopy<RawInputDB>(this);
            return obj;
        }
    }
}
