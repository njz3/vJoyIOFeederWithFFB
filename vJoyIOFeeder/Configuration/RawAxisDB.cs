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
    [Serializable]
    public class RawAxisDB :
        ICloneable
    {
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

        public RawAxisDB()
        {
            MappedIndexUsedvJoyAxis = 0;
            ControlPoints = new List<Point>();
        }
        public object Clone()
        {
            var obj = BackForceFeeder.Utils.Files.DeepCopy<RawAxisDB>(this);
            return obj;
        }

        /// <summary>
        /// Find closest control point
        /// </summary>
        /// <param name="scaled_f"></param>
        /// <returns>-1 if out of range, idx if within range</returns>
        public int FindClosestControlPoint(double scaled_f)
        {
            // Ensure a range exists
            if (this.ControlPoints.Count < 2)
                throw new Exception("Not enough control points");

            // most negative
            if (scaled_f < this.ControlPoints[0].X)
                return 0;

            // most positive
            if (scaled_f > this.ControlPoints[this.ControlPoints.Count - 1].X)
                return this.ControlPoints.Count - 1;


            // Find range [idx; idx+1]
            int idx = FindIndexRange(scaled_f);
            // No check for closest
            var negdist = Math.Abs(scaled_f - this.ControlPoints[idx].X);
            var posdist = Math.Abs(scaled_f - this.ControlPoints[idx+1].X);
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
        /// <param name="scaled_f"></param>
        /// <returns>-1 if out of range, idx if within range</returns>
        public int FindIndexRange(double scaled_f)
        {
            // Ensure a range exists
            if (this.ControlPoints.Count < 2)
                throw new Exception("Not enough control points");

            // Out of range? Strict limits
            if (scaled_f < this.ControlPoints[0].X)
                return -1;
            if (scaled_f > this.ControlPoints[this.ControlPoints.Count - 1].X)
                return this.ControlPoints.Count;


            // Find index by simple scanning
            // Next: find index using dichotomy!
            int idx = 0;
            for (; idx < this.ControlPoints.Count-1; idx++) {
                if (scaled_f <= this.ControlPoints[idx+1].X) {
                    break;
                }
            }
            // Now, input value is between idx and idx+1
            return idx;
        }
    }
}
