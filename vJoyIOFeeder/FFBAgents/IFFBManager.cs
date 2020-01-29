//#define CONSOLE_DUMP

using System;
using System.Diagnostics;
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
        protected MultimediaTimer Timer;
        protected int RefreshPeriod_ms = 1;
        protected double Tick_per_s = 1.0;
        protected Stopwatch TimeoutTimer = new Stopwatch();

        /// <summary>
        /// Default base constructor
        /// </summary>
        public IFFBManager(int refreshPeriod_ms)
        {
            RefreshPeriod_ms = refreshPeriod_ms;
            Tick_per_s = 1000.0 / (double)RefreshPeriod_ms;
            Timer = new MultimediaTimer(refreshPeriod_ms);
            RunningEffect.Reset();
            NewEffect.Reset();
            
        }

        /// <summary>
        /// Start manager
        /// </summary>
        /// <returns></returns>
        public virtual void Start()
        {
            Timer.Handler = Timer_Handler;
            Timer.Start();
        }


        void Timer_Handler(object sender, MultimediaTimer.EventArgs e)
        {
            // Print time every 20 periods (100ms)
#if CONSOLE_DUMP
            if (((++counter) % 20) == 0) {
                //Console.WriteLine("At " + e.CurrentTime.TotalMilliseconds + " tick=" + e.Tick + " last time=" + e.LastExecutionTime.TotalMilliseconds + " elapsed=" + MultimediaTimer.RefTimer.Elapsed.TotalMilliseconds);
                Console.Write(e.CurrentTime.TotalMilliseconds + "\tpos=" + FiltPosition_u_0.ToString(CultureInfo.InvariantCulture));
                Console.Write("\tvel=" + FiltSpeed_u_per_s_0.ToString(CultureInfo.InvariantCulture));
                Console.Write("\tacc=" + FiltAccel_u_per_s2_0.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine();
            }
#endif
            if (e.OverrunOccured) {
#if CONSOLE_DUMP
                Console.WriteLine("Overrun occured");
#endif
                e.OverrunOccured = false;
            }

            // Process commands
            FFBEffectsStateMachine();
        }

        /// <summary>
        /// Stop manager
        /// </summary>
        /// <returns></returns>
        public virtual void Stop()
        {
            Timer.Stop();
        }

        /// <summary>
        /// Log with module name
        /// </summary>
        /// <param name="text"></param>
        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[FFBMANAGER] " + text, level);
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
                if (this._OutputEffectInternal!=value) {
                    Log("Changing DRVBD cmd from " + this._OutputEffectInternal.ToString("X04") + " to " + value.ToString("X04"), LogLevels.INFORMATIVE);
                }
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
        /// Internal states for force feedback effects generation.
        /// ORDER IS IMPORTANT!
        /// </summary>
        protected enum FFBStates : int
        {
            UNDEF = 0,

            // Device init/reset
            DEVICE_DISABLE,
            DEVICE_RESET,
            DEVICE_INIT,
            DEVICE_READY,

            // No effect
            NO_EFFECT,
            // Effects running
            CONSTANT_TORQUE,
            RAMP,

            SPRING,
            DAMPER,
            FRICTION,
            INERTIA,
            // Periodic
            SQUARE,
            SINE,
            TRIANGLE,
            SAWTOOTHUP,
            SAWTOOTHDOWN,
        }


        protected FFBStates State;
        protected FFBStates PrevState;
        protected int Step = 0;
        protected int PrevStep = 0;

        public bool IsDeviceReady {
            get {
                return (State>=FFBStates.DEVICE_READY);
            }
        }

        public bool IsEffectRunning {
            get {
                return (State>FFBStates.NO_EFFECT);
            }
        }

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

            // ...

            
            // Now save output results in memory protected variables
            OutputTorqueLevel = RunningEffect.GlobalGain * Trq;
            OutputEffectCommand = 0x0;

        }

        /// <summary>
        /// Transition to another state and reset step counter
        /// </summary>
        /// <param name="newstate"></param>
        protected virtual void TransitionTo(FFBStates newstate)
        {
            this.PrevState = this.State;
            this.State = newstate;
            this.PrevStep = this.Step;
            this.Step = 0;
            Log("[" + this.PrevState.ToString() + "] step " + this.PrevStep + "\tto [" + newstate.ToString() + "] step " + this.Step);
        }
        /// <summary>
        /// Go to next step.
        /// Increase step by +1 or anything else (-1, +2, ...).
        /// </summary>
        /// <param name="skip"></param>
        protected virtual void GoToNextStep(int skip = 1)
        {
            this.PrevStep = this.Step;
            this.Step += skip;
            Log("[" + this.State.ToString() + "] step " + this.PrevStep + "\tto step " + this.Step);
        }
        #endregion

        #region Windows' force feedback effects parameters
        /// <summary>
        /// The 12 standard force feedback effects from Windows/HID protocol
        /// </summary>
        public enum FFBType : int
        {
            // No effect
            NO_EFFECT = 0,
            // Effects running
            CONSTANT,
            RAMP,

            SPRING,
            DAMPER,
            FRICTION,
            INERTIA,
            // Periodic
            SQUARE,
            SINE,
            TRIANGLE,
            SAWTOOTHUP,
            SAWTOOTHDOWN,
        }

        /// <summary>
        /// Effect's parameters.
        /// Depending on the effect, only some of these parameters are used.
        /// </summary>
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
            /// <summary>
            /// Between 0 and 1.0
            /// </summary>
            public double Magnitude;

            public double RampStartLevel_u;
            public double RampEndLevel_u;

            public double EnvAttackTime_ms;
            public double EnvAttackLevel_u;
            public double EnvFadeTime_ms;
            public double EnvFadeLevel_u;

            // For periodic effects
            public double Period_ms;
            public double PhaseShift_deg;

            public double Offset_u;
            public double Deadband_u;

            public double PositiveCoef_u;
            public double NegativeCoef_u;
            public double PositiveSat_u;
            public double NegativeSat_u;

            public void Reset()
            {
                Type = FFBType.NO_EFFECT;
                Direction_deg = 0.0;
                Duration_ms = -1.0;
                GlobalGain = 1.0;

                Magnitude = 0.0;

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
            Log("FFB set duration " + duration_ms + " ms (-1=infinite)");
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
            NewEffect.Magnitude = magnitude;
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
        public virtual void SetEnveloppeParams(double attacktime_ms, double attacklevel_u, double fadetime_ms, double fadelevel_u)
        {
            Log("FFB set enveloppe params attacktime=" + attacktime_ms + "ms attacklevel=" + attacklevel_u + " fadetime=" + fadetime_ms + "ms fadelevel=" + fadelevel_u);

            // Prepare data
            NewEffect.EnvAttackTime_ms = attacktime_ms;
            NewEffect.EnvAttackLevel_u = attacklevel_u;
            NewEffect.EnvFadeTime_ms = fadetime_ms;
            NewEffect.EnvFadeLevel_u = fadelevel_u;
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

        public virtual void SetPeriodicParams(double magnitude_u, double offset_u, double phaseshift_deg, double period_ms)
        {
            Log("FFB set periodic params magnitude=" + magnitude_u + " offset=" + offset_u + " phase= " + phaseshift_deg + " period=" + period_ms + "ms");

            // Prepare data
            NewEffect.Magnitude = magnitude_u;
            NewEffect.Offset_u = offset_u;
            NewEffect.PhaseShift_deg = phaseshift_deg;
            NewEffect.Period_ms = period_ms;
        }




        public virtual void SetEffect(FFBType type)
        {
            Log("FFB set " + type.ToString() + " Effect");
            NewEffect.Type = type;
        }

        public virtual void StartEffect()
        {
            Log("FFB Got start effect");
            if (!this.IsDeviceReady) {
                Log("Device not yet ready");
                return;
            }
            // Copy configured effect parameters
            NewEffect.CopyTo(ref RunningEffect);

            RunningEffect._LocalTime_ms = 0;
            // Switch to
            switch (RunningEffect.Type) {
                case FFBType.NO_EFFECT:
                    if (this.State != FFBStates.NO_EFFECT)
                        TransitionTo(FFBStates.NO_EFFECT);
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
                // Periodic:
                case FFBType.SQUARE:
                    if (this.State != FFBStates.SQUARE)
                        TransitionTo(FFBStates.SQUARE);
                    break;
                case FFBType.SINE:
                    if (this.State != FFBStates.SINE)
                        TransitionTo(FFBStates.SINE);
                    break;
                case FFBType.TRIANGLE:
                    if (this.State != FFBStates.TRIANGLE)
                        TransitionTo(FFBStates.TRIANGLE);
                    break;
                case FFBType.SAWTOOTHUP:
                    if (this.State != FFBStates.SAWTOOTHUP)
                        TransitionTo(FFBStates.SAWTOOTHUP);
                    break;
                case FFBType.SAWTOOTHDOWN:
                    if (this.State != FFBStates.SAWTOOTHDOWN)
                        TransitionTo(FFBStates.SAWTOOTHDOWN);
                    break;

                default:
                    Log("Unmanaged effect " + RunningEffect.Type.ToString());
                    break;
            }

        }

        public virtual void StopEffect()
        {
            Log("FFB Got stop effect");

            if (this.IsEffectRunning) {
                if (this.State != FFBStates.NO_EFFECT)
                    TransitionTo(FFBStates.NO_EFFECT);
            }
        }

        public virtual void ResetEffect()
        {
            Log("FFB Got reset effect");

            if (this.IsEffectRunning)
                StopEffect();
            NewEffect.Reset();
        }

        public virtual void DevReset()
        {
            Log("FFB Got device reset");

            // Switch to
            if (this.State != FFBStates.DEVICE_RESET)
                TransitionTo(FFBStates.DEVICE_RESET);
        }

        public virtual void DevEnable()
        {
            Log("FFB Got device enable");

            // Switch to
            if (this.State != FFBStates.DEVICE_INIT)
                TransitionTo(FFBStates.DEVICE_INIT);
        }
        public virtual void DevDisable()
        {
            Log("FFB Got device disable");
            // Switch to
            if (this.State != FFBStates.DEVICE_DISABLE)
                TransitionTo(FFBStates.DEVICE_DISABLE);
        }
        #endregion

    }
}


