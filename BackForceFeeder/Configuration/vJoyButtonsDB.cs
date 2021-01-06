using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Configuration
{
    [Serializable]
    public class vJoyButtonsDB :
        ICloneable
    {
        /// <summary>
        /// Period for autofire
        /// </summary>
        public int AutoFirePeriod_ms = 100;
        /// <summary>
        /// For Up/Down shifter decoder: time in neutral before a new gear is 
        /// engaged.
        /// </summary>
        public int UpDownNeutralDelay_ms = 300;
        /// <summary>
        /// For Up/Down shifter decoder: time to maintain a button when a new
        /// gear is decoded.
        /// If <=0, then maintain indefinitly.
        /// </summary>
        public int UpDownMaintain_ms = 2000;

        public vJoyButtonsDB()
        {
        }

        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<vJoyButtonsDB>(this);
            return obj;
        }
    }

}
