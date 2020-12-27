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
    [Flags]
    public enum KeyEmulationAPI : uint
    {
        DInput = 1<<0,
        SendInput = 1<<1,
        DInputAndSendInput = SendInput | DInput,
    }
    /// <summary>
    /// Key codes
    /// </summary>
    public enum KeyCodes : uint
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
    /// <summary>
    /// Where information will be read
    /// </summary>
    public enum KeySourceTypes : uint
    {
        UNDEF = 0,
        RAW_INPUT,
        RAW_AXIS,
        VJOY_BUTTON,
        VJOY_AXIS,
    }
    /// <summary>
    /// Operators to help detecting combined buttons
    /// </summary>
    public enum KeysOperators : uint
    {
        NO = 0,
        AND,
        OR,
        NAND,
        NOR,
    }
    /// <summary>
    /// Source of a keystroke trigger
    /// </summary>
    [Serializable]
    public class KeySource
    {
        /// <summary>
        /// Source to trigger an event
        /// </summary>
        public KeySourceTypes Type;
        /// <summary>
        /// Base 1
        /// </summary>
        public int Index = 1;
        /// <summary>
        /// Threshold value for axis
        /// </summary>
        public double Threshold = 0.75;
        /// <summary>
        /// Sign of the edge detection for an axis
        /// </summary>
        public bool Sign;

        static char[] SourceSplitter = new char[] { ':' };
        /// <summary>
        /// Combine KeySourceTypes + number of input or axis as:
        /// - RAW_INPUT:1 = first raw digital input
        /// - RAW_AXIS:1 = first raw analog value
        /// - VJOY_BUTTON:1 = first vjoy button
        /// - VJOY_AXIS:1 = first vjoy axis (X)
        /// Axis values are usually indexed as: X, Y, Z, RX, RY, RZ, ... see vJoy's definition
        /// </summary>
        public static string ConvertKeySourceToString(KeySourceTypes type, int index)
        {
            return type.ToString() + SourceSplitter[0] + index.ToString();
        }

        public static bool TryParse(string keysource, out KeySourceTypes type, out int index)
        {
            var tokens = keysource.Split(SourceSplitter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length!=2) {
                type = KeySourceTypes.UNDEF;
                index = -1;
                return false;
            }
            if (!Enum.TryParse<KeySourceTypes>(tokens[0], out type)) {
                type = KeySourceTypes.UNDEF;
            }
            if (!int.TryParse(tokens[1], out index)) {
                index = 0;
            }
            return true;
        }

        public override string ToString()
        {
            return ConvertKeySourceToString(this.Type, this.Index);
        }
    }
    /// <summary>
    /// Keystroke database
    /// </summary>
    [Serializable]
    public class KeyStrokeDB :
        ICloneable
    {
        public string UniqueName;

        public KeyCodes KeyCode;
        public List<KeyCodes> MappedKeyStrokes; // Future...
        public List<KeySource> KeySources;
        public List<KeysOperators> KeyCombineOperators;

        public long HoldTime_ms = 0;
        public bool IsInvertedLogic = false;
        public KeyEmulationAPI KeyAPI = KeyEmulationAPI.DInput;

        public KeyStrokeDB()
        {
            MappedKeyStrokes = new List<KeyCodes>(1);
            KeySources = new List<KeySource>(3);

            KeyCombineOperators = new List<KeysOperators>(2);
        }

        public void Initialize()
        {
            KeySources.Clear();
            for (int i = 0; i<3; i++) {
                KeySources.Add(new KeySource());
            }
            KeyCombineOperators.Clear();
            for (int i = 0; i<2; i++) {
                KeyCombineOperators.Add(new KeysOperators());
            }
        }

        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<KeyStrokeDB>(this);
            return obj;
        }

        public string GetExpression()
        {
            StringBuilder expr = new StringBuilder(this.KeySources[0].ToString());
            if (this.KeyCombineOperators[0]!= KeysOperators.NO) {
                expr.Append(" " + this.KeyCombineOperators[0].ToString() + " ");
                expr.Append(this.KeySources[1].ToString());

                if (this.KeyCombineOperators[1]!= KeysOperators.NO) {
                    expr.Insert(0, "(");
                    expr.Append(")");
                    expr.Append(" " + this.KeyCombineOperators[1].ToString() + " ");
                    expr.Append(this.KeySources[2].ToString());
                }
            }
            return expr.ToString();
        }


    }
}
