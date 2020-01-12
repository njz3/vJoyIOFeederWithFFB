using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class ProcessFindDB
    {
        public string ProcessName;
        public string[] AddressesValues;
        public string[] MatchingWordValues;
    }
    [Serializable]
    public class vJoyDB
    {
        public double AxeXMin;
        public double AxeYMin;
    }
    [Serializable]
    public class GameDB
    { 
        public ProcessFindDB Process;
        public vJoyDB vJoyParams;
    }

    [Serializable]
    public class GameListDB
    {
        public GameDB[] Games;
    }
}
