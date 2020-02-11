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
    public class RawInputDB
    {
        public int[] vJoyBtns;
        public bool IsInvertedLogic;
        public bool IsToggle;
        public bool IsAutoFire;

        public RawInputDB()
        {
            vJoyBtns = new int[0];
            IsInvertedLogic = false;
            IsToggle = false;
            IsAutoFire = false;
        }

    }
}
