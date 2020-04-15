using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class RawAxisDB :
        ICloneable
    {
        public string vJoyAxis;
        // For axis linear correction
        public List<Point> ControlPoints;

        public RawAxisDB()
        {
            vJoyAxis = null;
            ControlPoints = new List<Point>();
        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<RawAxisDB>(this);
            return obj;
        }
    }
}
