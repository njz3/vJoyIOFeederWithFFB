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
    public class ApplicationDB
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

        #region Name of known games and respective set of parameters
        public string GameFileDirectory;
        public List<string> GameFilesList;
        #endregion


        /// <summary>
        /// Default values
        /// </summary>
        public ApplicationDB()
        {
            GameFilesList = new List<string>();
        }
    }
}
