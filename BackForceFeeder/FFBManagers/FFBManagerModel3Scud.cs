//#define CONSOLE_DUMP

using System;
using System.Globalization;
using System.Threading;

// Don't forget to add this
using BackForceFeeder.Utils;

namespace BackForceFeeder.FFBManagers
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
            INTENSITY = 0x20,
            SPRING = 0x10,
            TURNLEFT = 0x60,
            TURNRIGHT = 0x50,
            WAITING = 0xC6,
            ENABLE = 0xC7,
            RESETBOARD = 0xCB,
        }



        /// <summary>
        /// Wheel sign is in opposite direction
        /// </summary>
        /// <param name="refreshPeriod_ms"></param>
        public FFBManagerModel3Scud(int refreshPeriod_ms) :
            base(refreshPeriod_ms)
        {
            this.MAX_LEVEL = 0x7;
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
                        OutputEffectCommand = (long)0x20;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.FRICTION: {
                            if (ForceTrqForAllCommands) {
                                Trq = TrqFromFriction(i, W);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // Translated to friction
                                // Select gain according to sign of velocity
                                if (W < 0)
                                    Trq = RunningEffects[i].NegativeCoef_u;
                                else
                                    Trq = RunningEffects[i].PositiveCoef_u;

                                // Scale in range and apply global gains before leaving
                                Trq = Math.Min(Math.Abs(Trq * RunningEffects[i].Gain * DeviceGain), 1.0);
                                // Trq is now in [0; 1]

                                // Friction strength – SendFriction
                                // 0x20: Disable - 0x21 = weakest - 0x2F = strongest
                                int strength = (int)(Trq* MAX_LEVEL);
                                OutputEffectCommand = (long)ScudCMD.INTENSITY + strength;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
                            }
                        }
                        break;
                    case EffectTypes.SPRING: {
                            if (ForceTrqForAllCommands) {
                                Trq = TrqFromSpring(i, R, P);
                                // Set flag to convert it to constant torque cmd
                                translTrq2Cmd = true;
                            } else {
                                // Translated to auto-centering
                                // Add Offset to reference position, then substract measure to
                                // get relative error sign
                                var error = (R + RunningEffects[i].Offset_u) - P;
                                // Select gain according to sign of error
                                // (maybe should be motion/velocity?)
                                if (error < 0)
                                    Trq = RunningEffects[i].NegativeCoef_u;
                                else
                                    Trq = RunningEffects[i].PositiveCoef_u;

                                // Scale in range and apply global gains before leaving
                                Trq = Math.Min(Math.Abs(Trq * RunningEffects[i].Gain * DeviceGain), 1.0);
                                // Trq is now in [0; 1]

                                // Set centering strength - auto-centering – SendSelfCenter
                                // 0x10: Disable – 0x11 = weakest – 0x1F = strongest

                                int strength = (int)(Trq* MAX_LEVEL);
                                OutputEffectCommand = (long)ScudCMD.SPRING + strength;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                                OutputEffectCommand = (long)ScudCMD.NO_EFFECT;
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
                TrqToCommand((int)ScudCMD.NO_EFFECT, (int)ScudCMD.TURNLEFT, (int)ScudCMD.TURNRIGHT);
            }

            this.CheckForEffectsDone();
        }

        protected int LastPositiveSign = 0;
        protected int SkipCounter = 0;

        protected override void TrqToCommand(
            int CmdNoTorque = (int)ScudCMD.NO_EFFECT,
            int CmdTurnLeft = (int)ScudCMD.TURNLEFT,
            int CmdTurnRight = (int)ScudCMD.TURNRIGHT)
        {
            // Trq is now in [-1; 1]
            double Trq = OutputTorqueLevel;
            const int max_level = 0x3F; // MAX_LEVEL+1

            int isPositive = (Trq>0) ? (1) : (-1);
            int levels = (int)(Math.Abs(Trq)*(max_level));
            int int_part = (levels>>2)&0xF; // 0..F for 0x2X
            int frac_part = (levels>>3)&0x7; //0..7 for 0x5X or 0x6X
                                             // Take the sub-period we are in


            // Par défaut, changement du couple
            if (LastPositiveSign != isPositive) {
                LastPositiveSign = isPositive;
                // Changement de signe + intensité globale du couple
                if (isPositive>0)
                    OutputEffectCommand = CmdTurnLeft + 0x7;
                else
                    OutputEffectCommand = CmdTurnRight + 0x7;
                SkipCounter = 2;
            } else {
                if (SkipCounter>0) {
                    SkipCounter--;
                } else {
                    // Dead-band for very small torque values
                    if ((Math.Abs(Trq) < (1.0/((max_level)<<3))) ||
                        (Math.Abs(Trq) < TrqDeadBand)) {
                        // No effect
                        OutputEffectCommand = 0x20;

                    } else {
                        OutputEffectCommand = 0x20 + int_part;
                    }
                    //SkipCounter = 1;
                }
            }
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
                        OutputEffectCommand = (long)ScudCMD.RESETBOARD;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 3:
                    if (TimeoutTimer.ElapsedMilliseconds > 1700) {
                        // 0xCB: reset board - SendStopAll
                        OutputEffectCommand = (long)0x7E;
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
                        OutputEffectCommand = (long)ScudCMD.WAITING;
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
                    if (TimeoutTimer.ElapsedMilliseconds > 3000) {
                        // Maximum power set to 100%
                        OutputEffectCommand = (long)GenericModel3CMD.MOTOR_LEVEL100;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 10:
                    if (TimeoutTimer.ElapsedMilliseconds > 170) {
                        OutputEffectCommand = (long)0xC6;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 11:
                    if (TimeoutTimer.ElapsedMilliseconds > 100) {
                        OutputEffectCommand = (long)0xC7;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 12:
                    if (TimeoutTimer.ElapsedMilliseconds > 50) {
                        OutputEffectCommand = (long)0x00;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 13:
                    if (TimeoutTimer.ElapsedMilliseconds > 100) {
                        // Centering
                        //OutputEffectCommand = (long)0x10;
                        TimeoutTimer.Restart();
                        GoToNextStep();
                    }
                    break;
                case 14:
                    TransitionTo(FFBStates.DEVICE_READY);
                    break;
            }
        }
        protected override void State_DISABLE()
        {
            switch (Step) {
                case 0:
                    OutputEffectCommand = (long)0x20;
                    break;
            }
        }
        protected override void State_READY()
        {
            switch (Step) {
                case 0:
                    OutputEffectCommand = (long)0x20;
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


