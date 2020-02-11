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

                case FFBStates.CONSTANT_TORQUE:
                    Trq = TrqFromConstant();
                    break;
                case FFBStates.RAMP:
                    Trq = TrqFromRamp();
                    break;
                case FFBStates.FRICTION:
                    Trq = TrqFromFriction(W);
                    break;
                case FFBStates.INERTIA:
                    Trq = TrqFromInertia(W, this.RawSpeed_u_per_s, A);
                    break;
                case FFBStates.SPRING:
                    Trq = TrqFromSpring(R, P);
                    break;
                case FFBStates.DAMPER:
                    Trq = TrqFromDamper(W, A);
                    break;
                case FFBStates.SINE:
                    Trq = TrqFromSine();
                    break;
                case FFBStates.SQUARE:
                    Trq = TrqFromSquare();
                    break;
                case FFBStates.TRIANGLE:
                    Trq = TrqFromTriangle();
                    break;
                case FFBStates.SAWTOOTHUP:
                    Trq = TrqFromSawtoothUp();
                    break;
                case FFBStates.SAWTOOTHDOWN:
                    Trq = TrqFromSawtoothDown();
                    break;

                default:
                    break;
            }

            // Sign torque if inverted
            Trq = this.TrqSign * Trq;

            // Saturation
            Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
            // In case output level is in other units (like Nm), we'll probably 
            // need to change this
            var FFB_To_Nm_cste = 1.0;

            // Now save in memory protected variable
            OutputTorqueLevel = RunningEffect.GlobalGain * Trq * FFB_To_Nm_cste;
            OutputEffectCommand = 0x0;

            FFBEffectsEndStateMachine();
        }

    }
}


