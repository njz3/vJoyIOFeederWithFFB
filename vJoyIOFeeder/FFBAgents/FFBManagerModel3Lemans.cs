//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.FFBAgents
{
    /// <summary>
    /// Testing results:
    /// only 8 level of effects (x = 0..7)
    /// 
    /// 0x00 sequence in selection screen?
    /// 0x1x no effect
    /// 0x2x = friction/clutch?
    /// 0x3x = spring
    /// 0x5x, 0x6X = constant torque turn left/right
    /// 0xFF = ping
    /// 
    /// </summary>
    public class FFBManagerModel3Lemans :
        IFFBManager
    {
        const int MAX_LEVEL = 0x7;

        public FFBManagerModel3Lemans(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
        }


        /// <summary>
        /// When states are too complicated, a separate method is called.
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
            // OutputEffectCommand
            //
            // For model 3 hardware, effects are directly translated to commands
            // for the driveboard

            switch (State) {

                case FFBStates.UNDEF:
                    TransitionTo(FFBStates.DEVICE_INIT);
                    break;
                case FFBStates.DEVICE_INIT:
                    State_INIT();
                    break;
                case FFBStates.DEVICE_RESET:
                    ResetEffect();
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
                case FFBStates.DEVICE_DISABLE:
                    OutputEffectCommand = 0x10;
                    break;
                case FFBStates.DEVICE_READY:
                    OutputEffectCommand = 0xFF;
                    break;


                case FFBStates.NO_EFFECT:
                    OutputEffectCommand = 0x10;
                    break;

                case FFBStates.CONSTANT_TORQUE: {
                        // Maintain given value, sign with direction if application
                        // set a 270° angle instead of the usual 90° (0x63).
                        // T = Direction x K1 x Constant
                        Trq = RunningEffect.Magnitude;
                        if (RunningEffect.Direction_deg > 180)
                            Trq = -RunningEffect.Magnitude;

                        // Scale in range and apply global gains before leaving
                        Trq = Math.Max(Math.Min(RunningEffect.GlobalGain * Trq, 1.0), -1.0);
                        // Trq is now in [-1; 1]

                        if (Trq<0) {
                            // Rotate wheel left - SendConstantForce(-)
                            // 0x60: Disable - 0x61 = weakest - 0x6F = strongest
                            int strength = (int)(-Trq* MAX_LEVEL);
                            OutputEffectCommand = 0x60 + strength;
                        } else {
                            // Rotate wheel right – SendConstantForce (+)
                            // 0x50: Disable - 0x51 = weakest - 0x5F = strongest
                            int strength = (int)(Trq* MAX_LEVEL);
                            OutputEffectCommand = 0x50 + strength;
                        }
                    }
                    break;

                case FFBStates.RAMP: {
                        // No translation? Use Vibrate?
                        if ((this.Timer.Tick%2)==0) {

                            OutputEffectCommand = 0x50;
                        } else {

                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;
                case FFBStates.FRICTION: {
                        // Translated to friction
                        // Select gain according to sign of velocity
                        if (W < 0)
                            Trq = RunningEffect.NegativeCoef_u;
                        else
                            Trq = RunningEffect.PositiveCoef_u;

                        // Scale in range and apply global gains before leaving
                        Trq = Math.Min(Math.Abs(RunningEffect.GlobalGain * Trq), 1.0);
                        // Trq is now in [0; 1]

                        // Friction strength – SendFriction
                        // 0x20: Disable - 0x21 = weakest - 0x2F = strongest
                        int strength = (int)(Trq* MAX_LEVEL);
                        OutputEffectCommand = 0x20 + strength;

                    }
                    break;
                case FFBStates.INERTIA: {
                        // No translation? Use friction?
                    }
                    break;
                case FFBStates.SPRING: {
                        // Translated to auto-centering
                        // Add Offset to reference position, then substract measure to
                        // get relative error sign
                        var error = (R + RunningEffect.Offset_u) - P;
                        // Select gain according to sign of error
                        // (maybe should be motion/velocity?)
                        if (error < 0)
                            Trq = RunningEffect.NegativeCoef_u;
                        else
                            Trq = RunningEffect.PositiveCoef_u;

                        // Scale in range and apply global gains before leaving
                        Trq = Math.Min(Math.Abs(RunningEffect.GlobalGain * Trq), 1.0);
                        // Trq is now in [0; 1]

                        // Set centering strength - auto-centering – SendSelfCenter
                        //

                        int strength = (int)(Trq* MAX_LEVEL);
                        OutputEffectCommand = 0x30 + strength;

                    }
                    break;
                case FFBStates.DAMPER: {
                        // No trnaslation? Use Spring?

                    }
                    break;


                case FFBStates.SINE:
                case FFBStates.SQUARE:
                case FFBStates.TRIANGLE:
                case FFBStates.SAWTOOTHUP:
                case FFBStates.SAWTOOTHDOWN: {
                        // All effects are translated to vibrations
                        Trq = RunningEffect.Magnitude;

                        // Scale in range and apply global gains before leaving
                        Trq = Math.Min(Math.Abs(RunningEffect.GlobalGain * Trq), 1.0);
                        // Trq is now in [0; 1]

                        // Uncentering (vibrate)- SendVibrate
                        int strength = (int)(Trq* MAX_LEVEL);
                        // Emulated for Lemans:
                        // Manually "vibrate" each 8 ticks
                        if ((this.Timer.Tick >> 3) % 2 == 0) {
                            // Rotate wheel left - SendConstantForce(-)
                            // 0x60: Disable - 0x61 = weakest - 0x6F = strongest
                            OutputEffectCommand = 0x60 + strength;
                        } else {
                            // Rotate wheel right – SendConstantForce (+)
                            // 0x50: Disable - 0x51 = weakest - 0x5F = strongest
                            OutputEffectCommand = 0x50 + strength;
                        }
                    }
                    break;

                default:
                    break;
            }


            RunningEffect._LocalTime_ms += Timer.Period_ms;
            if (this.State != FFBStates.NO_EFFECT) {
                if (RunningEffect.Duration_ms >= 0.0 &&
                    RunningEffect._LocalTime_ms > RunningEffect.Duration_ms) {
                    TransitionTo(FFBStates.NO_EFFECT);
                }
            }

        }



        protected void State_INIT()
        {
            switch (Step) {
                case 0:
                    ResetEffect();
                    // Echo test
                    OutputEffectCommand = 0xFF;
                    TimeoutTimer.Restart();
                    GoToNextStep();
                    break;
                case 1:
                    if (TimeoutTimer.ElapsedMilliseconds>1000) {
                        // Play sequence ?
                        OutputEffectCommand = 0x00;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 2:
                    if (TimeoutTimer.ElapsedMilliseconds>3000) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = 0x10;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 3:
                    if (TimeoutTimer.ElapsedMilliseconds>100) {
                        // Maximum power set to 100%
                        //OutputEffectCommand = 75;
                        GoToNextStep();
                    }
                    break;
                case 4:
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
            }
        }
    }
}


