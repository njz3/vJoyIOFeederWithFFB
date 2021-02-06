using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackForceFeeder.Utils;
using BackForceFeeder.BackForceFeeder;

namespace BackForceFeeder.Configuration
{
    [Serializable]
    public class ApplicationDB
    {

        #region Software
        public bool StartMinimized = false;
        public bool ShortcutStartWithWindowsCreated = false;
        public bool OutputOnly = false;
        public bool RunWithoutIOBoard = false;

        public LogLevels LogLevel = LogLevels.INFORMATIVE;
        public bool DumpLogToFile = false;
        public bool DebugModeGUI = false;
        public bool VerbosevJoyManager = false;
        public bool VerboseScanner = false;
        public bool VerboseFFBManager = false;
        public bool VerboseFFBManagerTorqueValues = false;
        public bool VerbosevJoyFFBReceiver = false;
        public bool VerbosevJoyFFBReceiverDumpFrames = false;
        public bool VerboseSerialIO = false;
        public bool VerboseSerialIODebugMode = false;
        public bool VerboseSerialIODumpFrames = false;
        #endregion

        #region Name of known games and respective set of parameters
        public bool AutodetectControlSetAtRuntime = false;
        public bool ResetFFBOnControlSetChange = true;
        public string DefaultControlSetName = "";
        public string LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackForceFeeder", "Logs");
        public string ControlSetsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackForceFeeder", "ControlSets");
        public List<string> ControlSetsList;
        #endregion


        /// <summary>
        /// Default values
        /// </summary>
        public ApplicationDB()
        {
            ControlSetsList = new List<string>();
        }
    }
}
