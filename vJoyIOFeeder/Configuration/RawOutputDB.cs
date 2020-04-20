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
    public class RawOutputDB :
        ICloneable
    {
        public List<int> RawOutputBit;
        public bool IsInvertedLogic = false;
        public bool IsToggle = false;

        public RawOutputDB()
        {
            RawOutputBit = new List<int>(1);
        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<RawOutputDB>(this);
            return obj;
        }
    }
}
