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
        public string MappedvJoyAxis;
        // For axis linear correction
        public List<Point> ControlPoints;
        // In case pedal calibration is required, save pos/neg and full range options
        public bool IsNegativeDirection;
        public bool IsFullRangeAxis;

        public RawAxisDB()
        {
            MappedvJoyAxis = null;
            ControlPoints = new List<Point>();
        }
        public object Clone()
        {
            var obj = BackForceFeeder.Utils.Files.DeepCopy<RawAxisDB>(this);
            return obj;
        }
    }
}
