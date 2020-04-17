using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyIOFeeder.Utils;
using static vJoyIOFeeder.vJoyManager;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class ApplicationDB
    {

        #region Software
        public bool StartMinimized = false;
        public bool ShortcutStartWithWindowsCreated = false;

        public LogLevels LogLevel = LogLevels.INFORMATIVE;
        public bool DumpLogToFile = false;
        public bool VerbosevJoyManager = false;
        public bool VerboseScanner = false;
        public bool VerboseFFBManager = false;
        public bool VerboseFFBManagerTorqueValues = false;
        public bool VerbosevJoyFFBReceiver = false;
        public bool VerbosevJoyFFBReceiverDumpFrames = false;
        public bool VerboseSerialIO = false;
        public bool VerboseSerialIODumpFrames = false;
        #endregion

        #region Name of known games and respective set of parameters
        public bool AutodetectControlSetAtRuntime = false;
        public string DefaultControlSetName = "";
        public string ControlSetsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "vJoyIOFeeder","ControlSets");
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
