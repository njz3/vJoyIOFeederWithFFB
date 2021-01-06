using System;
using System.Collections.Generic;

namespace BackForceFeeder.Configuration
{

    public enum OutputSequence : int
    {
        None = 0,
        Flash,
        Roll,
        BackAndForth,
    }

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

        /// <summary>
        /// Period for playing sequence
        /// </summary>
        public int SequenceDelay_ms = 2000;
        /// <summary>
        /// For Up/Down shifter decoder: time in neutral before a new gear is 
        /// engaged.
        /// </summary>
        public OutputSequence Sequence = OutputSequence.None;
        [NonSerialized]
        public ulong SequenceLastTime_ms = 0;
        [NonSerialized]
        public int SequenceIndex = 0;

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
