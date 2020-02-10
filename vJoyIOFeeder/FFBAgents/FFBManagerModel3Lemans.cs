//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.FFBAgents
{
    /// <summary>
    /// Testing results on real hardware:
    /// only 8 level of effects (x = 0..7)
    /// 
    /// 0x00 sequence in selection screen?
    /// 0x1x no effect
    /// 0x2x = friction/clutch?
    /// 0x3x = spring
    /// 0x5x, 0x6X = constant torque turn left (pos)/right (neg)
    /// 0xFF = ping - keep previous effect
    /// 
    /// </summary>
    public class FFBManagerModel3Lemans :
        FFBManagerModel3
    {
        /// <summary>
        /// Known commands for Le mans DriveBoard ROM
        /// </summary>
        public enum LemansCMD : int
        {
            SEQU = 0x00,
            NO_EFFECT = 0x10,
            FRICTION=0x20,
            SPRING = 0x30,
            TURNLEFT = 0x50,
            TURNRIGHT = 0x60,
        }



        /// <summary>
        /// Wheel sign is in opposite direction
        /// </summary>
        /// <param name="refreshPeriod_ms"></param>
        public FFBManagerModel3Lemans(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
            this.MAX_LEVEL = 0x7;
            this.WheelSign = -1.0;
            this.MinVelThreshold = 0.2;
            this.MinAccThreshold = 0.1;
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

            // If using torque emulated mode, then effect will fill "Trq" and
            // the value will be converted to a left/right torque command
            bool translTrq2Cmd = false;

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
                    OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                    break;
                case FFBStates.DEVICE_READY:
                    OutputEffectCommand = (long)GenericModel3CMD.PING;
                    break;

                case FFBStates.NO_EFFECT:
                    OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                    break;

                case FFBStates.CONSTANT_TORQUE: {
                        Trq = TrqFromConstant();
                        // Set flag to convert it to constant torque cmd
                        translTrq2Cmd = true;
                    }
                    break;

                case FFBStates.RAMP: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromRamp();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
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
                        OutputEffectCommand = (long)LemansCMD.FRICTION + strength;
                    }
                    break;
                case FFBStates.INERTIA: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromInertia(W, this.RawSpeed_u_per_s, A, 0.2, 0.1, 50.0);
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
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
                        OutputEffectCommand = (long)LemansCMD.SPRING + strength;
                    }
                    break;
                case FFBStates.DAMPER: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromDamper(W, A, 0.3, 0.5);
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;


                case FFBStates.SINE: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromSine();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;
                case FFBStates.SQUARE: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromSquare();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;
                case FFBStates.TRIANGLE: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromTriangle();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;
                case FFBStates.SAWTOOTHUP: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromSawtoothUp();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;
                case FFBStates.SAWTOOTHDOWN: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromSawtoothDown();
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = (long)LemansCMD.NO_EFFECT;
                        }
                    }
                    break;

                default:
                    break;
            }

            // If using Trq value, then convert to constant torque effect
            if (translTrq2Cmd) {
                // Change sign of torque if inverted
                Trq = this.TrqSign * Trq;
                // Scale in range and apply global gains
                Trq = Math.Max(Math.Min(RunningEffect.GlobalGain * Trq, 1.0), -1.0);
                // Save value
                OutputTorqueLevel = Trq;
                // Now convert to command
                TrqToCommand((int)LemansCMD.NO_EFFECT, (int)LemansCMD.TURNLEFT, (int)LemansCMD.TURNRIGHT);
            }

            this.FFBEffectsEndStateMachine();
        }


        /// <summary>
        /// Specific lemans
        /// </summary>
        protected void State_INIT()
        {
            switch (Step) {
                case 0:
                    ResetEffect();
                    // Echo test
                    OutputEffectCommand = (int)GenericModel3CMD.PING;
                    TimeoutTimer.Restart();
                    GoToNextStep();
                    break;
                case 1:
                    if (TimeoutTimer.ElapsedMilliseconds>2000) {
                        // Play sequence ?
                        OutputEffectCommand = (int)LemansCMD.SEQU;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 2:
                    if (TimeoutTimer.ElapsedMilliseconds>1000) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = (int)LemansCMD.NO_EFFECT;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 3:
                    if (TimeoutTimer.ElapsedMilliseconds>100) {
                        // Maximum power set to 100%
                        OutputEffectCommand = (long)GenericModel3CMD.MOTOR_LEVEL100;
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


