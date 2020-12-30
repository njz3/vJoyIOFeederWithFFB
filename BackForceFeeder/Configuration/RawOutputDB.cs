using System;
using System.Collections.Generic;

namespace BackForceFeeder.Configuration
{

    [Serializable]
    public class RawOutputDB :
        ICloneable
    {
        /// <summary>
        /// Map game output to raw output
        /// </summary>
        public List<int> MappedRawOutputBit;
        /// <summary>
        /// Inverted logic for game input?
        /// </summary>
        public bool IsInvertedLogic = false;
        /// <summary>
        /// Toggling output?
        /// </summary>
        public bool IsToggle = false;

        public RawOutputDB()
        {
            MappedRawOutputBit = new List<int>(1);
        }
        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<RawOutputDB>(this);
            return obj;
        }
    }
}
