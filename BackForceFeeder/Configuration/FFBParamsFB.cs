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
        /// Safe range: 0-1.0
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



}
