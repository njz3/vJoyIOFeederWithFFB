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
        public string[] IOBoardsXMLFile;
        public string[] GameListXMLFile;
        public string[] FFBparamXMLFile;

        public FFBTranslatingModes TranslatingModes;
    }
}
