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
    /// 0xFF = ping - keep previous effect
    /// 
    /// </summary>
    public class FFBManagerModel3Lemans :
        IFFBManager
    {

        public bool UseTrqEmulation = true;
        public bool UsePulseSeq = true;

        protected const int MAX_LEVEL = 0x7;
        protected int[,] PulseSequences = new int[,] {
            {1, 0, 0, 0 },
            {1, 0, 1, 0 },
            {1, 1, 1, 0 },
            {1, 1, 1, 1 },
        };

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

                        // Set flag to convert it to constant torque cmd
                        translTrq2Cmd = true;
                    }
                    break;

                case FFBStates.RAMP: {
                        if (UseTrqEmulation) {
                            // Ramp torque from start to end given a duration.
                            // Let 's' be the normalized time ratio between 0
                            // (start) and end (1.0) of the ramp.
                            // T = Start*(1-s) + End*s
                            // ^-- Start when s=0, End when s=1.
                            double time_ratio = RunningEffect._LocalTime_ms / RunningEffect.Duration_ms;
                            double value = RunningEffect.RampStartLevel_u * (1.0 - time_ratio) +
                                           RunningEffect.RampEndLevel_u * (time_ratio);
                            Trq = value;
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
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
                        if (UseTrqEmulation) {
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
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
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
                        OutputEffectCommand = 0x30 + strength;
                    }
                    break;
                case FFBStates.DAMPER: {
                        if (UseTrqEmulation) {
                            // Add torque in opposition to 
                            // current accel and speed (friction)
                            // T = -K2 x W -K3 x I x A

                            // Add friction/damper effect in opposition to motion
                            var k1 = 0.2;
                            var k2 = 0.2;
                            // Deadband for slow speed?
                            if ((Math.Abs(W) > MinVelThreshold) || (Math.Abs(A) > MinAccThreshold))
                                Trq = -k1 * W - Math.Sign(W) * k2 * Inertia * Math.Abs(A);
                            // Saturation
                            Trq = Math.Min(RunningEffect.PositiveSat_u, Trq);
                            Trq = Math.Max(RunningEffect.NegativeSat_u, Trq);

                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;


                case FFBStates.SINE: {
                        if (UseTrqEmulation) {
                            // Get phase in radians
                            double phase_rad = (Math.PI/180.0) * (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms));
                            Trq = Math.Sin(phase_rad)*RunningEffect.Magnitude + RunningEffect.Offset_u;

                            // Saturation
                            Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;
                case FFBStates.SQUARE: {
                        if (UseTrqEmulation) {
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
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;
                case FFBStates.TRIANGLE: {
                        if (UseTrqEmulation) {
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
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;
                case FFBStates.SAWTOOTHUP: {
                        if (UseTrqEmulation) {
                            // Get phase in degrees
                            double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                            double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
                            // Ramping up triangle given phase value between
                            Trq = RunningEffect.Magnitude*(2.0*time_ratio-1.0) + RunningEffect.Offset_u;
                            // Saturation
                            Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;
                case FFBStates.SAWTOOTHDOWN: {
                        if (UseTrqEmulation) {
                            // Get phase in degrees
                            double phase_deg = (RunningEffect.PhaseShift_deg + 360.0*(RunningEffect._LocalTime_ms / RunningEffect.Period_ms)) % 360.0;
                            double time_ratio = Math.Abs(phase_deg) * (1.0/360.0);
                            // Ramping up triangle given phase value between
                            Trq = RunningEffect.Magnitude*(1.0-2.0*time_ratio) + RunningEffect.Offset_u;
                            // Saturation
                            Trq = Math.Min(1.0, Math.Max(-1.0, Trq));
                            // Set flag to convert it to constant torque cmd
                            translTrq2Cmd = true;
                            // All done
                        } else {
                            // No effect
                            OutputEffectCommand = 0x10;
                        }
                    }
                    break;

                default:
                    break;
            }


            // If using Trq value, then convert to constant torque effect
            if (translTrq2Cmd) {

                // Dead-band for very small torque values
                if (Math.Abs(Trq)< (1.0/(MAX_LEVEL<<3))) {

                    // No effect
                    OutputEffectCommand = 0x10;

                } else if (Trq<0) {

                    // If using fast switching between levels
                    if (UsePulseSeq) {

                        // Resolve to 4x more levels, then make pulses
                        int levels = (int)(-Trq*((MAX_LEVEL<<2)+3));
                        int reminder = levels&0b11; //0..3

                        int upcmd = 0x60 + (levels>>2);
                        int downcmd = upcmd-1;
                        if (downcmd<0x60)
                            downcmd = 0x10; //resolve to no torque

                        // Take the sub-period we are in
                        int subper = (int)(this.Timer.Tick&0b11);
                        var pulse = PulseSequences[reminder, subper];
                        if (pulse==1) {
                            OutputEffectCommand = upcmd;
                        } else {
                            OutputEffectCommand = downcmd;
                        }

                    } else {

                        // Rotate wheel left - SendConstantForce(-)
                        int strength = (int)(-Trq* MAX_LEVEL);
                        OutputEffectCommand = 0x60 + strength;

                    }

                } else {

                    // If using fast switching UseFastSwitching
                    if (UsePulseSeq) {

                        // Resolve to 4x more levels, then make pulses
                        int levels = (int)(Trq*((MAX_LEVEL<<2)+3));
                        int reminder = levels&0b11; //0..3

                        int upcmd = 0x50 + (levels>>2);
                        int downcmd = upcmd-1;
                        if (downcmd<0x50)
                            downcmd = 0x10; //resolve to no torque

                        // Take the sub-period we are in
                        int subper = (int)(this.Timer.Tick&0b11);
                        var pulse = PulseSequences[reminder, subper];
                        if (pulse==1) {
                            OutputEffectCommand = upcmd;
                        } else {
                            OutputEffectCommand = downcmd;
                        }

                    } else {

                        // Rotate wheel right – SendConstantForce (+)
                        int strength = (int)(Trq* MAX_LEVEL);
                        OutputEffectCommand = 0x50 + strength;

                    }

                }
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


