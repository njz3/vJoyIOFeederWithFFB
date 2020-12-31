using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// KeyStrokes management
    /// </summary>
    public class KeyStroke
    {
        /// <summary>
        /// Keystroke index. Must be set at runtime after init
        /// </summary>
        public int KeyStrokeIndex = -1;
        /// <summary>
        /// Will be valid at runtime
        /// </summary>
        public KeyStrokeDB Config { get { return BFFManager.CurrentControlSet.KeyStrokeDBs[this.KeyStrokeIndex]; } }

        /// <summary>
        /// Condition value from expression
        /// </summary>
        public bool Condition { get; protected set; } = false;
        /// <summary>
        /// Previous condition
        /// </summary>
        public bool PrevCondition { get; protected set; } = false;

        /// <summary>
        /// Current filtered logical state
        /// </summary>
        public bool State { get; protected set; } = false;
        /// <summary>
        /// Previous filtered logical state
        /// </summary>
        public bool PrevState { get; protected set; } = false;

        protected ulong _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        protected ulong _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;

        public KeyStroke()
        {
        }
        /// <summary>
        /// Update internal value, and detect if state has changed
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>true if state has changed</returns>
        public bool UpdateValue(bool condition)
        {
            bool stt = false;
            // Default input value is current logic (false if not inverted)
            bool newval = Config.IsInvertedLogic;
            // Check if input is "on" and invert default value
            if (condition) {
                // If was false, then set true
                newval = !newval;
            }
            // Store corrected rawval
            Condition = newval;
            
            // Check for state change
            if (Condition!=State) {
                // if going positive edge, then check for timer
                if (Condition) {
                    // Is it first edge?
                    if (Condition!=PrevCondition) {
                        // Update last refresh timer when first detecting edge
                        _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                        // Save
                        PrevCondition = Condition;
                        Logger.Log("[KEY] " + Config.UniqueName + ", condition verified ", LogLevels.DEBUG);
                    }

                    // Update last change timer
                    _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                    var elapsed_ms = _lastTimeRefresh_ms -_lastTimeChange_ms;
                    if ((long)elapsed_ms>Config.HoldTime_ms) {
                        // timeout, validate current state
                        PrevState = State;
                        State = Condition;
                        stt = true;
                        _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                        Logger.Log("[KEY] " + Config.UniqueName + ", validated state", LogLevels.DEBUG);
                    } else {
                        // stay in current state until timeout or condition becomes
                        // false
                    }
                } else {
                    // Going down, do not wait and reset all
                    PrevState = State;
                    State = Condition;
                    PrevCondition = State;
                    // Update last refresh timer when first detecting edge
                    _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                    Logger.Log("[KEY] " + Config.UniqueName + ", released", LogLevels.DEBUG);
                    stt = true;
                }
            } else {
                // Clear prevcondition flag
                PrevCondition = Condition;
            }
            return stt;
        }

    }
}
