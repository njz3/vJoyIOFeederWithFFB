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
    /// Torque output can then be translated in PWM or any other digital value to tranmit
    /// to a motor driver.
    /// ----------------------------------------------------
    /// /!\ Check your signs before running strong effects !
    /// ----------------------------------------------------
    /// Trq > 0: torque should increase motor angle (positive rotation)
    /// Trq < 0: torque should decrease motor angle (negative rotation)
    /// </summary>
    public class FFBManagerTorque :
        IFFBManager
    {
        public FFBManagerTorque(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
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
                case FFBStates.UNDEF:
                    TransitionTo(FFBStates.DEVICE_INIT);
                    break;
                case FFBStates.DEVICE_RESET:
                    ResetEffect();
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
                case FFBStates.DEVICE_INIT:
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
                case FFBStates.DEVICE_DISABLE:
                    break;
                case FFBStates.DEVICE_READY:
                    break;

                case FFBStates.NO_EFFECT:
                    break;

                case FFBStates.CONSTANT_TORQUE: {
                        // Maintain given value, sign with direction if application
                        // set a 270° angle instead of the usual 90° (0x63).
                        // T = Direction x K1 x Constant
                        Trq = RunningEffect.Magnitude;
                        if (RunningEffect.Direction_deg > 180)
                            Trq = -RunningEffect.Magnitude;
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
                        // T = K1 x (R-P)
                        // Add Offset to reference position, then substract measure
                        var error = (R + RunningEffect.Offset_u) - P;
                        // Dead-band
                        if (Math.Abs(error) < RunningEffect.Deadband_u) {
                            error = 0;
                        }
                        // Apply proportionnal gain and select gain according
                        // to sign of error (maybe should be motion/velocity?)
                        var k1 = 1.0; // *0.5 *2?
                        if (error < 0)
                            Trq = k1 * RunningEffect.NegativeCoef_u * error;
                        else
                            Trq = k1 * RunningEffect.PositiveCoef_u * error;
                        // Saturation
                        Trq = Math.Min(RunningEffect.PositiveSat_u, Trq);
                        Trq = Math.Max(RunningEffect.NegativeSat_u, Trq);
                    }
                    break;

                case FFBStates.DAMPER: {
                        // Add torque in opposition to 
                        // current accel and speed (friction)
                        // T = -K2 x W -K3 x I x A
                        
                        // Add friction/damper effect in opposition to motion
                        var k1 = 0.2;
                        var k2 = 0.2;
                        // Deadband for slow speed?
                        if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccThreshold))
                            Trq = - k1 * W - Math.Sign(W) * k2 * Inertia * Math.Abs(A);
                        // Saturation
                        Trq = Math.Min(RunningEffect.PositiveSat_u, Trq);
                        Trq = Math.Max(RunningEffect.NegativeSat_u, Trq);
                    }
                    break;

                case FFBStates.SINE: {
                        // Get phase in radians
                        double phase_rad = (Math.PI/180.0) * (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms));
                        Trq = Math.Sin(phase_rad)*RunningEffect.Magnitude + RunningEffect.Offset_u;

                        // Saturation
                        Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                        // All done
                    }
                    break;
                case FFBStates.SQUARE: {
                        // Get phase in degrees
                        double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                        // produce a square pulse depending on phase value
                        if (phase_deg < 180.0) {
                            Trq = RunningEffect.Magnitude + RunningEffect.Offset_u;
                        } else {
                            Trq = -RunningEffect.Magnitude + RunningEffect.Offset_u;
                        }
                        // Saturation
                        Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                        // All done
                    }
                    break;

                case FFBStates.SAWTOOTHUP: {
                        // Get phase in degrees
                        double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                        double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
                        // Ramping up triangle given phase value between
                        Trq = RunningEffect.Magnitude*(2.0*time_ratio-1.0) + RunningEffect.Offset_u;
                        // Saturation
                        Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                        // All done
                    }
                    break;

                case FFBStates.SAWTOOTHDOWN: {
                        // Get phase in degrees
                        double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                        double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
                        // Ramping up triangle given phase value between
                        Trq = RunningEffect.Magnitude*(1.0-2.0*time_ratio) + RunningEffect.Offset_u;
                        // Saturation
                        Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                        // All done
                    }
                    break;


                case FFBStates.TRIANGLE: {
                        // Get phase in degrees
                        double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                        double time_ratio = Math.Abs(phase_deg) * (1.0/180.0);
                        // produce a triangle pulse depending on phase value
                        if (phase_deg <= 180.0) {
                            // Ramping up triangle
                            Trq = RunningEffect.Magnitude*(2.0*time_ratio-1.0) + RunningEffect.Offset_u;
                        } else {
                            // Ramping down triangle
                            Trq = RunningEffect.Magnitude*(3.0-2.0*time_ratio) + RunningEffect.Offset_u;
                        }
                        // Saturation
                        Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                        // All done
                    }
                    break;

                default:
                    break;
            }

            // Sign torque if inverted
            Trq = this.TrqSign * Trq;

            // Scale in range and apply global gains before leaving
            Trq = Math.Max(Trq, -1.0);
            Trq = Math.Min(Trq, 1.0);
            // In case output level is in other units (like Nm), we'll probably 
            // need to change this
            var FFB_To_Nm_cste = 1.0;

            // Now save in memory protected variable
            OutputTorqueLevel = RunningEffect.GlobalGain * Trq * FFB_To_Nm_cste;
            OutputEffectCommand = 0x0;

            RunningEffect._LocalTime_ms += Timer.Period_ms;
            if (this.State != FFBStates.NO_EFFECT) {
                if (RunningEffect.Duration_ms >= 0.0 &&
                    RunningEffect._LocalTime_ms > RunningEffect.Duration_ms) {
                    Log("Effect duration reached");
                    TransitionTo(FFBStates.NO_EFFECT);
                }
            }

        }



    }
}


