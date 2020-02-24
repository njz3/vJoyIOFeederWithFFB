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
        public FFBTranslatingModes TranslatingModes = FFBTranslatingModes.PWM_DIR;
        public bool RunWithoutIOBoard = true;
        public LogLevels LogLevel = LogLevels.INFORMATIVE;

        public int AutoFirePeriod_ms;

        public double GlobalGain = 1.0;
        public bool SkipStopEffect = false;
        public bool UseTrqEmulationForMissing = true;
        public bool UsePulseSeq = true;
        public double PowerLaw = 1.2;

        public List<RawAxisDB> RawAxisTovJoyDB;
        public List<RawInputDB> RawInputTovJoyMap;

        public List<string> GameListXMLFile;
        public List<string> FFBparamXMLFile;

        /// <summary>
        /// Default values
        /// </summary>
        public FeederDB()
        {
            RawAxisTovJoyDB = new List<RawAxisDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_AXES_VJOY);
            RawInputTovJoyMap = new List<RawInputDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_BUTTONS_VJOY);
            GameListXMLFile = new List<string>();
            FFBparamXMLFile = new List<string>();
        }
    }
}
