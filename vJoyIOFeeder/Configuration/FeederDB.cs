using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class FeederDB
    {
        public string[] IOBoardsXML;
        public string GameListXML;
        public string FFBparamXML;
    }
}
