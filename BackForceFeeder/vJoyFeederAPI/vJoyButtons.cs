using BackForceFeeder.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// vJoy button
    /// </summary>
    public class vJoyButton
    {
        /// <summary>
        /// Current filtered value
        /// </summary>
        public bool Value { get; protected set; }
        /// <summary>
        /// Previous filtered value
        /// </summary>
        public bool PrevValue { get; protected set; }

        /// <summary>
        /// Configuration for this vJoy button
        /// </summary>
        public RawInputDB Config;

    }

    /// <summary>
    /// vJoy buttons management
    /// </summary>
    public class vJoyButtons
    {
        public void ProcessInputs(Inputs inputs)
        {

        }
    }
}
