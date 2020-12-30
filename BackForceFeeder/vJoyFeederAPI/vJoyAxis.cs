using BackForceFeeder.Configuration;
using BackForceFeeder.Inputs;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using System;

namespace BackForceFeeder.vJoyIOFeederAPI
{
    /// <summary>
    /// This stores all information about all available vJoy axes.
    /// Information is not configurable, it is retrieved at runtime through
    /// the vJoy API.
    /// Maximum supported by vJoy 2.2.x: 16 axes, but only 8 can be reported by Windows
    /// X Y Z RX RY RZ SL0 SL1
    /// </summary>
    public class vJoyAxisInfos
    {
        #region Retrieved from vJoy at runtime
        public string Name;
        public HID_USAGES HID_Usage;
        public bool IsPresent;
        /// <summary>
        /// Min value for vJoy
        /// </summary>
        public long MinValue;
        /// <summary>
        /// Max value for vJoy
        /// </summary>
        public long MaxValue;
        #endregion
        
        #region To vJoy
        /// <summary>
        /// Corrected vjoy value in percent, after mapping and correction
        /// </summary>
        public double AxisValue_pct;
        /// <summary>
        /// Corrected value in vJoy range (min..max)
        /// </summary>
        public long AxisValue_cts {
            get {
                return (long)(this.MinValue + (this.MaxValue - this.MinValue) * AxisValue_pct);
            }
        }
        public int ScaleTovJoyMinMax(double scaled_pct)
        {
            // get back into axis range
            var finalvalue = (int)(this.MinValue + (this.MaxValue - this.MinValue) * scaled_pct);
            return finalvalue;
        }

        public int ScaleTovJoyMed(double scaled_pct)
        {
            var med = (this.MaxValue + this.MinValue) / 2;
            // get back into axis range
            var finalvalue = (int)(med + (this.MaxValue - med) * scaled_pct);
            return finalvalue;
        }
        #endregion

        public vJoyAxisInfos()
        { }

        public override string ToString()
        {
            return Name + " (HID:" + HID_Usage.ToString() + ") min=" + MinValue + " max=" + MaxValue;
        }
    }

}
