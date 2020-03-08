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
        public bool DumpToLogFile = false;
        public bool VerbosevJoyManager = false;
        public bool VerboseFFBManager = false;
        public bool VerbosevJoyFFBReceiver = false;

        public int AutoFirePeriod_ms;

        public double GlobalGain = 1.0;
        public bool SkipStopEffect = false;
        public bool UseTrqEmulationForMissing = true;
        public bool UsePulseSeq = true;
        public double PowerLaw = 1.2;
        public double TrqDeadBand = 0.0;
        public double TimeoutForInit_ms = 30000;

        /// <summary>
        /// False (+1.0) if turning wheel left increments position value (= positive speed)
        /// True (-1.0) if turning wheel left decrements position value (= negative speed)
        /// </summary>
        public bool InvertWheelDirection = true;
        /// <summary>
        /// False (+1.0) if positive torque command turn wheel left
        /// True (-1.0) if positive torque command turn wheel right
        /// </summary>
        public bool InvertTrqDirection = false;


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
