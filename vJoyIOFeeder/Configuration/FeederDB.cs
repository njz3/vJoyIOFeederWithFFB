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
        public bool RunWithoutIOBoard;
        public LogLevels LogLevel;

        public List<RawAxisDB> RawAxisTovJoyDB;
        public List<RawInputDB> RawInputTovJoyMap;

        public List<string> GameListXMLFile;
        public List<string> FFBparamXMLFile;

        public int AutoFirePeriod_ms;

        /// <summary>
        /// Default values
        /// </summary>
        public FeederDB()
        {
            TranslatingModes = FFBTranslatingModes.PWM_DIR;
            RunWithoutIOBoard = true;
            LogLevel = LogLevels.INFORMATIVE;

            RawAxisTovJoyDB = new List<RawAxisDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_AXES_VJOY);
            RawInputTovJoyMap = new List<RawInputDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_BUTTONS_VJOY);
            GameListXMLFile = new List<string>();
            FFBparamXMLFile = new List<string>();
        }
    }
}
