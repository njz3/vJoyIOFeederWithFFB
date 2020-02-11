//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.FFBAgents
{
    /// <summary>
    /// See :
    /// http://superusr.free.fr/model3.htm
    /// </summary>
    public class FFBManagerModel3Scud :
        FFBManagerModel3
    {
        public enum ScudCMD : int
        {
            SEQU = 0x00,
            NO_EFFECT = 0x10,
            FRICTION=0x20,
            SPRING = 0x10,
            TURNLEFT = 0x60,
            TURNRIGHT = 0x50,
            WAITING = 0xC1,
            RESETBOARD = 0xCB,
        }
       


        /// <summary>
        /// Wheel sign is in opposite direction
        /// </summary>
        /// <param name="refreshPeriod_ms"></param>
        public FFBManagerModel3Scud(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
            this.MAX_LEVEL = 0xF;
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
                    OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
                    break;
                case FFBStates.DEVICE_READY:
                    OutputEffectCommand = (long)ScudCMD.WAITING;
                    break;

                case FFBStates.NO_EFFECT:
                    OutputEffectCommand = (long)ScudCMD.WAITING;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                        OutputEffectCommand = (long)ScudCMD.FRICTION + strength;
                    }
                    break;
                case FFBStates.INERTIA: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromInertia(W, this.RawSpeed_u_per_s, A, 0.2, 0.1, 50.0);
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                        // 0x10: Disable – 0x11 = weakest – 0x1F = strongest

                        int strength = (int)(Trq* MAX_LEVEL);
                        OutputEffectCommand = (long)ScudCMD.SPRING + strength;
                    }
                    break;
                case FFBStates.DAMPER: {
                        if (UseTrqEmulation) {
                            Trq = TrqFromDamper(W, A, 0.2, 0.4);
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                            OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                TrqToCommand((int)ScudCMD.NO_EFFECT, (int)ScudCMD.TURNLEFT, (int)ScudCMD.TURNRIGHT);
                }

            this.FFBEffectsEndStateMachine();
        }


        /// <summary>
        /// Specific Scud/Daytona2
        /// </summary>
        protected override void State_INIT()
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
                    if (TimeoutTimer.ElapsedMilliseconds>1000) {
                        // Play sequence ?
                        OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 2:
                    if (TimeoutTimer.ElapsedMilliseconds>3000) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = (long)ScudCMD.RESETBOARD;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 3:
                    OutputEffectCommand = 0x7E; //?
                    TimeoutTimer.Restart();
                    GoToNextStep();
                    break;
                case 4:
                    if (TimeoutTimer.ElapsedMilliseconds>50) {
                        // Test mode 0x80 Stop motor SendStopAll
                        OutputEffectCommand = 0x80;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 5:
                    if (TimeoutTimer.ElapsedMilliseconds>100) {
                        // Send cabinet type ?
                        OutputEffectCommand = 0xB1;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 6:
                    if (TimeoutTimer.ElapsedMilliseconds>100) {
                        // Waiting for game start
                        OutputEffectCommand = (long)ScudCMD.WAITING;
                        GoToNextStep();
                    }
                    break;
                case 7:
                    if (TimeoutTimer.ElapsedMilliseconds>100) {
                        // Maximum power set to 100%
                        OutputEffectCommand = (long)GenericModel3CMD.MOTOR_LEVEL100;
                        GoToNextStep();
                    }
                    break;
                case 8:
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
            }
        }
    }
}


