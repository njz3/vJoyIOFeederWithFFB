using BackForceFeeder.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// Rawinput state management
    /// </summary>
    public class RawInput
    {
        public delegate void StateTriggerEvent(RawInput sender, bool newstate);
        public event StateTriggerEvent StateTrigger;
        
        /// <summary>
        /// Raw input index (can be used as a self reference)
        /// </summary>
        public int Index;
        /// <summary>
        /// Raw input value from hardware
        /// </summary>
        public bool RawValue { get; protected set; }

        /// <summary>
        /// Current filtered value
        /// </summary>
        public bool Value { get; protected set; }
        /// <summary>
        /// Previous filtered value
        /// </summary>
        public bool PrevValue { get; protected set; }

        /// <summary>
        /// Configuration for this raw input
        /// </summary>
        public RawInputDB Config;

        /// <summary>
        /// Default timer for rising edge detection
        /// Should be stored in Config
        /// </summary>
        public ulong ValidateDelay_ms = 0;


        protected ulong _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        protected ulong _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        /// <summary>
        /// Update internal state and detect if state has changed
        /// </summary>
        /// <param name="rawstate"></param>
        /// <returns>true is state has changed</returns>
        public bool UpdateState(bool rawstate)
        {
            bool stt = false;
            // Default input value is current logic (false if not inverted)
            bool newrawval = Config.IsInvertedLogic;
            // Check if input is "on" and invert default value
            if (rawstate) {
                // If was false, then set true
                newrawval = !newrawval;
            }
            // Store corrected rawval
            RawValue = newrawval;
            // Check for state change
            if (RawValue!=PrevValue) {
                // Update last change timer
                _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                var elapsed_ms = _lastTimeRefresh_ms -_lastTimeChange_ms;
                if (elapsed_ms<ValidateDelay_ms) {
                    // stay in current state until timeout
                } else {
                    // timeout, validate current state
                    PrevValue = Value;
                    Value = RawValue;
                    // Trigger
                    StateTrigger(this, Value);
                    stt = true;



                }
            }
            // Update last refresh timer
            _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
            return stt;
        }

    }
}
