using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public FeederDB()
        {
            TranslatingModes = FFBTranslatingModes.MODEL3_SCUD_DRVBD;
            AxisDB = new List<vJoyAxisDB>();
            GameListXMLFile = new List<string>();
            FFBparamXMLFile = new List<string>();
        }
    }
}
