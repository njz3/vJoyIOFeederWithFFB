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
    /// <summary>
    /// In case this input is part of a shifter decoder map
    /// </summary>
    public enum ShifterDecoderMap : uint
    {
        /// <summary>
        /// Not part of any decoder
        /// </summary>
        No = 0,
        #region H shifter decoder with 3 inputs
        /// <summary>
        /// Input used for left/right detection
        /// </summary>
        HShifterLeftRight,
        /// <summary>
        /// Input used for Up detection
        /// </summary>
        HShifterUp,
        /// <summary>
        /// Input used for Down detection
        /// </summary>
        HShifterDown,
        #endregion

        #region Up/Down sequentiel shifter to H shifter 4-gears+Neutral emulation
        /// <summary>
        /// Input used for Up
        /// </summary>
        SequencialUp,
        /// <summary>
        /// Input used for Down
        /// </summary>
        SequencialDown,
        #endregion
    }


    [Serializable]
    public class RawInputDB :
        ICloneable
    {
        /// <summary>
        /// List of vjoy buttons, base 0. (-1 does not exist)
        /// </summary>
        public List<int> MappedvJoyBtns;

        #region Hardware configuration
        public double HoldTime_s = 0;
        public bool IsInvertedLogic = false;
        public bool IsToggle = false;
        public bool IsAutoFire = false;
        #endregion

        #region Emulate sequenced buttons press ?
        public bool IsSequencedvJoy = false;
        [NonSerialized]
        public int SequenceCurrentToSet = 0;
        #endregion

        #region Shifter decoder?
        /// <summary>
        /// 0: not part of shifter decoder
        /// </summary>
        public ShifterDecoderMap ShifterDecoder = ShifterDecoderMap.No;
        /// <summary>
        /// If using a decoder, tell whether first is neutral
        /// </summary>
        public bool IsNeutralFirstBtn = false;
        #endregion


        public RawInputDB()
        {
            MappedvJoyBtns = new List<int>(1);
        }
        public object Clone()
        {
            var obj = global::BackForceFeeder.Utils.Files.DeepCopy<RawInputDB>(this);
            return obj;
        }
    }
}
