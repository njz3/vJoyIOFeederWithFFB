using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

namespace BackForceFeeder.Configuration
{
    public enum RawAxisTypes
    {
        Analog12bits = 0,
        Analog16bits,
        Encoder32bits,
    }

    [Serializable]
    public class RawAxisDB :
        ICloneable
    {
        public RawAxisTypes RawAxisType;
        /// <summary>
        /// Index base 0 of an active vJoy axis.
        /// Currently it is an increasing value from 0 to n axes
        /// </summary>
        public int MappedIndexUsedvJoyAxis;
        // For axis linear correction
        public List<Point> ControlPoints;
        // In case pedal calibration is required, save pos/neg and full range options
        public bool IsNegativeDirection;
        public bool IsFullRangeAxis;

        public long RawMin_cts;
        public long RawMax_cts;

        public RawAxisDB()
        {
            RawAxisType = RawAxisTypes.Analog12bits;
            RawMin_cts = 0;
            RawMax_cts = 4095;
            MappedIndexUsedvJoyAxis = 0;
            ControlPoints = new List<Point>();
        }
        public object Clone()
        {
            var obj = BackForceFeeder.Utils.Files.DeepCopy<RawAxisDB>(this);
            return obj;
        }

        public void ResetCorrectionFactors()
        {
            ControlPoints.Clear();
            ControlPoints.Add(new Point(0.0, 0.0));
            ControlPoints.Add(new Point(1.0, 1.0));
        }

        /// <summary>
        /// Find closest control point
        /// </summary>
        /// <param name="scaled_pct"></param>
        /// <returns>-1 if out of range, idx if within range</returns>
        public int FindClosestControlPoint(double scaled_pct)
        {
            // Ensure a range exists
            if (this.ControlPoints.Count < 2)
                throw new Exception("Not enough control points");

            // most negative
            if (scaled_pct < this.ControlPoints[0].X)
                return 0;

            // most positive
            if (scaled_pct > this.ControlPoints[this.ControlPoints.Count - 1].X)
                return this.ControlPoints.Count - 1;


            // Find range [idx; idx+1]
            int idx = FindIndexRange(scaled_pct);
            // No check for closest
            var negdist = Math.Abs(scaled_pct - this.ControlPoints[idx].X);
            var posdist = Math.Abs(scaled_pct - this.ControlPoints[idx+1].X);
            if (negdist<posdist) {
                return idx; // neg closest
            } else {
                return idx+1; //pos closest
            }
        }

        /// <summary>
        /// Find range [idx; idx+] where value belongs to.
        /// If value is below negative limit, returns -1 (no value)
        /// If value is above positive limit, return count
        /// </summary>
        /// <param name="scaled_pct"></param>
        /// <returns>-1 if out of range, idx if within range</returns>
        public int FindIndexRange(double scaled_pct)
        {
            // Ensure a range exists
            if (this.ControlPoints.Count < 2)
                throw new Exception("Not enough control points");

            // Out of range? Strict limits
            if (scaled_pct < this.ControlPoints[0].X)
                return -1;
            if (scaled_pct > this.ControlPoints[this.ControlPoints.Count - 1].X)
                return this.ControlPoints.Count;


            // Find index by simple scanning
            // Next: find index using dichotomy!
            int idx = 0;
            for (; idx < this.ControlPoints.Count-1; idx++) {
                if (scaled_pct <= this.ControlPoints[idx+1].X) {
                    break;
                }
            }
            // Now, input value is between idx and idx+1
            return idx;
        }
    }
}
