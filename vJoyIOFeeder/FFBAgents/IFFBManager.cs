//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.FFBAgents
{
    /// <summary>
    /// All "wheel" units are expressed between -1.0/+1.0, 0 being the center position.
    /// All FFB values (except direction) are usually between -1.0/+1.0 which are
    /// scaled value originally between -10000/+10000.
    /// When possible, time units are in [s].
    /// </summary>
    public abstract class IFFBManager
    {
        #region Constructor/start/stop/log
        /// <summary>
        /// Default base constructor
        /// </summary>
        public IFFBManager() { }

        /// <summary>
        /// Start manager
        /// </summary>
        /// <returns></returns>
        public virtual void Start() { }

        /// <summary>
        /// Stop manager
        /// </summary>
        /// <returns></returns>
        public virtual void Stop() { }

        /// <summary>
        /// Log with module name
        /// </summary>
        /// <param name="text"></param>
        protected void Log(string text)
        {
            Logger.Log("[FFBMANAGER] " + text, LogLevels.DEBUG);
        }
        /// <summary>
        /// Logformat with module name
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        /// <param name="args"></param>
        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[FFBMANAGER] " + text, args);
        }
        #endregion

        #region Memory barrier mechanism for concurrent access
        /// <summary>
        /// Lock for concurrent "write" access to memory
        /// </summary>
        volatile int _concurrentlock = 0;
        protected void EnterBarrier()
        {
            var spin = new SpinWait();
            while (true) {
                if (Interlocked.Exchange(ref _concurrentlock, 1) == 0)
                    break;
                spin.SpinOnce();
            }
        }

        protected void ExitBarrier()
        {
            Interlocked.Exchange(ref _concurrentlock, 0);
        }
        #endregion

        #region Torque/Force effect
        /// <summary>
        /// No units, but yet it is -1/+1.
        /// Positive = turn left
        /// Negative = turn right
        /// => some application gives a constant force
        /// that is inverse of position sensor, check your
        /// wiring or negate this value!
        /// </summary>
        public double OutputTorqueLevel {
            get {
                EnterBarrier();
                double val = _OutputTorqueLevelInternal;
                ExitBarrier();
                return val;
            }
            protected set {
                EnterBarrier();
                this._OutputTorqueLevelInternal = value;
                ExitBarrier();
            }
        }
        protected double _OutputTorqueLevelInternal;


        /// <summary>
        /// Force effect, no units.
        /// Currently, it is just an Hex code encoding an effect
        /// for Sega Model 3 drive board.
        /// Example from BigPanik's website:
        /// - Rotate wheel right – SendConstantForce (+)
        ///   0x50: Disable - 0x51 = weakest - 0x5F = strongest
        /// </summary>
        public Int64 OutputEffectCommand {
            get {
                EnterBarrier();
                var val = _OutputEffectInternal;
                ExitBarrier();
                return val;
            }
            protected set {
                EnterBarrier();
                this._OutputEffectInternal = value;
                ExitBarrier();
            }
        }
        protected Int64 _OutputEffectInternal;
        #endregion

        #region Feedack signal for position/vel/acc
        /// <summary>
        /// Position are between -1 .. 1. Center is 0.
        /// </summary>
        protected double RefPosition_u = 0.0;

        /// <summary>
        /// Position are between -1 .. 1. Center is 0.
        /// </summary>
        protected double RawPosition_u = 0.0;
        protected double FiltPosition_u_0 = 0.0;
        protected double FiltPosition_u_1 = 0.0;
        protected double FiltPosition_u_2 = 0.0;

        protected double RawSpeed_u_per_s = 0.0;
        protected double FiltSpeed_u_per_s_0 = 0.0;
        protected double FiltSpeed_u_per_s_1 = 0.0;

        protected double RawAccel_u_per_s2_0 = 0.0;
        protected double FiltAccel_u_per_s2_0 = 0.0;

        protected double Inertia = 0.1;
        protected double LastTimeRefresh_ms = 0.0;

        protected const double MinVelThreshold = 0.25f;
        protected const double MinAccThreshold = 0.25f;
        

        /// <summary>
        /// Values should be refresh periodically and as soon as they're
        /// received from the digitizer/converter.
        /// Velocity and acceleration are computed based on internal clock
        /// if they're not given by the hardware (see alternative function
        /// below)
        /// </summary>
        /// <param name="pos_u"></param>
        public virtual void RefreshCurrentPosition(double pos_u)
        {
            // Got a new position from outside!
            // Put a timestamp and use it for estimation
            var now_ms = MultimediaTimer.RefTimer.Elapsed.TotalMilliseconds;
            var span_s = (now_ms - LastTimeRefresh_ms) * 0.001;

            // Lock memory
            EnterBarrier();
            // Compute raw/filtered values - should better use backware euler
            // or an observer like a kalman filter
            RawPosition_u = pos_u;

            // Strong filtering when estimation is done on the PC

            // Smoothing average filter on 3 samples
            FiltPosition_u_2 = FiltPosition_u_1;
            FiltPosition_u_1 = FiltPosition_u_0;
            FiltPosition_u_0 = 0.2 * RawPosition_u + 0.4 * FiltPosition_u_1 + 0.4 * FiltPosition_u_2;

            RawSpeed_u_per_s = (FiltPosition_u_0 - FiltPosition_u_1) / span_s;
            FiltSpeed_u_per_s_1 = FiltSpeed_u_per_s_0;
            FiltSpeed_u_per_s_0 = 0.2 * RawSpeed_u_per_s + 0.8 * FiltSpeed_u_per_s_1;

            RawAccel_u_per_s2_0 = (FiltSpeed_u_per_s_0 - FiltSpeed_u_per_s_1) / span_s;
            FiltAccel_u_per_s2_0 = 0.2 * RawAccel_u_per_s2_0 + 0.8 * FiltAccel_u_per_s2_0;

            LastTimeRefresh_ms = now_ms;

            // Release the lock
            ExitBarrier();
        }

        /// <summary>
        /// Same as before, but hardware performs vel/accel calculations
        /// </summary>
        /// <param name="pos_u"></param>
        /// <param name="vel_u_per_s"></param>
        /// <param name="accel_u_per_s2"></param>
        public virtual void RefreshCurrentState(double pos_u, double vel_u_per_s, double accel_u_per_s2)
        {
            // Got a new position from outside!
            // Put a timestamp and use it for estimation
            var now_ms = MultimediaTimer.RefTimer.Elapsed.TotalMilliseconds;
            var span_s = (now_ms - LastTimeRefresh_ms) * 0.001;

            var newspeed_u_per_s = vel_u_per_s;
            var newaccel_u_per_s2 = accel_u_per_s2;

            // Lock memory
            EnterBarrier();

            // Light filtering when estimation is done on the IO board
            RawPosition_u = pos_u;

            // Smoothing average filter on 3 samples
            FiltPosition_u_2 = FiltPosition_u_1;
            FiltPosition_u_1 = FiltPosition_u_0;
            FiltPosition_u_0 = 0.6 * RawPosition_u + 0.3 * FiltPosition_u_1 + 0.1 * FiltPosition_u_2;

            RawSpeed_u_per_s = vel_u_per_s;
            FiltSpeed_u_per_s_1 = FiltSpeed_u_per_s_0;
            FiltSpeed_u_per_s_0 = 0.5 * RawSpeed_u_per_s + 0.5 * FiltSpeed_u_per_s_1;

            RawAccel_u_per_s2_0 = accel_u_per_s2;
            FiltAccel_u_per_s2_0 = 0.5 * RawAccel_u_per_s2_0 + 0.5 * FiltAccel_u_per_s2_0;

            LastTimeRefresh_ms = now_ms;

            // Release the lock
            ExitBarrier();
        }
        #endregion


        #region State machine management
        /// <summary>
        /// Internal states for force feedback effects generation
        /// </summary>
        protected enum FFBStates : int
        {
            NOP = 0,
            CONSTANT_TORQUE,
            RAMP,
            SPRING,
            DAMPER,
            FRICTION,
            INERTIA,
        }
        protected FFBStates State;
        protected FFBStates PrevState;
        protected int Step = 0;
        protected int PrevStep = 0;


        /// <summary>
        /// Taken from MMos firmware explanations:
        /// - Spring: is a force opposing the wheel rotation. Spring torque 
        ///   is proportional to wheel position (zero at center, max at max rotation angle)
        /// - Damper: is like a viscous force that "smooth" the rotation of the wheel,
        ///   so much probably is proportional so rotational speed(faster you try to rotate
        ///   much damping you feel).
        /// - Friction: should be a constant force opposing the rotation.
        /// - Inertia: is a force opposing the start of rotation, so it is max when you
        ///   start rotating and then decrease to zero.
        /// </summary>
        protected virtual void FFBEffectsStateMachine()
        {
            
            // Execute current effect every period of time

            // Take a snapshot of all values - convert time base to period
            EnterBarrier();
            double R = RefPosition_u + RunningEffect.Offset_u;
            double P = FiltPosition_u_0;
            double W = FiltSpeed_u_per_s_0;
            double A = FiltAccel_u_per_s2_0;
            double Trq = 0.0;
            // Release the lock
            ExitBarrier();

            // Inputs:
            // - R=angular reference
            // - P=angular position
            // - W=angular velocity (W=dot(P)=dP/dt)
            // - A=angular accel (A=dot(W)=dW/dt=d2P/dt2)
            // output:
            // - Trq = force/torque level

            // Memory protected variables
            OutputTorqueLevel = RunningEffect.GlobalGain * Trq;
            OutputEffectCommand = 0x0;

        }

       
        protected virtual void TransitionTo(FFBStates newstate)
        {
            this.PrevState = this.State;
            this.State = newstate;
            this.PrevStep = this.Step;
            this.Step = 0;
            Log("[" + this.PrevState.ToString() + "] step " + this.PrevStep + "\tto [" + newstate.ToString() + "] step " + this.Step);
        }
        #endregion

        #region Windows' force feedback effects parameters

        public enum FFBType : int
        {
            NOP = 0,
            CONSTANT,
            RAMP,
            SPRING,
            DAMPER,
            FRICTION,
            INERTIA,
        }

        public struct EffectParams
        {
            public double _LocalTime_ms;

            public FFBType Type;
            public double Direction_deg;

            public double Duration_ms;
            /// <summary>
            /// Between 0 and 1.0
            /// </summary>
            public double GlobalGain;

            public double ConstantTorqueMagnitude;

            public double RampStartLevel_u;
            public double RampEndLevel_u;

            public double EnvAttackTime_ms;
            public double EnvAttackLevel_u;
            public double EnvFadeTime_ms;
            public double EnvFadeLevel_u;

            public double Offset_u;
            public double Deadband_u;

            public double PositiveCoef_u;
            public double NegativeCoef_u;
            public double PositiveSat_u;
            public double NegativeSat_u;

            public void Reset()
            {
                Type = FFBType.NOP;
                Direction_deg = 0.0;
                Duration_ms = -1.0;
                GlobalGain = 1.0;

                ConstantTorqueMagnitude = 0.0;

                RampStartLevel_u = 0.0;
                RampEndLevel_u = 0.0;

                EnvAttackTime_ms = 0.0;

                _LocalTime_ms = 0.0;
            }

            public void CopyTo(ref EffectParams dest)
            {
                dest = (EffectParams)this.MemberwiseClone();
            }
        }


        protected EffectParams RunningEffect = new EffectParams();
        protected EffectParams NewEffect = new EffectParams();

        public virtual void SetDuration(double duration_ms)
        {
            Log("FFB set duration " + duration_ms + " ms");
            NewEffect.Duration_ms = duration_ms;
        }

        public virtual void SetDirection(double direction_deg)
        {
            Log("FFB set direction " + direction_deg + " deg");
            NewEffect.Direction_deg = direction_deg;
        }

        public virtual void SetConstantTorqueEffect(double magnitude)
        {
            Log("FFB set ConstantTorque magnitude " + magnitude);
            NewEffect.ConstantTorqueMagnitude = magnitude;
        }

        public virtual void SetRampParams(double startvalue_u, double endvalue_u)
        {
            Log("FFB set Ramp params " + startvalue_u + " " + endvalue_u);
            // Prepare data
            NewEffect.RampStartLevel_u = startvalue_u;
            NewEffect.RampEndLevel_u = endvalue_u;
        }
        /// <summary>
        /// Set global gain in percent (0/+1.0)
        /// </summary>
        /// <param name="gain_pct">[in] Global gain in percent</param>
        public virtual void SetGain(double gain_pct)
        {
            Log("FFB set global gain " + gain_pct);

            // Restrict range
            if (gain_pct < 0.0) gain_pct = 0.0;
            if (gain_pct > 1.0) gain_pct = 1.0;
            // save gain
            NewEffect.GlobalGain = gain_pct;
        }
        public virtual void SetEnveloppeParams(double attacktime_ms, double attacklevel_Nm, double fadetime_ms, double fadelevel_Nm)
        {
            Log("FFB set enveloppe params " + attacktime_ms + "ms " + attacklevel_Nm + " " + fadetime_ms + "ms " + fadelevel_Nm);

            // Prepare data
            NewEffect.EnvAttackTime_ms = attacktime_ms;
            NewEffect.EnvAttackLevel_u = attacklevel_Nm;
            NewEffect.EnvFadeTime_ms = fadetime_ms;
            NewEffect.EnvFadeLevel_u = fadelevel_Nm;
        }

        public virtual void SetLimitsParams(double offset_u, double deadband_u,
            double poscoef_u, double negcoef_u, double poslim_u, double neglim_u)
        {
            Log("FFB set limits offset=" + offset_u + " deadband=" + deadband_u
                + " PosCoef=" + poscoef_u + " NegCoef=" + negcoef_u + " sat=[" + neglim_u
                + "; " + poslim_u + "]");

            // Prepare data
            NewEffect.Deadband_u = deadband_u;
            NewEffect.Offset_u = offset_u;
            NewEffect.PositiveCoef_u = poscoef_u;
            NewEffect.NegativeCoef_u = negcoef_u;
            NewEffect.PositiveSat_u = poslim_u;
            NewEffect.NegativeSat_u = neglim_u;
        }


        public virtual void SetEffect(FFBType type)
        {
            Log("FFB set " + type.ToString() + " Effect");
            NewEffect.Type = type;
        }

        public virtual void StartEffect()
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB Got start effect");
#endif
            // Copy configured effect parameters
            NewEffect.CopyTo(ref RunningEffect);

            RunningEffect._LocalTime_ms = 0;
            // Switch to
            switch (RunningEffect.Type) {
                case FFBType.NOP:
                    if (this.State != FFBStates.NOP)
                        TransitionTo(FFBStates.NOP);
                    break;
                case FFBType.CONSTANT:
                    if (this.State != FFBStates.CONSTANT_TORQUE)
                        TransitionTo(FFBStates.CONSTANT_TORQUE);
                    break;
                case FFBType.RAMP:
                    if (this.State != FFBStates.RAMP)
                        TransitionTo(FFBStates.RAMP);
                    break;
                case FFBType.INERTIA:
                    if (this.State != FFBStates.INERTIA)
                        TransitionTo(FFBStates.INERTIA);
                    break;
                case FFBType.SPRING:
                    // For string effect, reference position is 0 (center) + given offset
                    RefPosition_u = 0.0;
                    if (this.State != FFBStates.SPRING)
                        TransitionTo(FFBStates.SPRING);
                    break;
                case FFBType.DAMPER:
                    if (this.State != FFBStates.DAMPER)
                        TransitionTo(FFBStates.DAMPER);
                    break;
                case FFBType.FRICTION:
                    if (this.State != FFBStates.FRICTION)
                        TransitionTo(FFBStates.FRICTION);
                    break;
            }

        }

        public virtual void StopEffect()
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB Got stop effect");
#endif
            // Switch to
            if (this.State != FFBStates.NOP)
                TransitionTo(FFBStates.NOP);
        }

        public virtual void ResetEffect()
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB Got device reset");
#endif
            StopEffect();
            NewEffect.Reset();
        }
        #endregion

    }
}


