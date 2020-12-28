using BackForceFeeder.Configuration;
using BackForceFeeder.Managers;
using BackForceFeeder.Utils;
using System;

namespace BackForceFeeder.vJoyIOFeederAPI
{
    /// <summary>
    /// This stores all information about all available vJoy axes.
    /// Maximum supported by vJoy 2.2.x: 16 axes, but only 8 can be reported by Windows
    /// X Y Z RX RY RZ SL0 SL1
    /// </summary>
    public class vJoyAxisInfos
    {
        #region From/To vJoy
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
        /// <summary>
        /// Corrected vjoy value in percent, after mapping and correction
        /// </summary>
        public double AxisValue_pct;
        /// <summary>
        /// Corrected value in vJoy range (min..max)
        /// </summary>
        public long AxisValue {
            get {
                return (long)(this.MinValue + (this.MaxValue - this.MinValue) * AxisValue_pct);
            }
        }
        #endregion

        public vJoyAxisInfos()
        { }

        public override string ToString()
        {
            return Name + " (HID:" + HID_Usage.ToString() + ") min=" + MinValue + " max=" + MaxValue;
        }
    }


    /// <summary>
    /// Single mapping between Raw Axis and vJoyAxis.
    /// Contains calibration data from current control set
    /// </summary>
    public class MappingRawTovJoyAxis
    {
        /// <summary>
        /// Raw axis value in counts
        /// </summary>
        public Int64 RawValue_cts;
        /// <summary>
        /// Raw axis value in percent, before applying mapping and correction
        /// </summary>
        public double RawValue_pct {
            get {
                switch (RawAxisDB.RawAxisType) {
                    case RawAxisTypes.Analog12bits:
                        return Converting.Normalize12b((uint)RawValue_cts);
                    case RawAxisTypes.Analog16bits:
                        return Converting.Normalize16b((uint)RawValue_cts);
                    case RawAxisTypes.Encoder32bits:
                    default:
                        return (double)(RawValue_cts) * (1.0 / (double)(RawAxisDB.RawMax_cts-RawAxisDB.RawMin_cts)); ;
                }
            }
        }


        #region Mapping a raw axis index to a vJoy axis
        public vJoyAxisInfos vJoyAxisInfo = null;
        public int RawAxisIndex = -1;
        #endregion

        public MappingRawTovJoyAxis()
        {
        }

        public RawAxisDB RawAxisDB {
            get {
                if (RawAxisIndex>=0 && RawAxisIndex<BFFManager.Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Count) {
                    return BFFManager.Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB[RawAxisIndex];
                } else {
                    throw new Exception("Missing axis configuration for index " + RawAxisIndex);
                }
            }
            set {
                if (RawAxisIndex>=0 && RawAxisIndex<BFFManager.Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB.Count) {
                    BFFManager.Config.CurrentControlSet.vJoyMapping.RawAxisTovJoyDB[RawAxisIndex] = value;
                } else {
                    throw new Exception("Missing axis configuration for index " + RawAxisIndex);
                }
            }
        }

        #region Calibration or Correction using configured value

        public int CorrectionMinMax(double scaled_pct)
        {
            // get back into axis range
            var finalvalue = (int)(vJoyAxisInfo.MinValue + (vJoyAxisInfo.MaxValue - vJoyAxisInfo.MinValue) * scaled_pct);
            return finalvalue;
        }

        public int CorrectionMed(double scaled_pct)
        {
            var med = (vJoyAxisInfo.MaxValue + vJoyAxisInfo.MinValue) / 2;
            // get back into axis range
            var finalvalue = (int)(med + (vJoyAxisInfo.MaxValue - med) * scaled_pct);
            return finalvalue;
        }
        public double CorrectionSegment(double scaled_pct)
        {
            var axisdb = this.RawAxisDB;
            var idx = axisdb.FindIndexRange(scaled_pct);
            // Exception for first and last point
            if (idx < 0)
                return axisdb.ControlPoints[0].Y;
            if (idx >= axisdb.ControlPoints.Count)
                return axisdb.ControlPoints[axisdb.ControlPoints.Count - 1].Y;
            if (idx == axisdb.ControlPoints.Count - 1)
                idx--;

            // use linear approximation between index and next point
            double range = axisdb.ControlPoints[idx + 1].X - axisdb.ControlPoints[idx].X;
            double incre = axisdb.ControlPoints[idx + 1].Y - axisdb.ControlPoints[idx].Y;
            double ratio = (scaled_pct - axisdb.ControlPoints[idx].X) / range;
            double newscale = ratio * incre + axisdb.ControlPoints[idx].Y;
            double outval = Math.Min(1.0, Math.Max(0.0, newscale));
            return outval;
        }
        public int CorrectionMinMaxSegment(double scaled_pct)
        {
            return CorrectionMinMax(CorrectionSegment(scaled_pct));
        }

        public int CorrectionMinMaxSegment12bits(uint axe12bits)
        {
            return CorrectionMinMaxSegment(Converting.Normalize12b(axe12bits));
        }
        public int CorrectionMinMaxSegment16bits(uint axe16bits)
        {
            return CorrectionMinMaxSegment(Converting.Normalize16b(axe16bits));
        }



        #endregion

    }

}
