using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.Managers;
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
        public KeyStrokesManager()
        {

        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[KEY] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[KEY] " + text, args);
        }

        public static void ProcessKeyStroke(KeyCodes key, KeyEmulationAPI keyAPI, bool newval, bool oldval)
        {
            // Leave early if no change
            if (newval == oldval)
                return;
            VirtualKeyCode keycode = 0;
            OSUtilities.DInputScanCodes scancode1 = 0;
            OSUtilities.DInputScanCodes scancode2 = 0;
            // Translation table
            switch (key) {
                case Configuration.KeyCodes.AltF4:
                    // Special keycode for combined press
                    if (keyAPI.HasFlag(KeyEmulationAPI.SendInput)) {
                        if (newval && (!oldval)) {
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

                default:
                    break;
            }

            // Pressed?
            if (newval && !oldval) {
                Logger.Log("Key " + key.ToString() + " pressed with " + keyAPI.ToString(), LogLevels.INFORMATIVE);
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
            if (!newval && oldval) {
                Logger.Log("Key " + key.ToString() + " released with " + keyAPI.ToString(), LogLevels.INFORMATIVE);
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
        public static void ProcessKeyStroke(RawInputDB rawdb, bool newval, bool oldval)
        {
            ProcessKeyStroke(rawdb.KeyStroke, rawdb.KeyAPI, newval, oldval);
        }


        /// <summary>
        /// Look for all global values and take decision whether a keystroke has to be send
        /// </summary>
        /// <param name="rawstate"></param>
        /// <returns>true is state has changed</returns>
        public void ProcessKeyStrokes()
        {
            var cs = BFFManager.Config.CurrentControlSet;
            var rawinput = Program.Manager.RawInputsStates;
            double[] rawanalog;
            int vjoybuttons = 0;
            double[] vjoyaxis;
        }

    }
}
