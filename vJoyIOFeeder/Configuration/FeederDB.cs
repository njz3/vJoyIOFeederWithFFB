using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyIOFeeder.Utils;
using static vJoyIOFeeder.vJoyManager;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class FeederDB
    {
        public FFBTranslatingModes TranslatingModes;
        public List<vJoyAxisDB> AxisDB;
        public List<string> GameListXMLFile;
        public List<string> FFBparamXMLFile;
        public bool RunWithoutIOBoard;
        public LogLevels LogLevel;

        /// <summary>
        /// Default values
        /// </summary>
        public FeederDB()
        {
            TranslatingModes = FFBTranslatingModes.PWM_DIR;
            AxisDB = new List<vJoyAxisDB>();
            GameListXMLFile = new List<string>();
            FFBparamXMLFile = new List<string>();
            RunWithoutIOBoard = true;
            LogLevel = LogLevels.INFORMATIVE;
        }
    }
}
