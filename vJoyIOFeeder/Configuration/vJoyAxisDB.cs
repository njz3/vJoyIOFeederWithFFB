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
    public class vJoyAxisDB
    {
        // For axis linear correction
        public List<Point> ControlPoints;
        public vJoyAxisDB()
        {
            ControlPoints = new List<Point>();
        }

    }
}
