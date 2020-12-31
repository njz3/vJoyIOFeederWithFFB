using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// KeyStrokes management
    /// </summary>
    public class KeyStrokesManager
    {
        List<KeyStroke> KeyStrokes;

        /// <summary>
        /// Will be valid at runtime
        /// </summary>

        public KeyStrokesManager()
        {
            KeyStrokes = new List<KeyStroke>();
        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[KEY] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[KEY] " + text, args);
        }

        public static void ProcessKeyStroke(KeyCodes key, KeyEmulationAPI keyAPI, bool newstate, bool oldstate)
        {
            // Leave early if no change
            if (newstate == oldstate)
                return;
            VirtualKeyCode keycode = 0;
            OSUtilities.DInputScanCodes scancode1 = 0;
            OSUtilities.DInputScanCodes scancode2 = 0;
            // Translation table
            switch (key) {
                case Configuration.KeyCodes.AltF4:
                    // Special keycode for combined press
                    if (keyAPI.HasFlag(KeyEmulationAPI.SendInput)) {
                        if (newstate && (!oldstate)) {
                            OSUtilities.SendAltF4();
                        }
                    }
                    if (keyAPI.HasFlag(KeyEmulationAPI.DInput)) {
                        scancode1 = OSUtilities.DInputScanCodes.DIK_LMENU;
                        scancode2 = OSUtilities.DInputScanCodes.DIK_F4;
                    }
                    break;
                case Configuration.KeyCodes.ESC:
                    keycode = VirtualKeyCode.ESCAPE; scancode1 = OSUtilities.DInputScanCodes.DIK_ESCAPE;
                    break;
                case Configuration.KeyCodes.ENTER:
                    keycode = VirtualKeyCode.RETURN; scancode1 = OSUtilities.DInputScanCodes.DIK_RETURN;
                    break;
                case Configuration.KeyCodes.TAB:
                    keycode = VirtualKeyCode.TAB; scancode1 = OSUtilities.DInputScanCodes.DIK_TAB;
                    break;
                case Configuration.KeyCodes.LCTRL:
                    keycode = VirtualKeyCode.LCONTROL; scancode1 = OSUtilities.DInputScanCodes.DIK_LCONTROL;
                    break;
                case Configuration.KeyCodes.RCTRL:
                    keycode = VirtualKeyCode.RCONTROL; scancode1 = OSUtilities.DInputScanCodes.DIK_RCONTROL;
                    break;
                case Configuration.KeyCodes.LSHIFT:
                    keycode = VirtualKeyCode.LSHIFT; scancode1 = OSUtilities.DInputScanCodes.DIK_LSHIFT;
                    break;
                case Configuration.KeyCodes.RSHIFT:
                    keycode = VirtualKeyCode.RSHIFT; scancode1 = OSUtilities.DInputScanCodes.DIK_RSHIFT;
                    break;
                case Configuration.KeyCodes.LALT:
                    keycode = VirtualKeyCode.LMENU; scancode1 = OSUtilities.DInputScanCodes.DIK_LMENU;
                    break;
                case Configuration.KeyCodes.RALT:
                    keycode = VirtualKeyCode.RMENU; scancode1 = OSUtilities.DInputScanCodes.DIK_RMENU;
                    break;
                case Configuration.KeyCodes.LEFT:
                    keycode = VirtualKeyCode.LEFT; scancode1 = OSUtilities.DInputScanCodes.DIK_LEFT;
                    break;
                case Configuration.KeyCodes.RIGHT:
                    keycode = VirtualKeyCode.RIGHT; scancode1 = OSUtilities.DInputScanCodes.DIK_RIGHT;
                    break;
                case Configuration.KeyCodes.UP:
                    keycode = VirtualKeyCode.UP; scancode1 = OSUtilities.DInputScanCodes.DIK_UP;
                    break;
                case Configuration.KeyCodes.DOWN:
                    keycode = VirtualKeyCode.DOWN; scancode1 = OSUtilities.DInputScanCodes.DIK_DOWN;
                    break;
                case Configuration.KeyCodes.F1:
                case Configuration.KeyCodes.F2:
                case Configuration.KeyCodes.F3:
                case Configuration.KeyCodes.F4:
                case Configuration.KeyCodes.F5:
                case Configuration.KeyCodes.F6:
                case Configuration.KeyCodes.F7:
                case Configuration.KeyCodes.F8:
                case Configuration.KeyCodes.F9:
                case Configuration.KeyCodes.F10:
                    keycode = (VirtualKeyCode)(VirtualKeyCode.F1 + (ushort)(key - Configuration.KeyCodes.F1));
                    scancode1 = (OSUtilities.DInputScanCodes)(OSUtilities.DInputScanCodes.DIK_F1 + (ushort)(key - Configuration.KeyCodes.F1));
                    break;
                case Configuration.KeyCodes.F11:
                    keycode = VirtualKeyCode.F11; scancode1 = OSUtilities.DInputScanCodes.DIK_F11;
                    break;
                case Configuration.KeyCodes.F12:
                    keycode = VirtualKeyCode.F12; scancode1 = OSUtilities.DInputScanCodes.DIK_F12;
                    break;
                case Configuration.KeyCodes.NUM0:
                    keycode = VirtualKeyCode.VK_0; scancode1 = OSUtilities.DInputScanCodes.DIK_0;
                    break;
                case Configuration.KeyCodes.NUM1:
                case Configuration.KeyCodes.NUM2:
                case Configuration.KeyCodes.NUM3:
                case Configuration.KeyCodes.NUM4:
                case Configuration.KeyCodes.NUM5:
                case Configuration.KeyCodes.NUM6:
                case Configuration.KeyCodes.NUM7:
                case Configuration.KeyCodes.NUM8:
                case Configuration.KeyCodes.NUM9:
                    keycode = (VirtualKeyCode)(VirtualKeyCode.VK_0 + (ushort)(key - Configuration.KeyCodes.F1));
                    scancode1 = (OSUtilities.DInputScanCodes)(OSUtilities.DInputScanCodes.DIK_F1 + (ushort)(key - Configuration.KeyCodes.F1));
                    break;
                case Configuration.KeyCodes.NUMPAD_0:
                    keycode = VirtualKeyCode.NUMPAD0; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD0;
                    break;
                case Configuration.KeyCodes.NUMPAD_1:
                    keycode = VirtualKeyCode.NUMPAD1; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD1;
                    break;
                case Configuration.KeyCodes.NUMPAD_2:
                    keycode = VirtualKeyCode.NUMPAD2; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD2;
                    break;
                case Configuration.KeyCodes.NUMPAD_3:
                    keycode = VirtualKeyCode.NUMPAD3; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD3;
                    break;
                case Configuration.KeyCodes.NUMPAD_4:
                    keycode = VirtualKeyCode.NUMPAD4; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD4;
                    break;
                case Configuration.KeyCodes.NUMPAD_5:
                    keycode = VirtualKeyCode.NUMPAD5; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD5;
                    break;
                case Configuration.KeyCodes.NUMPAD_6:
                    keycode = VirtualKeyCode.NUMPAD6; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD6;
                    break;
                case Configuration.KeyCodes.NUMPAD_7:
                    keycode = VirtualKeyCode.NUMPAD7; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD7;
                    break;
                case Configuration.KeyCodes.NUMPAD_8:
                    keycode = VirtualKeyCode.NUMPAD8; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD8;
                    break;
                case Configuration.KeyCodes.NUMPAD_9:
                    keycode = VirtualKeyCode.NUMPAD9; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPAD9;
                    break;
                case Configuration.KeyCodes.NUMPAD_DECIMAL:
                    keycode = VirtualKeyCode.SEPARATOR; scancode1 = OSUtilities.DInputScanCodes.DIK_NUMPADCOMMA;
                    break;
                case Configuration.KeyCodes.A:
                    keycode = VirtualKeyCode.VK_A; scancode1 = OSUtilities.DInputScanCodes.DIK_A;
                    break;
                case Configuration.KeyCodes.B:
                    keycode = VirtualKeyCode.VK_B; scancode1 = OSUtilities.DInputScanCodes.DIK_B;
                    break;
                case Configuration.KeyCodes.C:
                    keycode = VirtualKeyCode.VK_C; scancode1 = OSUtilities.DInputScanCodes.DIK_C;
                    break;
                case Configuration.KeyCodes.D:
                    keycode = VirtualKeyCode.VK_D; scancode1 = OSUtilities.DInputScanCodes.DIK_D;
                    break;
                case Configuration.KeyCodes.E:
                    keycode = VirtualKeyCode.VK_E; scancode1 = OSUtilities.DInputScanCodes.DIK_E;
                    break;
                case Configuration.KeyCodes.F:
                    keycode = VirtualKeyCode.VK_F; scancode1 = OSUtilities.DInputScanCodes.DIK_F;
                    break;
                case Configuration.KeyCodes.G:
                    keycode = VirtualKeyCode.VK_G; scancode1 = OSUtilities.DInputScanCodes.DIK_G;
                    break;
                case Configuration.KeyCodes.H:
                    keycode = VirtualKeyCode.VK_H; scancode1 = OSUtilities.DInputScanCodes.DIK_H;
                    break;
                case Configuration.KeyCodes.I:
                    keycode = VirtualKeyCode.VK_I; scancode1 = OSUtilities.DInputScanCodes.DIK_I;
                    break;
                case Configuration.KeyCodes.J:
                    keycode = VirtualKeyCode.VK_J; scancode1 = OSUtilities.DInputScanCodes.DIK_J;
                    break;
                case Configuration.KeyCodes.K:
                    keycode = VirtualKeyCode.VK_K; scancode1 = OSUtilities.DInputScanCodes.DIK_K;
                    break;
                case Configuration.KeyCodes.L:
                    keycode = VirtualKeyCode.VK_L; scancode1 = OSUtilities.DInputScanCodes.DIK_L;
                    break;
                case Configuration.KeyCodes.M:
                    keycode = VirtualKeyCode.VK_M; scancode1 = OSUtilities.DInputScanCodes.DIK_M;
                    break;
                case Configuration.KeyCodes.N:
                    keycode = VirtualKeyCode.VK_N; scancode1 = OSUtilities.DInputScanCodes.DIK_N;
                    break;
                case Configuration.KeyCodes.O:
                    keycode = VirtualKeyCode.VK_O; scancode1 = OSUtilities.DInputScanCodes.DIK_O;
                    break;
                case Configuration.KeyCodes.P:
                    keycode = VirtualKeyCode.VK_P; scancode1 = OSUtilities.DInputScanCodes.DIK_P;
                    break;
                case Configuration.KeyCodes.Q:
                    keycode = VirtualKeyCode.VK_Q; scancode1 = OSUtilities.DInputScanCodes.DIK_Q;
                    break;
                case Configuration.KeyCodes.R:
                    keycode = VirtualKeyCode.VK_R; scancode1 = OSUtilities.DInputScanCodes.DIK_R;
                    break;
                case Configuration.KeyCodes.S:
                    keycode = VirtualKeyCode.VK_S; scancode1 = OSUtilities.DInputScanCodes.DIK_S;
                    break;
                case Configuration.KeyCodes.T:
                    keycode = VirtualKeyCode.VK_T; scancode1 = OSUtilities.DInputScanCodes.DIK_T;
                    break;
                case Configuration.KeyCodes.U:
                    keycode = VirtualKeyCode.VK_U; scancode1 = OSUtilities.DInputScanCodes.DIK_U;
                    break;
                case Configuration.KeyCodes.V:
                    keycode = VirtualKeyCode.VK_V; scancode1 = OSUtilities.DInputScanCodes.DIK_V;
                    break;
                case Configuration.KeyCodes.W:
                    keycode = VirtualKeyCode.VK_W; scancode1 = OSUtilities.DInputScanCodes.DIK_W;
                    break;
                case Configuration.KeyCodes.X:
                    keycode = VirtualKeyCode.VK_X; scancode1 = OSUtilities.DInputScanCodes.DIK_X;
                    break;
                case Configuration.KeyCodes.Y:
                    keycode = VirtualKeyCode.VK_Y; scancode1 = OSUtilities.DInputScanCodes.DIK_Y;
                    break;
                case Configuration.KeyCodes.Z:
                    keycode = VirtualKeyCode.VK_Z; scancode1 = OSUtilities.DInputScanCodes.DIK_Z;
                    break;
                case Configuration.KeyCodes.SPACE:
                    keycode = VirtualKeyCode.SPACE; scancode1 = OSUtilities.DInputScanCodes.DIK_SPACE;
                    break;

                default:
                    break;
            }

            // Pressed?
            if (newstate && !oldstate) {
                Logger.Log("[KEY] " + key.ToString() + " pressed with " + keyAPI.ToString(), LogLevels.INFORMATIVE);
                if (keyAPI.HasFlag(KeyEmulationAPI.DInput)) {
                    if (scancode1 != 0)
                        OSUtilities.SendKeybDInputDown(scancode1);
                    if (scancode2 != 0)
                        OSUtilities.SendKeybDInputDown(scancode2);
                }
                if (keyAPI.HasFlag(KeyEmulationAPI.SendInput)) {
                    if (keycode != 0)
                        OSUtilities.SendKeyDown(keycode);
                }
            }
            // Released?
            if (!newstate && oldstate) {
                Logger.Log("[KEY] " + key.ToString() + " released with " + keyAPI.ToString(), LogLevels.INFORMATIVE);
                if (keyAPI.HasFlag(KeyEmulationAPI.DInput)) {
                    if (scancode1 != 0)
                        OSUtilities.SendKeybDInputUp(scancode1);
                    if (scancode2 != 0)
                        OSUtilities.SendKeybDInputUp(scancode2);
                }
                if (keyAPI.HasFlag(KeyEmulationAPI.SendInput)) {
                    if (keycode != 0)
                        OSUtilities.SendKeyUp(keycode);
                }
            }
        }


        /// <summary>
        /// Look for all values and take decision whether a keystroke
        /// has to be send or not
        /// </summary>
        /// <param name="rawstate"></param>
        /// <returns>true is state has changed</returns>
        public bool ProcessKeyStrokes(double[] rawaxis_pct, double[] axis_pct,
            UInt64 rawinputs, UInt64 buttons)
        {
            var stt = false;
            var cs = BFFManager.CurrentControlSet;
            // First ensure that KeyStrokes and config matches
            if (KeyStrokes.Count < cs.KeyStrokeDBs.Count) {
                // Add new
                int nb = cs.KeyStrokeDBs.Count - KeyStrokes.Count;
                for (int i = 0; i<nb; i++) {
                    KeyStrokes.Add(new KeyStroke());
                }
            }
            if (KeyStrokes.Count > cs.KeyStrokeDBs.Count) {
                // Remove old
                int nb = KeyStrokes.Count - cs.KeyStrokeDBs.Count;
                for (int i = 0; i<nb; i++) {
                    KeyStrokes.RemoveAt(KeyStrokes.Count-1);
                }
            }

            // Loop on all keystrokes
            for (int i = 0; i<KeyStrokes.Count; i++) {
                var keystroke = KeyStrokes[i];
                // Update index
                keystroke.KeyStrokeIndex = i;
                // Build condition expression on up to 3 sources
                bool allconditions = false;
                var db = keystroke.Config;
                for (int j = 0; j<db.KeySources.Count; j++) {
                    bool condition = false;
                    var source = db.KeySources[j];
                    int index = source.Index-1;
                    if (index<0) continue;
                    if (source.Type == KeySourceTypes.UNDEF) break;
                    switch (source.Type) {
                        case KeySourceTypes.RAW_AXIS:
                            // Detect threshold with AxisTolerance
                            if (rawaxis_pct[index]>(source.Threshold+db.AxisTolerance_pct)) {
                                // Ok, positive condition verified
                                source.PrevAxisCondition = true;
                            }
                            if (rawaxis_pct[index]<(source.Threshold-db.AxisTolerance_pct)) {
                                // fall down
                                source.PrevAxisCondition = false;
                            }
                            // Keep previous state or new if above tolerance
                            if (source.InvSign) {
                                // Inverse condition
                                condition = !source.PrevAxisCondition;
                            } else {
                                condition = source.PrevAxisCondition;
                            }
                            break;
                        case KeySourceTypes.VJOY_AXIS:
                            // Detect threshold with AxisTolerance
                            if (axis_pct[index]>(source.Threshold+db.AxisTolerance_pct)) {
                                // Ok, positive condition verified
                                source.PrevAxisCondition = true;
                            }
                            if (axis_pct[index]<(source.Threshold-db.AxisTolerance_pct)) {
                                // fall down
                                source.PrevAxisCondition = false;
                            }
                            // Keep previous state or new if above tolerance
                            if (source.InvSign) {
                                // Inverse condition
                                condition = !source.PrevAxisCondition;
                            } else {
                                condition = source.PrevAxisCondition;
                            }
                            break;
                        case KeySourceTypes.RAW_INPUT:
                            if (!source.InvSign && ((rawinputs & ((UInt64)1<<index))!=0)) {
                                // Ok, condition verified
                                condition = true;
                            } else if (source.InvSign && ((rawinputs & ((UInt64)1<<index))==0)) {
                                // Ok, condition verified
                                condition = true;
                            }
                            break;
                        case KeySourceTypes.VJOY_BUTTON:
                            if (!source.InvSign && ((buttons & ((UInt64)1<<index))!=0)) {
                                // Ok, condition verified
                                condition = true;
                            } else if (source.InvSign && ((buttons & ((UInt64)1<<index))==0)) {
                                // Ok, condition verified
                                condition = true;
                            }
                            break;
                        case KeySourceTypes.UNDEF:
                        default:
                            break;
                    }

                    // If first condition verified, then set global flag
                    if (j==0) {
                        allconditions = condition;
                    } else {
                        // Use operator 
                        switch (db.KeySourcesOperators[j-1]) {
                            case KeysOperators.AND:
                                allconditions = allconditions && condition;
                                break;
                            case KeysOperators.NAND:
                                allconditions = allconditions && !condition;
                                break;
                            case KeysOperators.OR:
                                allconditions = allconditions || condition;
                                break;
                            case KeysOperators.NOR:
                                allconditions = allconditions || !condition;
                                break;
                            case KeysOperators.NO:
                            default:
                                break;
                        }
                    }
                }
                // Check global condition holds
                var changed = keystroke.UpdateValue(allconditions);
                // Ok, keystroke should be handled
                if (changed) {
                    stt = true;
                    // Loop on all combined keys
                    for (int j = 0; j<db.CombinedKeyStrokes.Count; j++) {
                        if (db.CombinedKeyStrokes[j]!= KeyCodes.None) {
                            ProcessKeyStroke(db.CombinedKeyStrokes[j], db.KeyAPI, keystroke.State, keystroke.PrevState);
                        }
                    }
                }
            }
            return stt;
        }

    }
}
