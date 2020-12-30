using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;
using System;

namespace BackForceFeeder.Inputs
{

    /// <summary>
    /// Raw Axis
    /// Contains calibration data from current control set
    /// </summary>
    public class RawAxis
    {
        /// <summary>
        /// Raw axis index. Must be set at runtime after init
        /// </summary>
        public int RawAxisIndex = -1;
        /// <summary>
        /// Will be valid at runtime
        /// </summary>
        public RawAxisDB Config {
            get {
                return BFFManager.CurrentControlSet.RawAxisDBs[RawAxisIndex];
            }
        }

        /// <summary>
        /// Raw axis value in counts
        /// </summary>
        public Int64 RawValue_cts { get; protected set; } = 0;
        /// <summary>
        /// Raw axis value in percent, before applying mapping and correction
        /// </summary>
        public double RawValue_pct { get; protected set; }
        /// <summary>
        /// Corrected axis value in percent, after correction
        /// </summary>
        public double Corrected_pct { get; protected set; }

        public double NormalizeToPct(Int64 value)
        {
            // Scale input to 0.0 ... 1.0
            return (double)(value-Config.RawMin_cts) * (1.0 / (double)(Config.RawMax_cts-Config.RawMin_cts));
        }

        public RawAxis()
        {
        }
        public void UpdateValue(Int64 axis_cts)
        {
            RawValue_cts = axis_cts;
            switch (Config.RawAxisType) {
                case RawAxisTypes.Analog12bits:
                    RawValue_pct = Converting.Normalize12b((uint)RawValue_cts);
                    break;
                case RawAxisTypes.Analog16bits:
                    RawValue_pct =  Converting.Normalize16b((uint)RawValue_cts);
                    break;
                case RawAxisTypes.CustomRange:
                case RawAxisTypes.Encoder32bits:
                    RawValue_pct = NormalizeToPct(RawValue_cts);
                    break;
                default:
                    RawValue_pct = (double)(RawValue_cts) * (1.0 / (double)(Config.RawMax_cts)); ;
                    break;
            }
            Corrected_pct = CorrectionSegment(RawValue_pct);
        }

        #region Correction using control points
        /// <summary>
        /// Perform a piecewise segment linear correction using configurated
        /// control points.
        /// </summary>
        /// <param name="scaled_pct"></param>
        /// <returns></returns>
        protected double CorrectionSegment(double scaled_pct)
        {
            var axisdb = Config;
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
        #endregion

    }

}
