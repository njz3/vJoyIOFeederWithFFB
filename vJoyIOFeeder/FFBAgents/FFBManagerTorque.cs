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
        AFFBManager
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
        protected override void ComputeTrqFromAllEffects()
        {
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

            double AllTrq = 0.0;
            for (int i = 0; i<RunningEffects.Length; i++) {
                // Skip effect not running
                if (!RunningEffects[i].IsRunning || RunningEffects[i]._LocalTime_ms < 0.0) {
                    continue;
                }
                double Trq = 0.0;
                switch (RunningEffects[i].Type) {
                    case EffectTypes.NO_EFFECT:
                        break;
                    case EffectTypes.CONSTANT_TORQUE:
                        Trq += TrqFromConstant(i);
                        break;
                    case EffectTypes.RAMP:
                        Trq += TrqFromRamp(i);
                        break;
                    case EffectTypes.FRICTION:
                        Trq += TrqFromFriction(i, W);
                        break;
                    case EffectTypes.INERTIA:
                        Trq += TrqFromInertia(i, W, this.RawSpeed_u_per_s, A);
                        break;
                    case EffectTypes.SPRING:
                        Trq += TrqFromSpring(i, R, P);
                        break;
                    case EffectTypes.DAMPER:
                        Trq += TrqFromDamper(i, W, A);
                        break;
                    case EffectTypes.SINE:
                        Trq += TrqFromSine(i);
                        break;
                    case EffectTypes.SQUARE:
                        Trq += TrqFromSquare(i);
                        break;
                    case EffectTypes.TRIANGLE:
                        Trq += TrqFromTriangle(i);
                        break;
                    case EffectTypes.SAWTOOTHUP:
                        Trq += TrqFromSawtoothUp(i);
                        break;
                    case EffectTypes.SAWTOOTHDOWN:
                        Trq += TrqFromSawtoothDown(i);
                        break;

                    default:
                        break;
                }
                AllTrq += Trq * RunningEffects[i].Gain;
            }

            // Change sign of torque if inverted and apply gains
            AllTrq = TrqSign* Math.Sign(AllTrq) * Math.Pow(Math.Abs(AllTrq), PowerLaw) * DeviceGain* GlobalGain;


            // Deadband for small torque values
            if (Math.Abs(AllTrq) < TrqDeadBand) {
                // No effect
                AllTrq = 0.0;
            }

            // Scale in range
            AllTrq = Math.Max(Math.Min(AllTrq, 1.0), -1.0);

            // In case output level is in other units (like Nm), we'll probably 
            // need to change this
            var FFB_To_Nm_cste = 1.0;

            // Now save in memory protected variable
            OutputTorqueLevel = AllTrq * FFB_To_Nm_cste;
            OutputEffectCommand = 0x0;

            CheckForEffectsDone();
        }

    }
}


