﻿using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Outputs
{
    /// <summary>
    /// Single raw inputs
    /// </summary>
    public class RawOutput
    {
        /// <summary>
        /// Raw axis index. Must be set at runtime after init
        /// </summary>
        public int RawOutputIndex = -1;
        /// <summary>
        /// Will be valid at runtime
        /// </summary>
        public RawOutputDB Config {
            get {
                if (RawOutputIndex>=0 && RawOutputIndex<BFFManager.CurrentControlSet.RawInputDBs.Count)
                    return BFFManager.CurrentControlSet.RawOutputDBs[RawOutputIndex];
                return null;
            }
        }
        /// <summary>
        /// Raw input value from hardware
        /// </summary>
        public bool RawValue { get; protected set; } = false;
        /// <summary>
        /// Current filtered logical state
        /// </summary>
        public bool State { get; protected set; } = false;
        /// <summary>
        /// Previous filtered logical state
        /// </summary>
        public bool PrevState { get; protected set; } = false;
        /// <summary>
        /// Clear internal values
        /// </summary>
        public void Clear()
        {
            UpdateValue(false);
        }

        /// <summary>
        /// Update internal value, and detect if state has changed
        /// </summary>
        /// <param name="rawvalue"></param>
        /// <returns>true if state has changed</returns>
        public bool UpdateValue(bool rawvalue)
        {
            bool stt = false;
            // Store corrected rawval
            if (Config!=null && Config.IsInvertedLogic) {
                rawvalue = !rawvalue;
            }
            
            // Detect change
            if (rawvalue!=RawValue) {
                // Store corrected rawval
                RawValue = rawvalue;

                // Toggle?
                if (Config.IsToggle) {
                    // Only toggle for low->high edge
                    if (RawValue)
                        State = !State;
                } else {
                    // Simple copy
                    State = RawValue;
                }
            }


            return stt;
        }

    }
}
