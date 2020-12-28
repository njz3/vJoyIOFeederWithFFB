//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using BackForceFeeder.Utils;

namespace BackForceFeeder.FFBManagers
{
    /// <summary>
    /// Sega Rally 2 EPR-20512
    /// 
    /// </summary>
    public class FFBManagerModel3SegaRally2 :
        FFBManagerModel3
    {
        public enum SR2CMD : int
        {
            NO_EFFECT = 0x00,
            TURNLEFT = 0x00, //00-3F
            TURNRIGHT = 0x40, //40-7F
            WAITING = 0xC6,
            ENABLE = 0xC7,
            RESETBOARD = 0xCB,
        }


        /// <summary>
        /// Wheel sign is in opposite direction
        /// </summary>
        /// <param name="refreshPeriod_ms"></param>
        public FFBManagerModel3SegaRally2(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
            this.MAX_LEVEL = 0x3F;
        }

        protected override void ComputeTrqFromAllEffects()
        {
            // Inputs:
            // - R=angular reference
            // - P=angular position
            // - W=angular velocity (W=dot(P)=dP/dt)
            // - A=angular accel (A=dot(W)=dW/dt=d2P/dt2)
            // output:
            // OutputEffectCommand

            // Take a snapshot of all values - convert time base to period
            EnterBarrier();
            double R = RefPosition_u;
            double P = FiltPosition_u_0;
            double W = FiltSpeed_u_per_s_0;
            double A = FiltAccel_u_per_s2_0;
            // Release the lock
            ExitBarrier();

            // For model 3 hardware, effects are directly translated to commands
            // for the driveboard

            // If using torque emulated mode, then effect will fill "AllTrq" and
            // the value will be converted to a left/right torque command
            double AllTrq = 0.0;
            bool translTrq2Cmd = false;
            for (int i = 0; i<RunningEffects.Length; i++) {
                // Skip effect not running
                if (!RunningEffects[i].IsRunning || RunningEffects[i]._LocalTime_ms < 0.0) {
                    continue;
                }
                double Trq = 0.0;
                switch (RunningEffects[i].Type) {
                    case EffectTypes.NO_EFFECT:
                        OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                        break;

                    case EffectTypes.CONSTANT_TORQUE: {
                            Trq = TrqFromConstant(i);
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        }
                        break;

                    case EffectTypes.RAMP: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromRamp(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.FRICTION: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromFriction(i, W);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.INERTIA: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromInertia(i, W, this.RawSpeed_u_per_s, A);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.SPRING: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromSpring(i, R, P);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.DAMPER: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromDamper(i, W, this.RawSpeed_u_per_s, A);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;


                    case EffectTypes.SINE: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromSine(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                                // All done
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.SQUARE: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromSquare(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                                // All done
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.TRIANGLE: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromTriangle(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                                // All done
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.SAWTOOTHUP: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromSawtoothUp(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                                // All done
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.SAWTOOTHDOWN: {
                            if (ForceTrqForAllCommands || UseTrqEmulationForMissing) {
                                Trq = TrqFromSawtoothDown(i);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                                // All done
                            } else {
                                // No effect
                                OutputEffectCommand = (long)SR2CMD.NO_EFFECT;
                            }
                        }
                        break;

                    default:
                        break;
                }
                AllTrq += Trq * RunningEffects[i].Gain;
            }

            // If using Trq value, then convert to constant torque effect
            if (translTrq2Cmd) {
                // Change sign of torque if inverted and apply gains
                AllTrq = TrqSign * Math.Sign(AllTrq) * Math.Pow(Math.Abs(AllTrq), PowerLaw) * DeviceGain * GlobalGain;
                // Scale in range
                AllTrq = Math.Max(Math.Min(AllTrq, 1.0), -1.0);
                // Save value
                OutputTorqueLevel = AllTrq;
                // Now convert to command
                TrqToCommand((int)SR2CMD.NO_EFFECT, (int)SR2CMD.TURNLEFT, (int)SR2CMD.TURNRIGHT);
            }

            this.CheckForEffectsDone();
        }




        /// <summary>
        /// Specific Scud/Daytona2
        /// </summary>
        protected override void State_INIT()
        {
            switch (Step) {
                case 0:
                    ResetAllEffects();
                    // Echo test
                    OutputEffectCommand = (long)0xFF;
                    TimeoutTimer.Restart();
                    GoToNextStep();
                    this.Step = 11;
                    break;
                case 1:
                    if (TimeoutTimer.ElapsedMilliseconds>1000) {
                        // Play sequence ?
                        OutputEffectCommand = (long)0x00;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 2:
                    if (TimeoutTimer.ElapsedMilliseconds>3000) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = (long)SR2CMD.RESETBOARD;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 3:
                    if (TimeoutTimer.ElapsedMilliseconds > 1700) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = (long)0xCB;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
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
                    if (TimeoutTimer.ElapsedMilliseconds > 800) {
                        // Waiting for game start
                        OutputEffectCommand = (long)SR2CMD.WAITING;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 7:
                    if (TimeoutTimer.ElapsedMilliseconds > 500) {
                        // Waiting for game start
                        OutputEffectCommand = (long)0xC2;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 8:
                    if (TimeoutTimer.ElapsedMilliseconds > 50) {
                        // Waiting for game start
                        OutputEffectCommand = (long)0xC4;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 9:
                    if (TimeoutTimer.ElapsedMilliseconds > 100) {
                        OutputEffectCommand = (long)0xC7;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 10:
                    if (TimeoutTimer.ElapsedMilliseconds > 50) {
                        OutputEffectCommand = (long)0x00;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 11:
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
            }
        }
        protected override void State_DISABLE()
        {
            switch (Step) {
                case 0:
                    OutputEffectCommand = (long)0x00;
                    break;
            }
        }
        protected override void State_READY()
        {
            switch (Step) {
                case 0:
                    OutputEffectCommand = (long)0x00;
                    break;
            }
        }
        protected override void State_RESET()
        {
            switch (Step) {
                case 0:
                    ResetAllEffects();
                    OutputEffectCommand = (long)0xC7;
                    TimeoutTimer.Restart();
                    GoToNextStep();
                    break;
                case 1:
                    if (TimeoutTimer.ElapsedMilliseconds > 100) {
                        TransitionTo(FFBStates.DEVICE_READY);
                    }
                    break;
            }
        }
    }
}


