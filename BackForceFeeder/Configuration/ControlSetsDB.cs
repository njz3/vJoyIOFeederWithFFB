using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Configuration
{
    [Serializable]
    public class FFBParamsDB :
        ICloneable
    {
        public bool AllowEffectsTuning = false;

        public bool SkipStopEffect = false;
        public bool UseTrqEmulationForMissing = true;
        public bool UsePulseSeq = true;
        public bool ForceTrqForAllCommands = true;

        public bool DirectionUseSignedMagnitude = false;

        public double GlobalGain = 1.0;
        public double PowerLaw = 1.2;
        public double TrqDeadBand = 0.0;

        /// <summary>
        /// Minimum velocity threshold deadband
        /// Safe range: 0.01-10.0
        /// </summary>
        public double MinVelThreshold = 0.2;
        /// <summary>
        /// Minimum acceleration threshold deadband
        /// Safe range: 0.01-10.0
        /// </summary>
        public double MinAccelThreshold = 0.1;
        /// <summary>
        /// Minimal Damper effect for all active effects ?
        /// Safe range: 0-0.5
        /// </summary>
        public double MinDamperForActive = 0.1;
        /// <summary>
        /// Permanent spring effect ?
        /// Safe range: 0-0.5
        /// </summary>
        public double PermanentSpring = 0.0;
        /// <summary>
        /// Safe range: 0-0.5
        /// </summary>
        public double Spring_TrqDeadband = 0.00;
        /// <summary>
        /// Safe range: 0.01-2.0
        /// </summary>
        public double Spring_Kp = 1.0;
        /// <summary>
        /// Safe range: 0-0.5
        /// </summary>
        public double Spring_Bv = 0.1;
        /// <summary>
        /// Safe range: 0-0.005
        /// </summary>
        public double Spring_Ki = 0.0;

        /// <summary>
        /// Safe range: 0-0.4
        /// </summary>
        public double Damper_Bv = 0.05;
        /// <summary>
        /// Safe range 0-0.2
        /// </summary>
        public double Damper_J = 0.03;

        /// <summary>
        /// Safe range 0-0.5
        /// </summary>
        public double Friction_Bv = 0.1;

        /// <summary>
        /// Safe range: 0-0.25
        /// </summary>
        public double Inertia_Bv = 0.05;
        /// <summary>
        /// Safe range 0-0.5
        /// </summary>
        public double Inertia_BvRaw = 0.1;
        /// <summary>
        /// Safe range 0-1.0
        /// </summary>
        public double Inertia_J = 0.5;

        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<FFBParamsDB>(this);
            return obj;
        }
    }

    [Serializable]
    public class vJoyButtonsDB :
        ICloneable
    {
        public int AutoFirePeriod_ms = 100;
        public int UpDownDelay_ms = 300;

        public vJoyButtonsDB()
        {
        }

        public object Clone()
        {
            var obj = Utils.Files.DeepCopy<vJoyButtonsDB>(this);
            return obj;
        }
    }

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

    /// <summary>
    /// One control set
    /// </summary>
    [Serializable]
    public class ControlSetDB :
        ICloneable
    {
        public string UniqueName = "";
        public string GameName = "";
        public PriorityLevels PriorityLevel = 0;
        public ProcessDescriptorDB ProcessDescriptor = new ProcessDescriptorDB();
        
        public FFBParamsDB FFBParams = new FFBParamsDB();
        
        
//        public List<RawInputDB> RawInputDBs = new List<RawInputDB>(16);
        public List<RawAxisDB> RawAxisDBs = new List<RawAxisDB>(8);
        public List<RawInputDB> RawInputDBs = new List<RawInputDB>(32);
        public List<RawOutputDB> RawOutputDBs = new List<RawOutputDB>(16);
        public List<KeyStrokeDB> KeyStrokeDBs = new List<KeyStrokeDB>();

        public vJoyButtonsDB vJoyButtonsDB = new vJoyButtonsDB();
        
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
