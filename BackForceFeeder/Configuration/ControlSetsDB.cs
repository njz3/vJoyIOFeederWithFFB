using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Configuration
{
    
    /// <summary>
    /// One control set
    /// </summary>
    [Serializable]
    public class ControlSetDB :
        ICloneable
    {
        public string UniqueName = "";
        
        public ProcessDescriptorDB ProcessDescriptor = new ProcessDescriptorDB();
        
        public FFBParamsDB FFBParams = new FFBParamsDB();
        
        
        public List<RawAxisDB> RawAxisDBs = new List<RawAxisDB>(8);
        public List<RawInputDB> RawInputDBs = new List<RawInputDB>(32);
        public vJoyButtonsDB vJoyButtonsDB = new vJoyButtonsDB();

        public List<RawOutputDB> RawOutputDBs = new List<RawOutputDB>(16);
        public List<KeyStrokeDB> KeyStrokeDBs = new List<KeyStrokeDB>();

        
        public ControlSetDB()
        { }

        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<ControlSetDB>(this);
            return obj;
        }
    }

    /// <summary>
    /// All control sets
    /// </summary>
    [Serializable]
    public class ControlSetsDB :
        ICloneable
    {
        public List<ControlSetDB> ControlSets;

        public ControlSetsDB()
        {
            ControlSets = new List<ControlSetDB>();
        }
        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<ControlSetsDB>(this);
            return obj;
        }
    }

}
