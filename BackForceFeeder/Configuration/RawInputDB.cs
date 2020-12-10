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

    public enum KeyStrokes : uint
    {
        No = 0,

        ESC,
        AltF4,
        ENTER,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10

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
