using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyIOFeeder.Utils;
using static vJoyIOFeeder.vJoyManager;

namespace vJoyIOFeeder.Configuration
{
    public class FeederConfig
    {
        public ApplicationDB Application;
        public HardwareDB Hardware;
        public ControlSetDB CurrentControlSet;
        public ControlSetsDB AllControlSets;

        /// <summary>
        /// Default values
        /// </summary>
        public FeederConfig()
        {
            Application = new ApplicationDB();
            Hardware = new HardwareDB();
            CurrentControlSet = new ControlSetDB();
            AllControlSets = new ControlSetsDB();
        }


    }
}
