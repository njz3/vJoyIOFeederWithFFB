//#define CONSOLE_DUMP
//#define VERBOSE

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;

// Don't forget to add this
using BackForceFeeder.Utils;

namespace BackForceFeeder.FFBManagers
{
    /// <summary>
    /// All "wheel" units are expressed between -1.0/+1.0, 0 being the center position.
    /// All FFB values (except direction) are usually between -1.0/+1.0 which are
    /// scaled value originally between -10000/+10000.
    /// When possible, time units are in [s].
    /// </summary>
    public abstract class FFBManager
    {
        #region Constructor/start/stop/log
        protected MultimediaTimer Timer;
        protected int RefreshPeriod_ms = 5;
        protected double Tick_per_s = 1.0;
        protected Stopwatch TimeoutTimer = new Stopwatch();

        /// <summary>
        /// +1 if positive torque command turn wheel left
        /// -1 if positive torque command turn wheel right
        /// </summary>
        public double TrqSign {
            get {
                if (BFFManager.Config.Hardware.InvertTrqDirection)
                    return -1.0;
                else
                    return 1.0;
            }
        }

        /// <summary>
        /// +1 if turning wheel left increments position value (= positive speed)
        /// -1 if turning wheel left decrements position value (= negative speed)
        /// </summary>
        public double WheelSign {
            get {
                if (BFFManager.Config.Hardware.InvertWheelDirection)
                    return -1.0;
                else
                    return 1.0;
            }
        }

        /// <summary>
        /// Device gain from FFB frame
        /// </summary>
        public double DeviceGain = 1.0;
        protected FFBParamsDB FFBParams { get { return BFFManager.CurrentControlSet.FFBParams; } }

        /// <summary>
        /// Global gain used by this application
        /// </summary>
        public double GlobalGain { get { return FFBParams.GlobalGain; } }

        /// <summary>
        /// Torque dead band used by this application
        /// </summary>
        public double TrqDeadBand { get { return FFBParams.TrqDeadBand; } }

        /// <summary>
        /// Some games like M2Emulator sends a lot of stop effects cmds...
        /// Filters them out using this flag.
        /// </summary>
        public bool SkipStopEffect { get { return FFBParams.SkipStopEffect; } }

        /// <summary>
        /// "Power law" on torque : this is to avoid some oscillations
        /// on some games (Daytona 2 for exemple) where small torque
        /// values can make great wheel motion, thus entering a limit
        /// cycle with left/right oscillations.
        /// OutputTrq = Trq^PowerLaw
        /// => if PowerLaw = 1.0, then outputtrq is equal to computed trq
        ///    if PowerLaw > 1.0, then outputtrq is smaller than trq
        ///    if PowerLaw < 1.0, then outputtrq is bigger than trq
        /// Recommanded value on Model 2/3: 1.2-1.5
        /// </summary>
        public double PowerLaw { get { return FFBParams.PowerLaw; } }

        /// <summary>
        /// Default base constructor
        /// </summary>
        public FFBManager(int refreshPeriod_ms)
        {
            RefreshPeriod_ms = refreshPeriod_ms;
            Tick_per_s = 1000.0 / (double)RefreshPeriod_ms;
            Timer = new MultimediaTimer(refreshPeriod_ms);
            for (int i = 0; i<RunningEffects.Length; i++) {
                RunningEffects[i] = new Effect();
                RunningEffects[i].Reset();
            }
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
                Log("Overrun occured", LogLevels.INFORMATIVE);
                e.OverrunOccured = false;
            }

            // Process commands
            DeviceStateMachine();
        }

