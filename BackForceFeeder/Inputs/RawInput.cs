using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// Single raw inputs
    /// </summary>
    public class RawInput
    {
        /// <summary>
        /// Raw axis index. Must be set at runtime after init
        /// </summary>
        public int RawInputIndex = -1;
        /// <summary>
        /// Will be valid at runtime
        /// </summary>
        public RawInputDB Config { get { return BFFManager.CurrentControlSet.RawInputDBs[RawInputIndex]; } }


        public delegate void StateChangeEvent(RawInput sender, bool newstate);
        public event StateChangeEvent StateChange;
        
        /// <summary>
        /// Raw input value from hardware
        /// </summary>
        public bool RawValue { get; protected set; } = false;

        /// <summary>
        /// Current filtered logical state (includes inverted logic and delay)
        /// </summary>
        public bool State { get; protected set; } = false;
        /// <summary>
        /// Previous filtered logical state
        /// </summary>
        public bool PrevState { get; protected set; } = false;


        /// <summary>
        /// Default timer for rising edge detection
        /// Should be stored in Config
        /// </summary>
        public long ValidateDelay_ms = 0;


        protected ulong _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        protected ulong _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;

        /// <summary>
        /// Update internal value, and detect if state has changed
        /// </summary>
        /// <param name="rawvalue"></param>
        /// <returns>true if state has changed</returns>
        public bool UpdateValue(bool rawvalue)
        {
            bool stt = false;
            // Default input value is current logic (false if not inverted)
            bool newrawval = Config.IsInvertedLogic;
            // Check if input is "on" and invert default value
            if (rawvalue) {
                // If was false, then set true
                newrawval = !newrawval;
            }
            // Store corrected rawval
            RawValue = newrawval;

            // Check for state change
            if (RawValue!=State) {
                // Update last change timer
                _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                var elapsed_ms = _lastTimeRefresh_ms - _lastTimeChange_ms;
                if ((long)elapsed_ms>ValidateDelay_ms) {
                    // timeout, validate current state
                    PrevState = State;
                    State = RawValue;
                    // Trigger state change event
                    if (StateChange!=null)
                        StateChange(this, State);
                    stt = true;
                } else {
                    // stay in current state until timeout
                }
            }
            // Update last refresh timer
            if (stt)
                _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
            return stt;
        }

    }
}
