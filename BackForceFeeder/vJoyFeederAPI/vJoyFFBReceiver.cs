//#define DUMP_FFB_FRAME

using BackForceFeeder.FFBManagers;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using System;
using System.Runtime.InteropServices;
// Don't forget to add this
using vJoyInterfaceWrap;

namespace BackForceFeeder.vJoyIOFeederAPI
{
    /// <summary>
    /// Force feedback receiver from vJoy interface
    /// </summary>
    public class vJoyFFBReceiver
    {
        vJoy.FFB_DEVICE_PID PIDBlock = new vJoy.FFB_DEVICE_PID();
        protected FFBManager FFBManager;
        protected vJoy Joystick;
        protected uint Id;
        protected vJoy.FfbCbFunc wrapper;
        protected bool isRegistered = false;
        protected const Byte BLOCK_INDEX_FIRST_ID = 1;

        /// <summary>
        /// Scale FFB units, between -10000/+10000 to unit values
        /// between -1/+1.
        /// This is to help performs computations on a normalized
        /// scaled value, independant of the wheel ratio or
        /// digital values.
        /// </summary>
        protected double Scale_FFB_to_u = (1.0/10000.0);



        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[FFBRECV] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[FFBRECV] " + text, args);
        }

        public vJoyFFBReceiver()
        {
            PIDBlock.PIDBlockLoad.EffectBlockIndex = BLOCK_INDEX_FIRST_ID;
            PIDBlock.PIDBlockLoad.LoadStatus = 0;
            PIDBlock.PIDBlockLoad.RAMPoolAvailable = 0xFFFF;
            PIDBlock.PIDPool.MaxSimultaneousEffects = 5;
            PIDBlock.EffectStates = new vJoy.FFB_PID_EFFECT_STATE_REPORT[vJoy.VJOY_FFB_MAX_EFFECTS_BLOCK_INDEX];
            for (int i = 0; i<PIDBlock.EffectStates.Length; i++) {
                PIDBlock.EffectStates[i].State = 0;
            }
        }

        /// <summary>
        /// Register the base callback if not yet registered.
        /// </summary>
        public void RegisterBaseCallback(vJoy joystick, uint id, FFBManager ffb)
        {
            this.FFBManager = ffb;
            this.Joystick = joystick;
            this.Id = id;
            // Read PID block
            this.Joystick.FfbReadPID(this.Id, ref this.PIDBlock);

            if (!isRegistered) {
                this.wrapper = this.FfbReceiverFunction; //needed to keep a reference!
                Joystick.FfbRegisterGenCB(this.wrapper, IntPtr.Zero);
                this.isRegistered = true;
            }
        }

#if DUMP_FFB_FRAME
        public void DumpFrame(IntPtr data)
        {
            unsafe {
                InternalFfbPacket* FfbData = (InternalFfbPacket*)data;
                int size = FfbData->DataSize;
                int command = (int)FfbData->Command;
                byte* bytes = (byte*)FfbData->PtrToData;
                StringBuilder line = new StringBuilder();
                line.AppendFormat(String.Format("FFB Size {0}", size));
                line.AppendFormat(" Cmd:" + String.Format("{0:X08}", (int)FfbData->Command));
                line.AppendFormat(" ID:" + String.Format("{0:X02}", command));
                line.AppendFormat(" Size:" + String.Format("{0:D02}", (int)(size - 8)));
                line.AppendFormat(" -");
                for (uint i = 0; i < size - 8; i++)
                    line.AppendFormat(String.Format(" {0:X02}", (uint)(bytes[i])));

                Log(line.ToString());
            }
        }
#endif
        /// <summary>
        /// HID descriptor type: feature or report
        /// </summary>
        private enum CommandType : int
        {
            IOCTL_HID_SET_FEATURE = 0xB0191,
            IOCTL_HID_WRITE_REPORT = 0xB000F
        }
        /// <summary>
        /// Aligned struct for marshaling of raw packets back to C#
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct InternalFfbPacket
        {
            public int DataSize;
            public CommandType Command;
            public IntPtr PtrToData;
        }

