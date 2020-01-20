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
    public class FFBManagerModel3 :
        IFFBManager
    {
        protected MultimediaTimer Timer;
        protected int RefreshPeriod_ms = 1;
        protected double Tick_per_s = 1.0;

        public FFBManagerModel3(int refreshPeriod_ms) :
            base()
        {
            RefreshPeriod_ms = refreshPeriod_ms;
            Tick_per_s = 1000.0 / (double)RefreshPeriod_ms;
            Timer = new MultimediaTimer(refreshPeriod_ms);
        }

        /// <summary>
        /// Start the timer operation
        /// </summary>
        /// <returns></returns>
        public override void Start()
        {
            Timer = new MultimediaTimer(RefreshPeriod_ms);
            Timer.Handler = Timer_Handler;
            Timer.Start();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        /// <returns></returns>
        public override void Stop()
        {
            Timer.Stop();
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
        protected override void FFBEffectsStateMachine()
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

            switch (State) {
                case FFBStates.NOP:
                    break;

                case FFBStates.CONSTANT_TORQUE: {
                        // Maintain given value, sign with direction if application
                        // set a 270° angle instead of the usual 90° (0x63).
                        // T = Direction x K1 x Constant
                        var k1 = 1.0;
                        Trq = k1 * RunningEffect.ConstantTorqueMagnitude;
                        if (RunningEffect.Direction_deg > 180)
                            Trq = -RunningEffect.ConstantTorqueMagnitude;
                    }
                    break;
                case FFBStates.RAMP: {
                        // Ramp torque from start to end given a duration.
                        // Let 's' be the normalized time ratio between 0
                        // (start) and end (1.0) of the ramp.
                        // T = Start*(1-s) + End*s
                        // ^-- Start when s=0, End when s=1.
                        double time_ratio = RunningEffect._LocalTime_ms / RunningEffect.Duration_ms;
                        double value = RunningEffect.RampStartLevel_u * (1.0 - time_ratio) +
                                       RunningEffect.RampEndLevel_u * (time_ratio);
                        Trq = value;
                    }
                    break;
                case FFBStates.FRICTION: {
                        // Constant torque in opposition to current velocity.
                        // T = -Sign(W) x K1 x Constant
                        //        ^-- sign with opposite direction of motion
                        var k1 = 0.1; //1.0 Coeff ?
                        // Deadband for slow speed?
                        if (Math.Abs(W) < MinVelThreshold)
                            Trq = 0.0;
                        else if (W < 0)
                            Trq = k1 * RunningEffect.NegativeCoef_u;
                        else
                            Trq = -k1 * RunningEffect.PositiveCoef_u;
                    }
                    break;
                case FFBStates.INERTIA: {
                        // Torque to maintain current velocity with boost in 
                        // acceleration since Inertia should add repulsive
                        // force when accelerating/starting to move the wheel.
                        // T = -Sign(W) x K1 x I x |A| + K2 * W
                        //        ^-- opposite direction      ^-- same direction of motion
                        var k1 = 0.1; // ridiculous coeff ?
                        var k2 = 0.2;
                        // Deadband for slow speed?
                        if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccThreshold))
                            Trq = -Math.Sign(W) * k1 * Inertia * Math.Abs(A) + k2 * W;
                        else
                            Trq = 0.0;
                    }
                    break;
                case FFBStates.SPRING: {
                        // Torque proportionnal to error in position
                        // T = -K1 x (R-P)
                        // Add Offset to reference position, then substract measure
                        var error = (R + RunningEffect.Offset_u) - P;
                        // Dead-band
                        if (Math.Abs(error) < RunningEffect.Deadband_u) {
                            error = 0;
                        }
                        // Apply proportionnal gain and select gain according
                        // to sign of error (maybe should be motion/velocity?)
                        var k1 = 1.0; // *2?
                        if (error < 0)
                            Trq = -k1 * RunningEffect.NegativeCoef_u * error;
                        else
                            Trq = -k1 * RunningEffect.PositiveCoef_u * error;
                        // Saturation
                        Trq = Math.Min(RunningEffect.PositiveSat_u, Trq);
                        Trq = Math.Max(RunningEffect.NegativeSat_u, Trq);
                    }
                    break;
                case FFBStates.DAMPER: {
                        // Like spring, but add torque in opposition to 
                        // current accel and speed (friction)
                        // T = -K1 x (R-P) -K2 x W -K3 x I x A
                        // Add Offset to reference position, then substract measure
                        var error = (R + RunningEffect.Offset_u) - P;
                        // Dead-band
                        if (Math.Abs(error) < RunningEffect.Deadband_u) {
                            error = 0;
                        }
                        // Apply proportionnal gain and select gain according
                        // to sign of error (maybe should be motion/velocity?)
                        var k1 = 1.0; // *2?
                        if (error < 0)
                            Trq = -k1 * RunningEffect.NegativeCoef_u * error;
                        else
                            Trq = -k1 * RunningEffect.PositiveCoef_u * error;
                        // Add friction/damper effect in opposition to motion
                        var k2 = 0.2;
                        var k3 = 0.2;
                        // Deadband for slow speed?
                        if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccThreshold))
                            Trq = Trq - k2 * W - Math.Sign(W) * k3 * Inertia * Math.Abs(A);
                        // Saturation
                        Trq = Math.Min(RunningEffect.PositiveSat_u, Trq);
                        Trq = Math.Max(RunningEffect.NegativeSat_u, Trq);
                    }
                    break;
                default:
                    break;
            }

            // Scale in range and apply global gains before leaving
            Trq = Math.Max(Trq, -1.0);
            Trq = Math.Min(Trq, 1.0);
            // In case output level is in other units (like Nm), we'll probably 
            // need to change this
            var FFB_To_Nm_cste = 1.0;

            // Memory protected variable
            OutputTorqueLevel = RunningEffect.GlobalGain * Trq * FFB_To_Nm_cste;

            RunningEffect._LocalTime_ms += Timer.Period_ms;
            if (this.State != FFBStates.NOP) {
                if (RunningEffect.Duration_ms >= 0.0 &&
                    RunningEffect._LocalTime_ms > RunningEffect.Duration_ms) {
                    TransitionTo(FFBStates.NOP);
                }
            }

        }
        

        public override void SetDuration(double duration_ms)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set duration " + duration_ms + " ms");
