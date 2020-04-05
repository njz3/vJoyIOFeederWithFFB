using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class FFBParamsDB :
        ICloneable
    {
        public bool SkipStopEffect = false;
        public bool UseTrqEmulationForMissing = true;
        public bool UsePulseSeq = true;
        public bool ForceTrqForAllCommands = false;

        public bool DirectionUseSignedMagnitude = false;

        public double GlobalGain = 1.0;
        public double PowerLaw = 1.2;
        public double TrqDeadBand = 0.0;

        /// <summary>
        /// Minimum velocity threshold deadband
        /// </summary>
        public double MinVelThreshold = 0.2;
        /// <summary>
        /// Minimum acceleration threshold deadband
        /// </summary>
        public double MinAccelThreshold = 0.1;


        public double Spring_Deadband = 0.05;
        public double Spring_Kp = 0.0;
        public double Spring_Bv = 0.0;

        public double Damper_Bv = 0.0;
        public double Damper_J = 0.0;

        public double Friction_Bv = 0.0;

        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<FFBParamsDB>(this);
            return obj;
        }
    }

    [Serializable]
    public class vJoyMappingDB :
        ICloneable
    {
        public int AutoFirePeriod_ms;
        public List<RawAxisDB> RawAxisTovJoyDB;
        public List<RawInputDB> RawInputTovJoyMap;

        public vJoyMappingDB()
        {
            RawAxisTovJoyDB = new List<RawAxisDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_AXES_VJOY);
            RawInputTovJoyMap = new List<RawInputDB>(vJoyIOFeederAPI.vJoyFeeder.MAX_BUTTONS_VJOY);
        }

        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<vJoyMappingDB>(this);
            return obj;
        }
    }
    public enum ExecTypes : int
    {
        NATIVE_WINDOWS = 0,
        NEBULA_MODEL2,
        SUPERMODEL3,
        MAME,
        TECKNOPARROT,
    }

    [Serializable]
    public class ProcessDescriptorDB :
        ICloneable
    {
        public ExecTypes ExecType = ExecTypes.NATIVE_WINDOWS;
        public string ProcessName = "";
        public string MainWindowTitle = "";
        public List<string> AddressesValues = new List<string>();
        public List<string> MatchingWordValues = new List<string>();

        public ProcessDescriptorDB()
        {

        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<ProcessDescriptorDB>(this);
            return obj;
        }
    }


    [Serializable]
    public class ControlSetDB :
        ICloneable
    {
        public string UniqueName = "";
        public string GameName = "";
        public FFBParamsDB FFBParams = new FFBParamsDB();
        public vJoyMappingDB vJoyMapping = new vJoyMappingDB();
        public ProcessDescriptorDB ProcessDescriptor = new ProcessDescriptorDB();

        public ControlSetDB()
        {
        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<ControlSetDB>(this);
            obj.FFBParams = (FFBParamsDB)this.FFBParams.Clone();
            obj.vJoyMapping = (vJoyMappingDB)this.vJoyMapping.Clone();
            obj.ProcessDescriptor = (ProcessDescriptorDB)this.ProcessDescriptor.Clone();
            return obj;
        }
    }

    [Serializable]
    public class ControlSetsDB :
        ICloneable
    {
        public string DefaultControlSetName = "";
        public List<ControlSetDB> ControlSets;

        public ControlSetsDB()
        {
            ControlSets = new List<ControlSetDB>();
        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<ControlSetsDB>(this);
            return obj;
        }
    }

}
