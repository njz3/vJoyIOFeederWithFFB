using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Configuration
{

    public enum PriorityLevels : int
    {
        Lowest = -2,
        Lower = -1,
        Normal = 0,
        Higher = 1,
        Highest = 2,
    }
    public enum ExecTypes : int
    {
        NATIVE_WINDOWS = 0,
        NEBULA_MODEL2,
        SUPERMODEL3,
        MAME,
        TECKNOPARROT,
    }

    public enum OutputTypes : int
    {
        NONE = 0,
        RAW_MEMORY_READ,
        MAME_WIN,
        MAME_NET,
    }

    /// <summary>
    /// Process scanner
    /// </summary>
    [Serializable]
    public class ProcessDescriptorDB :
        ICloneable
    {
        public ExecTypes ExecType = ExecTypes.NATIVE_WINDOWS;
        public OutputTypes OutputType = OutputTypes.NONE;
        public string ProcessName = "";
        public string MainWindowTitle = "";
        public List<string> AddressesValues = new List<string>();
        public List<string> MatchingWordValues = new List<string>();

        public ProcessDescriptorDB()
        {

        }
        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<ProcessDescriptorDB>(this);
            return obj;
        }
    }

}