        protected enum ERROR : uint
        {
            ERROR_SUCCESS = 0,
        }

        
        /// <summary>
        /// Called when vJoy has a new FFB packet.
        /// WARNING This is called from a thread pool managed by windows.
        /// The thread itself is created and managed by vJoyInterface.dll.
        /// Do not overload it, else you will me missing FFB packets from
        /// third party application.
        /// </summary>
        /// <param name="ffbDataPtr"></param>
        /// <param name="userData"></param>
        public void FfbReceiverFunction(IntPtr data, object userdata)
        {
            // Packet Header
            //copy ffb packet to managed structure
            InternalFfbPacket packet = (InternalFfbPacket)Marshal.PtrToStructure(data, typeof(InternalFfbPacket));

            /////// Packet Device ID, and Type Block Index (if exists)
            #region Packet Device ID, and Type Block Index
#if DUMP_FFB_FRAME
            if (vJoyManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                DumpFrame(data);
            }
#endif

            uint DeviceID = 0, BlockIndex = 0;
            FFBPType Type = new FFBPType();

            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_DeviceID(data, ref DeviceID)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " > Device ID: {0}", DeviceID);
                }
            }

            // Effect block index only used when simultaneous effects should be done by
            // underlying hardware, which is not the case for a single motor driving wheel
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_EffectBlockIndex(data, ref BlockIndex)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " > Effect Block Index: {0}", BlockIndex);
                }
            }

            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Type(data, ref Type)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    if (!PacketType2Str(Type, out var TypeStr))
                        LogFormat(LogLevels.DEBUG, " > Packet Type: {0}", Type);
                    else
                        LogFormat(LogLevels.DEBUG, " > Packet Type: {0}", TypeStr);
                }
                switch (Type) {
                    case FFBPType.PT_POOLREP:
                        if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                            LogFormat(LogLevels.DEBUG, " > Pool report handled by driver side");
                        }
                        break;
                    case FFBPType.PT_BLKLDREP:
                        if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                            LogFormat(LogLevels.DEBUG, " > Block Load report handled by driver side");
                        }
                        break;
                    case FFBPType.PT_BLKFRREP:
                        FFBManager.FreeEffect(BlockIndex);
                        // Update PID
                        Joystick.FfbReadPID(DeviceID, ref PIDBlock);
                        if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                            LogFormat(LogLevels.DEBUG, " > Block Free effect id {0}", PIDBlock.NextFreeEID);
                        }
                        break;
                }
            }

            

            #endregion

            #region PID Device Control
            FFB_CTRL Control = new FFB_CTRL();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_DevCtrl(data, ref Control) && DevCtrl2Str(Control, out var CtrlStr)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> PID Device Control: {0}", CtrlStr);
                }
                switch (Control) {
                    case FFB_CTRL.CTRL_DEVRST:
                        // Update PID data to get the resetted values from driver side
                        Joystick.FfbReadPID(DeviceID, ref PIDBlock);
                        // device reset
                        FFBManager.DevReset();
                        break;
                    case FFB_CTRL.CTRL_ENACT:
                        FFBManager.DevEnable();
                        break;
                    case FFB_CTRL.CTRL_DISACT:
                        FFBManager.DevDisable();
                        break;
                    case FFB_CTRL.CTRL_STOPALL:
                        FFBManager.StopAllEffects();
                        break;
                }
            }

            #endregion

            #region Create new effect
            FFBEType EffectType = new FFBEType();
            uint NewBlockIndex = 0;
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_CreateNewEffect(data, ref EffectType, ref NewBlockIndex)) {
                FFBManager.CreateNewEffect(NewBlockIndex);
                // Update PID
                Joystick.FfbReadPID(Id, ref PIDBlock);

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    if (EffectType2Str(EffectType, out var TypeStr))
                        LogFormat(LogLevels.DEBUG, " >> Effect Type: {0}", TypeStr);
                    else
                        LogFormat(LogLevels.DEBUG, " >> Effect Type: Unknown({0})", EffectType);
                    LogFormat(LogLevels.DEBUG, " >> New Effect ID: {0}", NewBlockIndex);
                    if (NewBlockIndex != PIDBlock.PIDBlockLoad.EffectBlockIndex) {
                        LogFormat(LogLevels.DEBUG, "!!! BUG NewBlockIndex=" + NewBlockIndex + " <> pid=" + ((int)PIDBlock.PIDBlockLoad.EffectBlockIndex));
                    }
                    LogFormat(LogLevels.DEBUG, " >> LoadStatus {0}", PIDBlock.PIDBlockLoad.LoadStatus);
                }
            }
            #endregion

            #region Condition
            vJoy.FFB_EFF_COND Condition = new vJoy.FFB_EFF_COND();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Cond(data, ref Condition)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    if (Condition.isY)
                        LogFormat(LogLevels.DEBUG, " >> Y Axis");
                    else
                        LogFormat(LogLevels.DEBUG, " >> X Axis");
                    LogFormat(LogLevels.DEBUG, " >> Center Point Offset: {0}", TwosCompWord2Int(Condition.CenterPointOffset));
                    LogFormat(LogLevels.DEBUG, " >> Positive Coefficient: {0}", TwosCompWord2Int(Condition.PosCoeff));
                    LogFormat(LogLevels.DEBUG, " >> Negative Coefficient: {0}", TwosCompWord2Int(Condition.NegCoeff));
                    LogFormat(LogLevels.DEBUG, " >> Positive Saturation: {0}", Condition.PosSatur);
                    LogFormat(LogLevels.DEBUG, " >> Negative Saturation: {0}", Condition.NegSatur);
                    LogFormat(LogLevels.DEBUG, " >> Dead Band: {0}", Condition.DeadBand);
                }
                // Skip all processing if Y axis (single axis for wheel FFB!)
                if (Condition.isY) {
                    // Leave early!
                    return;
                }

                FFBManager.SetLimitsParams(BlockIndex,
                    TwosCompWord2Int(Condition.CenterPointOffset) * Scale_FFB_to_u,
                    Condition.DeadBand * Scale_FFB_to_u,
                    TwosCompWord2Int(Condition.PosCoeff) * Scale_FFB_to_u,
                    TwosCompWord2Int(Condition.NegCoeff) * Scale_FFB_to_u,
                    Condition.PosSatur * Scale_FFB_to_u,
                    -Condition.NegSatur * Scale_FFB_to_u);

            }
            #endregion

            #region Effect Report
            vJoy.FFB_EFF_REPORT Effect = new vJoy.FFB_EFF_REPORT();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Report(data, ref Effect)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    if (!EffectType2Str(Effect.EffectType, out var TypeStr))
                        LogFormat(LogLevels.DEBUG, " >> Effect Report: {0} {1}", (int)Effect.EffectType, Effect.EffectType.ToString());
                    else
                        LogFormat(LogLevels.DEBUG, " >> Effect Report: {0}", TypeStr);
                    LogFormat(LogLevels.DEBUG, " >> AxisEnabledDirection: {0}", (ushort)Effect.AxesEnabledDirection);
                }
                if (Effect.Polar) {
                    if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                        LogFormat(LogLevels.DEBUG, " >> Direction: {0} deg ({1})", Polar2Deg(Effect.Direction), Effect.Direction);
                    }
                    FFBManager.SetDirection(BlockIndex, Polar2Deg(Effect.Direction));
                } else {
                    if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                        LogFormat(LogLevels.DEBUG, " >> X Direction: {0}", Effect.DirX);
                        LogFormat(LogLevels.DEBUG, " >> Y Direction: {0}", Effect.DirY);
                    }
                    FFBManager.SetDirection(BlockIndex, Effect.DirX);
                }

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    if (Effect.Duration == 0xFFFF)
                        LogFormat(LogLevels.DEBUG, " >> Duration: Infinit");
                    else
                        LogFormat(LogLevels.DEBUG, " >> Duration: {0} MilliSec", (int)(Effect.Duration));

                    if (Effect.TrigerRpt == 0xFFFF)
                        LogFormat(LogLevels.DEBUG, " >> Trigger Repeat: Infinit");
                    else
                        LogFormat(LogLevels.DEBUG, " >> Trigger Repeat: {0}", (int)(Effect.TrigerRpt));

                    if (Effect.SamplePrd == 0xFFFF)
                        LogFormat(LogLevels.DEBUG, " >> Sample Period: Infinit");
                    else
                        LogFormat(LogLevels.DEBUG, " >> Sample Period: {0}", (int)(Effect.SamplePrd));

                    if (Effect.StartDelay == 0xFFFF)
                        LogFormat(LogLevels.DEBUG, " >> Start Delay: max ");
                    else
                        LogFormat(LogLevels.DEBUG, " >> Start Delay: {0}", (int)(Effect.StartDelay));


                    LogFormat(LogLevels.DEBUG, " >> Gain: {0}%%", Byte2Percent(Effect.Gain));
                }

                if (Effect.Duration==65535)
                    FFBManager.SetDuration(BlockIndex, -1.0);
                else
                    FFBManager.SetDuration(BlockIndex, Effect.Duration);
                if (Effect.StartDelay == 65535)
                    FFBManager.SetStartDelay(BlockIndex, 0);
                else
                    FFBManager.SetStartDelay(BlockIndex, Effect.StartDelay);

                FFBManager.SetEffectGain(BlockIndex, Byte2Percent(Effect.Gain)*0.01);
                switch (Effect.EffectType) {
                    case FFBEType.ET_CONST:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.CONSTANT_TORQUE);
                        break;
                    case FFBEType.ET_RAMP:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.RAMP);
                        break;
                    case FFBEType.ET_INRT:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.INERTIA);
                        break;
                    case FFBEType.ET_SPRNG:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.SPRING);
                        break;
                    case FFBEType.ET_DMPR:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.DAMPER);
                        break;
                    case FFBEType.ET_FRCTN:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.FRICTION);
                        break;
                    // Periodic
                    case FFBEType.ET_SQR:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.SQUARE);
                        break;
                    case FFBEType.ET_SINE:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.SINE);
                        break;
                    case FFBEType.ET_TRNGL:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.TRIANGLE);
                        break;
                    case FFBEType.ET_STUP:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.SAWTOOTHUP);
                        break;
                    case FFBEType.ET_STDN:
                        FFBManager.SetEffect(BlockIndex, FFBManager.EffectTypes.SAWTOOTHDOWN);
                        break;
                }
            }
            #endregion

            #region Effect Operation
            vJoy.FFB_EFF_OP Operation = new vJoy.FFB_EFF_OP();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_EffOp(data, ref Operation) && EffectOpStr(Operation.EffectOp, out var EffOpStr)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Effect Operation: {0}", EffOpStr);
                    if (Operation.LoopCount == 0xFF)
                        LogFormat(LogLevels.DEBUG, " >> Loop until stopped");
                    else
                        LogFormat(LogLevels.DEBUG, " >> Loop {0} times", (int)(Operation.LoopCount));
                }

                switch (Operation.EffectOp) {
                    case FFBOP.EFF_START:
                        // Start the effect identified by the Effect Handle.
                        FFBManager.StartEffect(BlockIndex, (int)(Operation.LoopCount));
                        break;
                    case FFBOP.EFF_STOP:
                        // Stop the effect identified by the Effect Handle.
                        FFBManager.StopEffect(BlockIndex);
                        break;
                    case FFBOP.EFF_SOLO:
                        // Start the effect identified by the Effect Handle and stop all other effects.
                        FFBManager.StartEffect(BlockIndex, 1);
                        break;
                }

            }
            #endregion

            #region Global Device Gain
            byte Gain = 0;
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_DevGain(data, ref Gain)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Global Device Gain: {0}", Byte2Percent(Gain));
                }
                FFBManager.SetDeviceGain(Byte2Percent(Gain)*0.01);
            }

            #endregion

            #region Envelope
            vJoy.FFB_EFF_ENVLP Envelope = new vJoy.FFB_EFF_ENVLP();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Envlp(data, ref Envelope)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Attack Level: {0}", Envelope.AttackLevel);
                    LogFormat(LogLevels.DEBUG, " >> Fade Level: {0}", Envelope.FadeLevel);
                    LogFormat(LogLevels.DEBUG, " >> Attack Time: {0}", (int)(Envelope.AttackTime));
                    LogFormat(LogLevels.DEBUG, " >> Fade Time: {0}", (int)(Envelope.FadeTime));
                }
                FFBManager.SetEnveloppeParams(BlockIndex, Envelope.AttackTime, Envelope.AttackLevel, Envelope.FadeTime, Envelope.FadeLevel);
            }

            #endregion

            #region Periodic
            vJoy.FFB_EFF_PERIOD EffPrd = new vJoy.FFB_EFF_PERIOD();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Period(data, ref EffPrd)) {

                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Magnitude: {0}", EffPrd.Magnitude);
                    LogFormat(LogLevels.DEBUG, " >> Offset: {0}", TwosCompWord2Int(EffPrd.Offset));
                    LogFormat(LogLevels.DEBUG, " >> Phase: {0}", EffPrd.Phase * 3600 / 255);
                    LogFormat(LogLevels.DEBUG, " >> Period: {0}", (int)(EffPrd.Period));
                }
                FFBManager.SetPeriodicParams(BlockIndex, (double)EffPrd.Magnitude* Scale_FFB_to_u, TwosCompWord2Int(EffPrd.Offset)* Scale_FFB_to_u, EffPrd.Phase * 0.01, EffPrd.Period);
            }
            #endregion

            #region Ramp Effect
            vJoy.FFB_EFF_RAMP RampEffect = new vJoy.FFB_EFF_RAMP();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Ramp(data, ref RampEffect)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Ramp Start: {0}", TwosCompWord2Int(RampEffect.Start));
                    LogFormat(LogLevels.DEBUG, " >> Ramp End: {0}", TwosCompWord2Int(RampEffect.End));
                }
                FFBManager.SetRampParams(BlockIndex, RampEffect.Start * Scale_FFB_to_u, RampEffect.End * Scale_FFB_to_u);
            }

            #endregion

            #region Constant Effect
            vJoy.FFB_EFF_CONSTANT CstEffect = new vJoy.FFB_EFF_CONSTANT();
            if ((uint)ERROR.ERROR_SUCCESS == Joystick.Ffb_h_Eff_Constant(data, ref CstEffect)) {
                if (BFFManager.Config.Application.VerbosevJoyFFBReceiverDumpFrames) {
                    LogFormat(LogLevels.DEBUG, " >> Block Index: {0}", TwosCompWord2Int(CstEffect.EffectBlockIndex));
                    LogFormat(LogLevels.DEBUG, " >> Magnitude: {0}", TwosCompWord2Int(CstEffect.Magnitude));
                }
                FFBManager.SetConstantTorqueEffect(BlockIndex, (double)CstEffect.Magnitude * Scale_FFB_to_u);
            }

            #endregion

        }

        #region Utilities to convert data
        // Convert Packet type to String
        public static bool PacketType2Str(FFBPType Type, out string Str)
        {
            bool stat = true;
            Str = "";

            switch (Type) {
                case FFBPType.PT_EFFREP:
                    Str = "Effect Report";
                    break;
                case FFBPType.PT_ENVREP:
                    Str = "Envelope Report";
                    break;
                case FFBPType.PT_CONDREP:
                    Str = "Condition Report";
                    break;
                case FFBPType.PT_PRIDREP:
                    Str = "Periodic Report";
                    break;
                case FFBPType.PT_CONSTREP:
                    Str = "Constant Force Report";
                    break;
                case FFBPType.PT_RAMPREP:
                    Str = "Ramp Force Report";
                    break;
                case FFBPType.PT_CSTMREP:
                    Str = "Custom Force Data Report";
                    break;
                case FFBPType.PT_SMPLREP:
                    Str = "Download Force Sample";
                    break;
                case FFBPType.PT_EFOPREP:
                    Str = "Effect Operation Report";
                    break;
                case FFBPType.PT_BLKFRREP:
                    Str = "PID Block Free Report";
                    break;
                case FFBPType.PT_CTRLREP:
                    Str = "PID Device Control";
                    break;
                case FFBPType.PT_GAINREP:
                    Str = "Device Gain Report";
                    break;
                case FFBPType.PT_SETCREP:
                    Str = "Set Custom Force Report";
                    break;
                case FFBPType.PT_NEWEFREP:
                    Str = "Create New Effect Report";
                    break;
                case FFBPType.PT_BLKLDREP:
                    Str = "Block Load Report";
                    break;
                case FFBPType.PT_POOLREP:
                    Str = "PID Pool Report";
                    break;
                default:
                    stat = false;
                    break;
            }

            return stat;
        }

        // Convert Effect type to String
        public static bool EffectType2Str(FFBEType Type, out string Str)
        {
            bool stat = true;
            Str = "";

            switch (Type) {
                case FFBEType.ET_NONE:
                    stat = false;
                    break;
                case FFBEType.ET_CONST:
                    Str = "Constant Force";
                    break;
                case FFBEType.ET_RAMP:
                    Str = "Ramp";
                    break;
                case FFBEType.ET_SQR:
                    Str = "Square";
                    break;
                case FFBEType.ET_SINE:
                    Str = "Sine";
                    break;
                case FFBEType.ET_TRNGL:
                    Str = "Triangle";
                    break;
                case FFBEType.ET_STUP:
                    Str = "Sawtooth Up";
                    break;
                case FFBEType.ET_STDN:
                    Str = "Sawtooth Down";
                    break;
                case FFBEType.ET_SPRNG:
                    Str = "Spring";
                    break;
                case FFBEType.ET_DMPR:
                    Str = "Damper";
                    break;
                case FFBEType.ET_INRT:
                    Str = "Inertia";
                    break;
                case FFBEType.ET_FRCTN:
                    Str = "Friction";
                    break;
                case FFBEType.ET_CSTM:
                    Str = "Custom Force";
                    break;
                default:
                    stat = false;
                    break;
            }

            return stat;
        }

        // Convert PID Device Control to String
        public static bool DevCtrl2Str(FFB_CTRL Ctrl, out string Str)
        {
            bool stat = true;
            Str = "";

            switch (Ctrl) {
                case FFB_CTRL.CTRL_ENACT:
                    Str = "Enable Actuators";
                    break;
                case FFB_CTRL.CTRL_DISACT:
                    Str = "Disable Actuators";
                    break;
                case FFB_CTRL.CTRL_STOPALL:
                    Str = "Stop All Effects";
                    break;
                case FFB_CTRL.CTRL_DEVRST:
                    Str = "Device Reset";
                    break;
                case FFB_CTRL.CTRL_DEVPAUSE:
                    Str = "Device Pause";
                    break;
                case FFB_CTRL.CTRL_DEVCONT:
                    Str = "Device Continue";
                    break;
                default:
                    stat = false;
                    break;
            }

            return stat;
        }

        // Convert Effect operation to string
        public static bool EffectOpStr(FFBOP Op, out string Str)
        {
            bool stat = true;
            Str = "";

            switch (Op) {
                case FFBOP.EFF_START:
                    Str = "Effect Start";
                    break;
                case FFBOP.EFF_SOLO:
                    Str = "Effect Solo Start";
                    break;
                case FFBOP.EFF_STOP:
                    Str = "Effect Stop";
                    break;
                default:
                    stat = false;
                    break;
            }

            return stat;
        }

        // Polar values (0x00-0xFF) to Degrees (0-360)
        public static int Polar2Deg(UInt16 Polar)
        {
            return (int)((long)Polar * 360) / 32767;
        }

        // Convert range 0x00-0xFF to 0%-100%
        public static int Byte2Percent(byte InByte)
        {
            return ((byte)InByte * 100) / 255;
        }

        // Convert One-Byte 2's complement input to integer
        public static int TwosCompByte2Int(byte inb)
        {
            int tmp;
            byte inv = (byte)~inb;
            bool isNeg = ((inb >> 7) != 0 ? true : false);
            if (isNeg) {
                tmp = (int)(inv);
                tmp = -1 * tmp;
                return tmp;
            } else
                return (int)inb;
        }

        // Convert One-Byte 2's complement input to integer
        public static int TwosCompWord2Int(short inb)
        {
            int tmp;
            int inv = (int)~inb + 1;
            bool isNeg = ((inb >> 15) != 0 ? true : false);
            if (isNeg) {
                tmp = (int)(inv);
                tmp = -1 * tmp;
                return tmp;
            } else
                return (int)inb;
        }
        #endregion
    }
}
