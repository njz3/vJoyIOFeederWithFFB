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

        #region Software
        public bool StartMinimized = false;
        public bool ShortcutStartWithWindowsCreated = false;

        public LogLevels LogLevel = LogLevels.INFORMATIVE;
        public bool DumpToLogFile = false;
        public bool VerbosevJoyManager = false;
        public bool VerboseFFBManager = false;
        public bool VerboseFFBManagerTorqueValues = false;
        public bool VerbosevJoyFFBReceiver = false;
        public bool VerbosevJoyFFBReceiverDumpFrames = false;
        public bool VerboseSerialIO = false;
        public bool VerboseSerialIODumpFrames = false;
        #endregion

        #region  Hardware related
        public FFBTranslatingModes TranslatingModes = FFBTranslatingModes.PWM_DIR;
        public bool RunWithoutIOBoard = true;
        public double TimeoutForInit_ms = 30000;
        /// <summary>
        /// False (+1.0) if turning wheel left increments position value (= positive speed)
        /// True (-1.0) if turning wheel left decrements position value (= negative speed)
        /// </summary>
        public bool InvertWheelDirection = false;
        /// <summary>
        /// False (+1.0) if positive torque command turn wheel left
        /// True (-1.0) if positive torque command turn wheel right
        /// </summary>
        public bool InvertTrqDirection = false;
        public double WheelCenterOffset_u = -1.0;
        public double WheelScaleFactor_u_per_cts = -2.0/(0xFFF);
        #endregion

        #region FFB configuration (will be per game in the future)

        public bool SkipStopEffect = false;
        public bool UseTrqEmulationForMissing = true;
        public bool UsePulseSeq = true;
        public bool ForceTrqForAllCommands = false;

        public double GlobalGain = 1.0;
        public double PowerLaw = 1.2;
        public double TrqDeadBand = 0.0;

        public double Spring_Kp = 0.0;
        public double Spring_Kd = 0.0;
        public double Spring_Ki = 0.0;
        public double Spring_Damp_Kd = 0.0;

        public double Damper_Bv = 0.0;
        public double Damper_J = 0.0;

        public double Friction_Bv = 0.0;
        #endregion

        #region vJoy configuration and effects (will be per game)
        public int AutoFirePeriod_ms;
        public List<RawAxisDB> RawAxisTovJoyDB;
        public List<RawInputDB> RawInputTovJoyMap;
        #endregion


        #region Name of known games and respective set of parameters
        public List<string> GameListXMLFile;
        public List<string> FFBparamXMLFile;
        #endregion


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
