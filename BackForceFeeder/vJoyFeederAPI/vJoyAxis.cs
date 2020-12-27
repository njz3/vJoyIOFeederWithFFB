using BackForceFeeder.Configuration;
using BackForceFeeder.Managers;
using System;

namespace BackForceFeeder.vJoyIOFeederAPI
{
    /// <summary>
    /// This stores all information about all vJoy axes.
    /// Maximum supported by vJoy 2.2.x: 16 axes
    /// X Y Z RX RY RZ SL0 SL1
    /// </summary>
    public class vJoyAxisInfos
    {
        #region From/To vJoy
        public string Name;
        public HID_USAGES HID_Usage;
        public bool IsPresent;

        public long MinValue;
        public long MaxValue;

        public long RawValue;
        public long CorrectedValue;
        #endregion

        public vJoyAxisInfos()
        { }
    }


    /// <summary>
    /// Single mapping between Raw Axis and vJoyAxis.
    /// Contains calibration data from current control set
    /// </summary>
    public class UsedvJoyAxis
    {
        #region Mapping a raw axis index to a vJoy axis
        public vJoyAxisInfos vJoyAxisInfo = null;
        public int RawAxisIndex = -1;
        #endregion

        public UsedvJoyAxis()
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

        public int CorrectionSegment12bits(uint axe12bits)
        {
            return CorrectionMinMax(CorrectionSegment(Normalize12b(axe12bits)));
        }
        public int CorrectionSegment16bits(uint axe16bits)
        {
            return CorrectionMinMax(CorrectionSegment(Normalize16b(axe16bits)));
        }
        public double Normalize12b(uint axe12bits)
        {
            // Scale input to 0.0 ... 1.0
            return (double)(axe12bits) * (1.0 / (double)0xFFF);
        }

        public double Normalize16b(uint axe16bits)
        {
            // Scale input to 0.0 ... 1.0
            return (double)(axe16bits) * (1.0 / (double)0xFFFF);
        }

        public int CorrectionMinMax(double scaled_f)
        {
            // get back into axis range
            var finalvalue = (int)(vJoyAxisInfo.MinValue + (vJoyAxisInfo.MaxValue - vJoyAxisInfo.MinValue) * scaled_f);
            return finalvalue;
        }

        public int CorrectionMed(double scaled_f)
        {
            var med = (vJoyAxisInfo.MaxValue + vJoyAxisInfo.MinValue) / 2;
            // get back into axis range
            var finalvalue = (int)(med + (vJoyAxisInfo.MaxValue - med) * scaled_f);
            return finalvalue;
        }



        public double CorrectionSegment(double scaled_f)
        {
            var axisdb = this.RawAxisDB;
            var idx = axisdb.FindIndexRange(scaled_f);
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
            double ratio = (scaled_f - axisdb.ControlPoints[idx].X) / range;
            double newscale = ratio * incre + axisdb.ControlPoints[idx].Y;
            double outval = Math.Min(1.0, Math.Max(0.0, newscale));
            return outval;
        }

        #endregion

    }

}