        /// <summary>
        /// Stop manager
        /// </summary>
        /// <returns></returns>
        public virtual void Stop()
        {
            TransitionTo(FFBStates.UNDEF);
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

        public static FFBManager Factory(FFBTranslatingModes translatingMode, int refreshPeriod_ms)
        {
            FFBManager FFB = null;
            switch (translatingMode) {
                case FFBTranslatingModes.PWM_CENTERED:
                case FFBTranslatingModes.PWM_DIR: {
                        FFB = new FFBManagerTorque(refreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.COMP_M3_UNKNOWN: {
                        // Default to Scud/Daytona2
                        FFB = new FFBManagerModel3Scud(refreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.COMP_M2_INDY_STC:
                case FFBTranslatingModes.COMP_M3_LEMANS: {
                        FFB = new FFBManagerModel3Lemans(refreshPeriod_ms);
                    }
                    break;
                case FFBTranslatingModes.COMP_M3_SCUD: {
                        FFB = new FFBManagerModel3Scud(refreshPeriod_ms*2);
                    }
                    break;
                case FFBTranslatingModes.COMP_M3_SR2: {
                        FFB = new FFBManagerModel3SegaRally2(refreshPeriod_ms*2);
                    }
                    break;
                case FFBTranslatingModes.RAW_M2PAC_MODE: {
                        FFB = new FFBManagerRawModel23(refreshPeriod_ms);
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported FFB mode " + translatingMode.ToString());
            }
            return FFB;
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
                if (this._OutputTorqueLevelInternal!=value) {
                    if (BFFManager.Config.Application.VerboseFFBManagerTorqueValues) {
                        Log("Changing trq level from " + this._OutputTorqueLevelInternal.ToString() + " to " + value.ToString(), LogLevels.INFORMATIVE);
                    }
                }
                this._OutputTorqueLevelInternal = value;
                ExitBarrier();
            }
        }
        protected double _OutputTorqueLevelInternal = 0.0;


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
                    if (BFFManager.Config.Application.VerboseFFBManager) {
                        Log("Changing DRVBD cmd from " + this._OutputEffectInternal.ToString("X04") + " to " + value.ToString("X04"), LogLevels.INFORMATIVE);
                    }
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
        /// +1 should be wheel turned most left
        /// -1 should be wheel turned most right
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
        protected double FiltAccel_u_per_s2_1 = 0.0;

        protected double Inertia = 0.005;
        protected double LastTimeRefresh_ms = 0.0;


        double[] LastPosition_u = new double[2];
        double[] LastVelocity_u = new double[4];
        double[] LastAccel_u = new double[32];


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

            span_s = BFFManager.GlobalRefreshPeriod_ms * 0.001;

            for (int i = LastPosition_u.Length-1; i>0; i--) {
                LastPosition_u[i] = LastPosition_u[i-1];
            }
            for (int i = LastVelocity_u.Length-1; i>0; i--) {
                LastVelocity_u[i] = LastVelocity_u[i-1];
            }
            for (int i = LastAccel_u.Length-1; i>0; i--) {
                LastAccel_u[i] = LastAccel_u[i-1];
            }

            // Lock memory
            EnterBarrier();

            // Compute raw/filtered values - should better use backware euler
            // or an observer like a kalman filter
            RawPosition_u = this.WheelSign * pos_u;
            LastPosition_u[0] = RawPosition_u;
            RawSpeed_u_per_s = (LastPosition_u[0] - LastPosition_u[1]) / span_s;
            LastVelocity_u[0] = RawSpeed_u_per_s;
            RawAccel_u_per_s2_0 = (LastVelocity_u[0] - LastVelocity_u[1]) / span_s;
            LastAccel_u[0] = RawAccel_u_per_s2_0;

            FiltPosition_u_0 = 0.0;
            FiltSpeed_u_per_s_0 = 0.0;
            FiltAccel_u_per_s2_0 = 0.0;
            for (int i = 0; i<LastPosition_u.Length; i++) {
                FiltPosition_u_0 += LastPosition_u[i]*(1.0/LastPosition_u.Length);
            }
            for (int i = 0; i<LastVelocity_u.Length; i++) {
                FiltSpeed_u_per_s_0 += LastVelocity_u[i]*(1.0/LastVelocity_u.Length);
            }
            for (int i = 0; i<LastAccel_u.Length; i++) {
                FiltAccel_u_per_s2_0 += LastAccel_u[i]*(1.0/LastAccel_u.Length);
            }

            LastTimeRefresh_ms = now_ms;

            // Release the lock
            ExitBarrier();

            Console.WriteLine(FiltPosition_u_0 + " " + FiltSpeed_u_per_s_0  + " " + FiltAccel_u_per_s2_0);

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

            var newspeed_u_per_s = this.WheelSign *vel_u_per_s;
            var newaccel_u_per_s2 = this.WheelSign *accel_u_per_s2;

            for (int i = LastPosition_u.Length-1; i>0; i--) {
                LastPosition_u[i] = LastPosition_u[i-1];
            }
            for (int i = LastVelocity_u.Length-1; i>0; i--) {
                LastVelocity_u[i] = LastVelocity_u[i-1];
            }
            for (int i = LastAccel_u.Length-1; i>0; i--) {
                LastAccel_u[i] = LastAccel_u[i-1];
            }

            // Lock memory
            EnterBarrier();

            // Compute raw/filtered values - should better use backware euler
            // or an observer like a kalman filter
            RawPosition_u = this.WheelSign * pos_u;
            LastPosition_u[0] = RawPosition_u;
            RawSpeed_u_per_s = vel_u_per_s;
            LastVelocity_u[0] = RawSpeed_u_per_s;
            RawAccel_u_per_s2_0 = accel_u_per_s2;
            LastAccel_u[0] = RawAccel_u_per_s2_0;

            FiltPosition_u_0 = 0.0;
            FiltSpeed_u_per_s_0 = 0.0;
            FiltAccel_u_per_s2_0 = 0.0;
            for (int i = 0; i<LastPosition_u.Length; i++) {
                FiltPosition_u_0 += LastPosition_u[i]*(1.0/LastPosition_u.Length);
            }
            for (int i = 0; i<LastVelocity_u.Length; i++) {
                FiltSpeed_u_per_s_0 += LastVelocity_u[i]*(1.0/LastVelocity_u.Length);
            }
            for (int i = 0; i<LastAccel_u.Length; i++) {
                FiltAccel_u_per_s2_0 += LastAccel_u[i]*(1.0/LastAccel_u.Length);
            }

            LastTimeRefresh_ms = now_ms;

            // Release the lock
            ExitBarrier();

            //Console.WriteLine(FiltSpeed_u_per_s_0 + "\t" + FiltAccel_u_per_s2_0);
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
            DEVICE_INIT,        // Device is initializing after boot-up
            DEVICE_RESET,       // Reseting device memory
            DEVICE_DISABLE,     // Disabled, but can be switch to ready
            DEVICE_READY,       // Enabled and ready to play effects
            DEVICE_EFFECT_RUNNING, // Playing effects
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
                return (State>=FFBStates.DEVICE_EFFECT_RUNNING);
            }
        }

        /// <summary>
        /// When states are too complicated, a separate method is called.
        /// </summary>
        protected virtual void DeviceStateMachine()
        {
            switch (State) {
                case FFBStates.UNDEF:
                    TransitionTo(FFBStates.DEVICE_INIT);
                    break;
                case FFBStates.DEVICE_INIT:
                    State_INIT();
                    break;
                case FFBStates.DEVICE_RESET:
                    State_RESET();
                    break;
                case FFBStates.DEVICE_DISABLE:
                    State_DISABLE();
                    break;
                case FFBStates.DEVICE_READY:
                    State_READY();
                    break;
                case FFBStates.DEVICE_EFFECT_RUNNING:
                    ComputeTrqFromAllEffects();
                    break;
            }
        }

        protected virtual void State_INIT()
        {
            switch (Step) {
                case 0:
                    TransitionTo(FFBStates.DEVICE_RESET);
                    break;
            }
        }
        protected virtual void State_RESET()
        {
            switch (Step) {
                case 0:
                    ResetAllEffects();
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
            }
        }
        protected virtual void State_DISABLE()
        {
            switch (Step) {
                case 0:
                    break;
            }
        }
        protected virtual void State_READY()
        {
            switch (Step) {
                case 0:
                    // Set null torque&command
                    OutputTorqueLevel = 0.0;
                    OutputEffectCommand = 0x0;
                    break;
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
        protected virtual void ComputeTrqFromAllEffects()
        {
            // Execute every period of time

            // Inputs:
            // - R=angular reference
            // - P=angular position
            // - W=angular velocity (W=dot(P)=dP/dt)
            // - A=angular accel (A=dot(W)=dW/dt=d2P/dt2)
            // output:
            // - Trq = force/torque level

            // Take a snapshot of all values - convert time base to period
            EnterBarrier();
            double R = RefPosition_u;
            double P = FiltPosition_u_0;
            double W = FiltSpeed_u_per_s_0;
            double A = FiltAccel_u_per_s2_0;
            // Release the lock
            ExitBarrier();

            double Trq = 0.0;
            for (int i = 0; i<RunningEffects.Length; i++) {
                // ...
                //Trq += RunningEffects[i].GetTrqValue(R, P, W, A);
            }

            // Now save output results in memory protected variables
            OutputTorqueLevel = TrqSign * Math.Sign(Trq) * Math.Pow(Math.Abs(Trq), PowerLaw) * GlobalGain * DeviceGain;
            OutputEffectCommand = 0x0;

            CheckForEffectsDone();
        }

        protected virtual void CheckForEffectsDone()
        {
            bool alldone = true;
            for (int i = 0; i<RunningEffects.Length; i++) {
                if (RunningEffects[i].IsRunning && RunningEffects[i].Type != EffectTypes.NO_EFFECT) {
                    RunningEffects[i]._LocalTime_ms += Timer.Period_ms;
                    if (RunningEffects[i].Duration_ms >= 0.0 &&
                        RunningEffects[i]._LocalTime_ms > RunningEffects[i].Duration_ms) {
                        Log("Effect " + (i).ToString() + " duration reached");
                        RunningEffects[i].IsRunning = false;
                    }
                }
                // At least one effect running?
                if (RunningEffects[i].IsRunning) {
                    alldone = false;
                }
            }
            if (alldone) {
                Log("All effects done", LogLevels.INFORMATIVE);
                TransitionTo(FFBStates.DEVICE_READY);
            }
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
            Log("[" + this.PrevState.ToString() + "] step " + this.PrevStep + "\tto [" + newstate.ToString() + "] step " + this.Step, LogLevels.INFORMATIVE);
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
        /// <summary>
        /// Set global device gain in percent (0/+1.0)
        /// </summary>
        /// <param name="gain_pct">[in] Global gain in percent</param>
        public virtual void SetDeviceGain(double gain_pct)
        {
            Log("FFB set device gain " + gain_pct);

            // Restrict range
            if (gain_pct < 0.0) gain_pct = 0.0;
            if (gain_pct > 1.0) gain_pct = 1.0;
            // save gain and update current gain
            DeviceGain = gain_pct;
        }
        public virtual void DevReset()
        {
            Log("FFB Got device reset");
            if (this.State==FFBStates.DEVICE_INIT) {
                Log("Device cannot be reset yet", LogLevels.INFORMATIVE);
                return;
            }
            // Switch to
            if (this.State != FFBStates.DEVICE_RESET)
                TransitionTo(FFBStates.DEVICE_RESET);
        }

        public virtual void DevEnable()
        {
            Log("FFB Got device enable");
            if (this.State==FFBStates.DEVICE_INIT) {
                Log("Device cannot be enabled yet", LogLevels.INFORMATIVE);
                return;
            }

            // Switch to
            if (this.State != FFBStates.DEVICE_READY)
                TransitionTo(FFBStates.DEVICE_READY);
        }
        public virtual void DevDisable()
        {
            Log("FFB Got device disable");
            if (this.State==FFBStates.DEVICE_INIT) {
                Log("Device cannot be disabled yet", LogLevels.INFORMATIVE);
                return;
            }
            // Switch to
            if (this.State != FFBStates.DEVICE_DISABLE)
                TransitionTo(FFBStates.DEVICE_DISABLE);
        }
        #endregion

        #region Windows' force feedback effects parameters
        /// <summary>
        /// The 12 standard force feedback effects from Windows/HID protocol
        /// </summary>
        public enum EffectTypes : int
        {
            // No effect
            NO_EFFECT = 0,
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

        /// <summary>
        /// Effect's parameters.
        /// Depending on the effect, only some of these parameters are used.
        /// </summary>
        public struct Effect
        {
            /// <summary>
            /// Counter for elapsed time since start of the effect.
            /// It will be incremented every system tick.
            /// It can be negative. In that case, it means that the effect is 
            /// delayed until the effect will start.
            /// This value is used to generate periodic waves or enveloppe.
            /// </summary>
            public double _LocalTime_ms;

            public bool IsRunning;
            public EffectTypes Type;
            public EffectTypes PrevType;

            public double Direction_deg;

            public double Duration_ms;
            public double StartDelay_ms;
            /// <summary>
            /// Between 0.0 and 1.0
            /// </summary>
            public double Gain;
            /// <summary>
            /// Between -1.0 and 1.0
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

            public double Trq;
            public void Reset()
            {
                IsRunning = false;
                Type = EffectTypes.NO_EFFECT;
                PrevType = EffectTypes.NO_EFFECT;

                Direction_deg = 0.0;

                Duration_ms = -1.0;
                StartDelay_ms = -1.0;

                Gain = 1.0;

                Magnitude = 1.0;
                RampStartLevel_u = 0.0;
                RampEndLevel_u = 0.0;

                EnvAttackTime_ms = 0.0;
                EnvFadeTime_ms = 0.0;
                EnvAttackLevel_u = 0.0;
                EnvFadeLevel_u = 0.0;

                Period_ms = 50.0;
                PhaseShift_deg = 0.0;
                Offset_u = 0.0;
                Deadband_u = 0.0;

                PositiveCoef_u = 1.0;
                NegativeCoef_u = 1.0;
                PositiveSat_u = 1.0;
                NegativeSat_u = -1.0;


                _LocalTime_ms = 0.0;
                Trq = 0.0;
                Offset_u = 0.0;

            }

            public void CopyTo(ref Effect dest)
            {
                dest = (Effect)this.MemberwiseClone();
            }
        }

        /// <summary>
        /// All effects, indexed by BlockIndex.
        /// Effect 0 is not used by HID FFB, but we do use it for generic
        /// effects that are added on a per application bases, like a 
        /// permanent spring.
        /// </summary>
        protected Effect[] RunningEffects = new Effect[vJoyInterfaceWrap.vJoy.VJOY_FFB_MAX_EFFECTS_BLOCK_INDEX+1];
        protected Effect NewEffect = new Effect();
        protected virtual void SwitchTo(uint handle, EffectTypes effect)
        {
            if (RunningEffects[handle].Type == effect)
                return;
            RunningEffects[handle].PrevType = RunningEffects[handle].Type;
            RunningEffects[handle].Type = effect;
            Log("Effect " + handle.ToString() + " [" + RunningEffects[handle].PrevType.ToString() + "] to [" + effect.ToString() + "]");
        }

        public int LastNewEffectID = 0;

        public virtual void CreateNewEffect(uint handle)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Create new effect " + handle);
            }
            // Create effect is now done on vJoy driver side
        }

        public virtual void FreeEffect(uint handle)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Free Effect " + handle);
            }
            // Free effect is now done on vJoy driver side
        }
        public virtual void SetDuration(uint handle, double duration_ms)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set duration " + duration_ms + " ms (-1=infinite)");
            }
            RunningEffects[handle].Duration_ms = duration_ms;
        }
        public virtual void SetStartDelay(uint handle, double delay_ms)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set start delay " + delay_ms + " ms (0=no delay)");
            }
            RunningEffects[handle].StartDelay_ms = delay_ms;
        }

        public virtual void SetDirection(uint handle, double direction_deg)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set direction " + direction_deg + " deg");
            }
            RunningEffects[handle].Direction_deg = direction_deg;
        }

        public virtual void SetConstantTorqueEffect(uint handle, double magnitude)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set ConstantTorque magnitude " + magnitude);
            }
            RunningEffects[handle].Magnitude = magnitude;
        }

        public virtual void SetRampParams(uint handle, double startvalue_u, double endvalue_u)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set Ramp params " + startvalue_u + " " + endvalue_u);
            }
            // Prepare data
            RunningEffects[handle].RampStartLevel_u = startvalue_u;
            RunningEffects[handle].RampEndLevel_u = endvalue_u;
        }

        /// <summary>
        /// Set effect gain in percent (0/+1.0)
        /// </summary>
        /// <param name="gain_pct">[in] Global gain in percent</param>
        public virtual void SetEffectGain(uint handle, double gain_pct)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set magnitude gain " + gain_pct);
            }
            // Restrict range
            if (gain_pct < 0.0) gain_pct = 0.0;
            if (gain_pct > 1.0) gain_pct = 1.0;
            // save gain and update current gain
            RunningEffects[handle].Gain = gain_pct;
        }

        public virtual void SetEnveloppeParams(uint handle, double attacktime_ms, double attacklevel_u, double fadetime_ms, double fadelevel_u)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set enveloppe params attacktime=" + attacktime_ms + "ms attacklevel=" + attacklevel_u + " fadetime=" + fadetime_ms + "ms fadelevel=" + fadelevel_u);
            }
            // Prepare data
            RunningEffects[handle].EnvAttackTime_ms = attacktime_ms;
            RunningEffects[handle].EnvAttackLevel_u = attacklevel_u;
            RunningEffects[handle].EnvFadeTime_ms = fadetime_ms;
            RunningEffects[handle].EnvFadeLevel_u = fadelevel_u;
        }

        public virtual void SetLimitsParams(uint handle, double offset_u, double deadband_u,
            double poscoef_u, double negcoef_u, double poslim_u, double neglim_u)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set limits offset=" + offset_u + " deadband=" + deadband_u
                + " PosCoef=" + poscoef_u + " NegCoef=" + negcoef_u + " sat=[" + neglim_u
                + "; " + poslim_u + "]");
            }
            // Prepare data
            RunningEffects[handle].Deadband_u = deadband_u;
            RunningEffects[handle].Offset_u = offset_u;
            RunningEffects[handle].PositiveCoef_u = poscoef_u;
            RunningEffects[handle].NegativeCoef_u = negcoef_u;
            RunningEffects[handle].PositiveSat_u = poslim_u;
            RunningEffects[handle].NegativeSat_u = neglim_u;
        }

        public virtual void SetPeriodicParams(uint handle, double magnitude_u, double offset_u, double phaseshift_deg, double period_ms)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB set Effect " + handle + " periodic params magnitude=" + magnitude_u + " offset=" + offset_u + " phase= " + phaseshift_deg + " period=" + period_ms + "ms");
            }
            // Prepare data
            RunningEffects[handle].Magnitude = magnitude_u;
            RunningEffects[handle].Offset_u = offset_u;
            RunningEffects[handle].PhaseShift_deg = phaseshift_deg;
            // Default period if null
            if (period_ms==0)
                period_ms = this.RefreshPeriod_ms*10; //5ms*10 = 20Hz
                                                      // Minimale period
            if (period_ms<=(RefreshPeriod_ms*2))
                RunningEffects[handle].Period_ms = (RefreshPeriod_ms*2);
            else
                RunningEffects[handle].Period_ms = period_ms;
        }

        public virtual void SetEffect(uint handle, EffectTypes type)
        {
            if (BFFManager.Config.Application.VerboseFFBManager) {
                Log("FFB Effect " + handle + " set " + type.ToString() + " Effect");
            }
            if (!this.IsDeviceReady) {
                Log("Device not yet ready", LogLevels.INFORMATIVE);
                return;
            }
            SwitchTo(handle, type);
        }

        /// <summary>
        /// Not yet done
        /// </summary>
        /// <param name="loopCount"></param>
        public virtual void StartEffect(uint handle, int loopCount)
        {
            StartEffect(handle);
        }

        public virtual void StartEffect(uint handle)
        {
            Log("FFB Effect " + handle + " got start effect");
            if (!this.IsDeviceReady) {
                Log("Device not yet ready");
                return;
            }
            // Set Start flag
            RunningEffects[handle].IsRunning = true;
            RunningEffects[handle]._LocalTime_ms = -RunningEffects[handle].StartDelay_ms;
            if (this.State != FFBStates.DEVICE_EFFECT_RUNNING)
                TransitionTo(FFBStates.DEVICE_EFFECT_RUNNING);
        }
        public virtual void StopEffect(uint handle)
        {
            if (SkipStopEffect) {
                Log("FFB Effect " + handle + " got stop effect, but skipped it (configured)");
                return;
            }

            Log("FFB Effect " + handle + " got stop effect");
            if (RunningEffects[handle].IsRunning) {
                RunningEffects[handle].IsRunning = false;
            }
        }
        public virtual void StopAllEffects()
        {
            if (SkipStopEffect) {
                Log("FFB Got stop all effects, but skipped it (configured)");
                return;
            }

            Log("FFB Got stop all effects");
            for (int i = 0; i < RunningEffects.Length; i++) {
                if (RunningEffects[i].IsRunning) {
                    RunningEffects[i].IsRunning = false;
                }
            }
        }

        public virtual void ResetAllEffects()
        {
            Log("FFB Reset all effects");
            for (int i = 0; i<RunningEffects.Length; i++) {
                RunningEffects[i].Reset();
            }
        }

        public virtual void ResetEffect(uint handle)
        {
            Log("FFB Effect " + handle + " got reset effect");
            if (RunningEffects[handle].IsRunning)
                StopEffect(handle);
            RunningEffects[handle].Reset();
        }

        #endregion

        #region Torque computation for PWM or emulation
        public double MinVelThreshold { get { return FFBParams.MinVelThreshold; } }
        public double MinAccelThreshold { get { return FFBParams.MinAccelThreshold; } }
        public double MinDamperForActive { get { return FFBParams.MinDamperForActive; } }
        public double PermanentSpring { get { return FFBParams.PermanentSpring; } }

        public double Spring_TrqDeadband { get { return FFBParams.Spring_TrqDeadband; } }
        public double Spring_Kp { get { return FFBParams.Spring_Kp; } }
        public double Spring_Bv { get { return FFBParams.Spring_Bv; } }
        public double Spring_Ki { get { return FFBParams.Spring_Ki; } }
        public double Damper_J { get { return FFBParams.Damper_J; } }
        public double Damper_Bv { get { return FFBParams.Damper_Bv; } }
        public double Friction_Bv { get { return FFBParams.Friction_Bv; } }
        public double Inertia_Bv { get { return FFBParams.Inertia_Bv; } }
        public double Inertia_BvRaw { get { return FFBParams.Inertia_BvRaw; } }
        public double Inertia_J { get { return FFBParams.Inertia_J; } }

        /// <summary>
        /// Maintain given value, sign with direction if application
        /// set a 270° angle instead of the usual 90° (0x63).
        /// T = Direction x K1 x Constant
        /// </summary>
        /// <returns></returns>
        protected virtual double TrqFromConstant(int handle)
        {
            double Trq = RunningEffects[handle].Magnitude;
            if (RunningEffects[handle].Direction_deg > 180) {
                if (FFBParams.DirectionUseSignedMagnitude) {
                    Trq = -RunningEffects[handle].Magnitude;
                } else {
                    Trq = -Math.Abs(RunningEffects[handle].Magnitude);
                }
            }
            return Trq;
        }

        /// <summary>
        /// Ramp torque from start to end given a duration.
        /// Let 's' be the normalized time ratio between 0
        /// (start) and end (1.0) of the ramp.
        /// T = Start*(1-s) + End*s
        /// ^-- Start when s=0, End when s=1.
        /// </summary>
        /// <returns></returns>
        protected virtual double TrqFromRamp(int handle)
        {
            double time_ratio = RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Duration_ms;
            double Trq = RunningEffects[handle].RampStartLevel_u * (1.0 - time_ratio) +
                           RunningEffects[handle].RampEndLevel_u * (time_ratio);
            return Trq;
        }

        /// <summary>
        /// Constant torque in opposition to current velocity.
        /// T = -Sign(W) x K1 x Constant
        ///        ^-- sign with opposite direction of motion
        /// </summary>
        /// <param name="W"></param>
        /// <param name="kvel"></param>
        /// <returns></returns>
        protected virtual double TrqFromFriction(int handle, double W)
        {
            double Trq;
            // Deadband for slow speed?
            if (Math.Abs(W) < MinVelThreshold)
                Trq = 0.0;
            else if (W< 0)
                Trq =  Friction_Bv * RunningEffects[handle].NegativeCoef_u;
            else
                Trq = -Friction_Bv * RunningEffects[handle].PositiveCoef_u;
            //Log("Friction W=" + W + " trq=" + Trq + " (kv=" + kvel + ")");
            return Trq;
        }

        /// <summary>
        /// Torque to maintain current velocity with boost in 
        /// acceleration since Inertia should add repulsive
        /// force when accelerating/starting to move the wheel.
        /// T = -Sign(W) x K1 x I x |A| - K2 * rawW + k3 * W
        ///     ^---- opposite dir. ----^           ^-- same dir. of motion
        /// </summary>
        /// <param name="W"></param>
        /// <param name="rawW"></param>
        /// <param name="A"></param>
        /// <param name="kvelraw"></param>
        /// <param name="kvel"></param>
        /// <param name="kinertia"></param>
        /// <returns></returns>
        protected virtual double TrqFromInertia(int handle, double W, double rawW, double A)
        {
            double Trq;
            // Deadband for slow speed?
            if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccelThreshold))
                Trq = -Math.Sign(W) * Inertia_J * Inertia * Math.Abs(A) - Inertia_BvRaw * this.RawSpeed_u_per_s + Inertia_Bv * W;
            else
                Trq = 0.0;
            //Log("Inertia W=" + W + " rW=" + rawW + " A=" + A + " trq=" + Trq + " (kvr=" + kvelraw + " kv=" + kvel + " ki=" + kinertia + ")");
            return Trq;
        }


        double lasterror = 0;
        double interror = 0;

        /// <summary>
        /// PID servoing to given position
        /// T = Kp x (R-P) + Bv*d(R-P)/dt + Ki*Int(R-P,dt)
        /// </summary>
        /// <param name="Ref"></param>
        /// <param name="Mea"></param>
        /// <returns></returns>
        protected virtual double TrqFromSpring(int handle, double Ref, double Mea)
        {
            double Trq;
            // Add Offset to reference position, then substract measure
            var offset_u = RunningEffects[handle].Offset_u;
            var error = (Ref + offset_u) - Mea;
            double differror = (error-lasterror)/(BFFManager.GlobalRefreshPeriod_ms * 0.001);
            lasterror = error;
            // Error dead-band
            if (Math.Abs(error) < RunningEffects[handle].Deadband_u) {
                error = 0;
            }
            // Apply PID gains and select gain according
            // to sign of error (maybe should be motion/velocity?)
            if (error < 0) {
                Trq = RunningEffects[handle].NegativeCoef_u * (Spring_Kp * error + Spring_Bv*differror + Spring_Ki*interror);
            } else {
                Trq = RunningEffects[handle].PositiveCoef_u * (Spring_Kp * error + Spring_Bv*differror + Spring_Ki*interror);
            }
            // Integral anti wind-up
            if ((Trq<RunningEffects[handle].PositiveSat_u) && (Trq>RunningEffects[handle].NegativeSat_u)) {
                interror += error;
                // Saturation
                interror = Math.Min(RunningEffects[handle].PositiveSat_u, interror);
                interror = Math.Max(RunningEffects[handle].NegativeSat_u, interror);
            }
            // Torque dead band
            if (Math.Abs(Trq) < Spring_TrqDeadband) {
                Trq = 0.0;
            }
            // Saturation
            Trq = Math.Min(RunningEffects[handle].PositiveSat_u, Trq);
            Trq = Math.Max(RunningEffects[handle].NegativeSat_u, Trq);
            //Log("Spring R=" + Ref + " P=" + Mea + " error=" + error + " trq=" + Trq + " (kp=" + kp + ")");
            return Trq;
        }

        /// <summary>
        /// Add torque in opposition to current accel and speed (friction)
        /// T = -K2 x W - K3 x I x A
        /// </summary>
        /// <param name="W"></param>
        /// <param name="rawW"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        protected virtual double TrqFromDamper(int handle, double W, double rawW, double A)
        {
            double Trq;

            // Add friction/damper effect in opposition to motion
            // Deadband for slow speed?
            if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccelThreshold)) {
                // Friction
                Trq = -this.Damper_Bv * rawW; // - Math.Sign(W) * Damper_J * Inertia * Math.Abs(A);
                // W and A same sign ? Add another W with a smaller coef
                if (W*A>0) {
                    Trq += -Damper_J * W;
                }

            } else {
                Trq = 0.0;
            }
            // Saturation
            if (handle>=0) {
                Trq = Math.Min(RunningEffects[handle].PositiveSat_u, Trq);
                Trq = Math.Max(RunningEffects[handle].NegativeSat_u, Trq);
            }
            //Log("Damper W=" + W + " A=" + A + " trq=" + Trq + " (kv=" + kvel + " ki=" + kinertia + ")");
            return Trq;
        }

        /// <summary>
        /// Apply enveloppe gain to the computed torque.
        /// Only used by periodic (wave) effects.
        /// See https://github.com/njz3/ArduinoJoystickWithFFBLibrary/blob/master/src/Joystick.cpp
        /// </summary>
        /// <returns></returns>
        protected virtual double ApplyEnvelope(int handle, double value)
        {
            // Unused for now (need to clarify the units and the offsets)
            double magnitude = RunningEffects[handle].Magnitude * RunningEffects[handle].Gain;
            double attackLevel = RunningEffects[handle].EnvAttackLevel_u * RunningEffects[handle].Gain;
            double fadeLevel = RunningEffects[handle].EnvFadeLevel_u * RunningEffects[handle].Gain;
            double newValue = magnitude;

            double attackTime_ms = RunningEffects[handle].EnvAttackTime_ms;
            double fadeTime_ms = RunningEffects[handle].EnvFadeTime_ms;
            double elapsedTime_ms = RunningEffects[handle]._LocalTime_ms;
            double duration_ms = RunningEffects[handle].Duration_ms;

            if (elapsedTime_ms<attackTime_ms) {
                newValue = (magnitude - attackLevel) * attackTime_ms;
                newValue /= attackTime_ms;
                newValue += attackLevel;
            }
            if (elapsedTime_ms > (duration_ms - fadeTime_ms)) {
                newValue = (magnitude - fadeLevel) * (duration_ms - elapsedTime_ms);
                newValue /= fadeTime_ms;
                newValue += fadeLevel;
            }

            newValue *= value;
            //return newValue;
            // Do not apply enveloppe for now: need to clarify the units and the offsets
            return value;
        }
        protected virtual double TrqFromSine(int handle)
        {
            // Get phase in radians
            double phase_rad = (Math.PI/180.0) * (RunningEffects[handle].PhaseShift_deg + 360.0*(RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Period_ms));
            double Trq = ApplyEnvelope(handle, Math.Sin(phase_rad) * RunningEffects[handle].Magnitude) + RunningEffects[handle].Offset_u;
            return Trq;
        }

        protected virtual double TrqFromSquare(int handle)
        {
            double Trq;
            // Get phase in degrees
            double phase_deg = (RunningEffects[handle].PhaseShift_deg + 360.0*(RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Period_ms)) % 360.0;
            // produce a square pulse depending on phase value
            if (phase_deg < 180.0) {
                Trq =  ApplyEnvelope(handle, RunningEffects[handle].Magnitude) + RunningEffects[handle].Offset_u;
            } else {
                Trq = ApplyEnvelope(handle, -RunningEffects[handle].Magnitude) + RunningEffects[handle].Offset_u;
            }
            return Trq;
        }
        protected virtual double TrqFromTriangle(int handle)
        {
            double Trq;
            // Get phase in degrees
            double phase_deg = (RunningEffects[handle].PhaseShift_deg + 360.0*(RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Period_ms)) % 360.0;
            double time_ratio = Math.Abs(phase_deg) * (1.0/180.0);
            // produce a triangle pulse depending on phase value
            if (phase_deg <= 180.0) {
                // Ramping up triangle
                Trq = ApplyEnvelope(handle, RunningEffects[handle].Magnitude* (2.0*time_ratio-1.0)) + RunningEffects[handle].Offset_u;
            } else {
                // Ramping down triangle
                Trq = ApplyEnvelope(handle, RunningEffects[handle].Magnitude* (3.0-2.0* time_ratio)) + RunningEffects[handle].Offset_u;
            }
            return Trq;
        }

        protected virtual double TrqFromSawtoothUp(int handle)
        {
            double Trq;
            // Get phase in degrees
            double phase_deg = (RunningEffects[handle].PhaseShift_deg + 360.0*(RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Period_ms)) % 360.0;
            double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
            // Ramping up triangle given phase value between
            Trq = ApplyEnvelope(handle, RunningEffects[handle].Magnitude* (2.0*time_ratio-1.0)) + RunningEffects[handle].Offset_u;
            return Trq;
        }

        protected virtual double TrqFromSawtoothDown(int handle)
        {
            double Trq;
            // Get phase in degrees
            double phase_deg = (RunningEffects[handle].PhaseShift_deg + 360.0*(RunningEffects[handle]._LocalTime_ms / RunningEffects[handle].Period_ms)) % 360.0;
            double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
            // Ramping up triangle given phase value between
            Trq = ApplyEnvelope(handle, RunningEffects[handle].Magnitude*(1.0-2.0*time_ratio)) + RunningEffects[handle].Offset_u;
            return Trq;
        }
        #endregion

    }
}


