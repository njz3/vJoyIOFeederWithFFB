using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// Inputs processing
    /// </summary>
    public abstract class Inputs
    {
        public delegate void SingleTriggerEvent(RawInput sender, bool state);
        public event SingleTriggerEvent SingleTrigger;

        public delegate void StatesTriggerEvent(Inputs sender, UInt64 prevstates, UInt64 newstates);
        public event StatesTriggerEvent StatesTrigger;

        public const int MAXRAWINPUTS = 32;

        /// <summary>
        /// Raw inputs (up to 32), used to store config and fire events
        /// </summary>
        public List<RawInput> RawInputs;

        /// <summary>
        /// Combined value for all inputs
        /// </summary>
        public UInt64 InputStates { get; protected set; }
        /// <summary>
        /// Previous combined value for all inputs
        /// </summary>
        public UInt64 PrevInputsStates { get; protected set; }

        public Inputs(uint number)
        {
            if (number>MAXRAWINPUTS) {
                number = MAXRAWINPUTS;
            }
            RawInputs = new List<RawInput>((int)number);
            for (uint i = 0; i<number; i++) {
                RawInputs.Add(new RawInput());
            }
        }


        /// <summary>
        /// Update all raw values from given state register.
        /// Will fire an trigger event.
        /// </summary>
        /// <param name="allbits"></param>
        public bool UpdateAll(UInt64 newstates)
        {
            bool stt = false;
            if (newstates!=InputStates) {
                for (int i = 0; i<RawInputs.Count; i++) {
                    bool newstate = (newstates & (ulong)(1<<i)) != 0;
                    // Check state update
                    if (RawInputs[i].UpdateState(newstate)) {
                        stt = true;
                    }
                }
            }
            if (stt) {
                // Rebuilt new inputstates
                PrevInputsStates = InputStates;
                for (int i = 0; i<RawInputs.Count; i++) {
                    if (RawInputs[i].Value) {
                        // Set
                        InputStates |= (ulong)1<<i;
                    } else {
                        // Clear
                        InputStates &= ~(ulong)1<<i;
                    }
                }
                InputStates = newstates;
            }
            return stt;
        }

        /// <summary>
        /// Update a single raw input
        /// Will fire a trigger event.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        public bool UpdateSingle(int idx, bool newvalue)
        {
            bool stt = false;
            if (RawInputs[idx].UpdateState(newvalue))
                stt = true;
            return stt;
        }

        public void SynchronizeToState()
        {

        }
    }
}