#endif
            NewEffect.Duration_ms = duration_ms;
        }

        public override void SetDirection(double direction_deg)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set direction " + direction_deg + " deg");
#endif
            NewEffect.Direction_deg = direction_deg;
        }

        public override void SetConstantTorqueEffect(double magnitude)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set ConstantTorque magnitude " + magnitude);
#endif
            NewEffect.ConstantTorqueMagnitude = magnitude;
        }

        public override void SetRampParams(double startvalue_u, double endvalue_u)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set Ramp params " + startvalue_u + " " + endvalue_u);
#endif
            // Prepare data
            NewEffect.RampStartLevel_u = startvalue_u;
            NewEffect.RampEndLevel_u = endvalue_u;
        }
        /// <summary>
        /// Set global gain in percent (0/+1.0)
        /// </summary>
        /// <param name="gain_pct">[in] Global gain in percent</param>
        public override void SetGain(double gain_pct)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set global gain " + gain_pct);
#endif
            // Restrict range
            if (gain_pct < 0.0) gain_pct = 0.0;
            if (gain_pct > 1.0) gain_pct = 1.0;
            // save gain
            NewEffect.GlobalGain = gain_pct;
        }
        public override void SetEnveloppeParams(double attacktime_ms, double attacklevel_Nm, double fadetime_ms, double fadelevel_Nm)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set enveloppe params " + attacktime_ms + "ms " + attacklevel_Nm + " " + fadetime_ms + "ms " + fadelevel_Nm);
#endif
            // Prepare data
            NewEffect.EnvAttackTime_ms = attacktime_ms;
            NewEffect.EnvAttackLevel_u = attacklevel_Nm;
            NewEffect.EnvFadeTime_ms = fadetime_ms;
            NewEffect.EnvFadeLevel_u = fadelevel_Nm;
        }

        public override void SetLimitsParams(double offset_u, double deadband_u,
            double poscoef_u, double negcoef_u, double poslim_u, double neglim_u)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set limits offset=" + offset_u + " deadband=" + deadband_u
                + " PosCoef=" + poscoef_u + " NegCoef=" + negcoef_u + " sat=[" + neglim_u
                + "; " + poslim_u + "]");
#endif
            // Prepare data
            NewEffect.Deadband_u = deadband_u;
            NewEffect.Offset_u = offset_u;
            NewEffect.PositiveCoef_u = poscoef_u;
            NewEffect.NegativeCoef_u = negcoef_u;
            NewEffect.PositiveSat_u = poslim_u;
            NewEffect.NegativeSat_u = neglim_u;
        }


        public override void SetEffect(FFBType type)
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB set " + type.ToString() + " Effect");
#endif
            NewEffect.Type = type;
        }

        public override void StartEffect()
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

        public override void StopEffect()
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB Got stop effect");
#endif
            // Switch to
            if (this.State != FFBStates.NOP)
                TransitionTo(FFBStates.NOP);
        }

        public override void ResetEffect()
        {
#if CONSOLE_DUMP
            Console.WriteLine("FFB Got device reset");
#endif
            StopEffect();
            NewEffect.Reset();
        }

    }
}


