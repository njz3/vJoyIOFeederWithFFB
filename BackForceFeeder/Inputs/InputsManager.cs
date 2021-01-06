using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackForceFeeder.Utils;

namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// Inputs processing : either analog or digital inputs.
    /// Also handles trigger events, shift decodees and keystrokes
    /// </summary>
    public class InputsManager
    {
        public const int MAXRAWINPUTS = 32;
        public const int MAXRAWAXIS = 4;

        /// <summary>
        /// Raw inputs (up to MAXRAWINPUTS), used to store states, config and fire events
        /// </summary>
        public List<RawInput> RawInputs;
        public RawInput SafeGetRawInput(int rawinput)
        {
            if (rawinput<0 || rawinput>=this.RawAxes.Count)
                return null;
            return this.RawInputs[rawinput];
        }

        /// <summary>
        /// Analog axes inputs (up to MAXANALOGINPUTS)
        /// </summary>
        public List<RawAxis> RawAxes;
        public RawAxis SafeGetRawAxis(int axis)
        {
            if (axis<0 || axis>=this.RawAxes.Count)
                return null;
            return this.RawAxes[axis];
        }

        /// <summary>
        /// Last updated raw digital inputs
        /// </summary>
        protected UInt64 _LastUpdatedRawInputsValues;
        /// <summary>
        /// Combined value for all raw digital inputs from IO board
        /// </summary>
        public UInt64 RawInputsValues { get; protected set; }
        /// <summary>
        /// Combined value for all raw digital states (includes delay and
        /// inverted logic)
        /// </summary>
        public UInt64 RawInputsStates { get; protected set; }
        /// <summary>
        /// Previous combined value for all digital states
        /// </summary>
        public UInt64 PrevRawInputsStates { get; protected set; }
        /// <summary>
        /// Combined value for all buttons
        /// </summary>
        public UInt64 ButtonsValues { get; protected set; }

        public InputsManager()
        {
            RawInputs = new List<RawInput>(MAXRAWINPUTS);
            RawAxes = new List<RawAxis>(MAXRAWAXIS);
        }

        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        { Logger.Log("[INPUTS] " + text, level); }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        { Logger.LogFormat(level, "[INPUTS] " + text, args); }

        public void Initialize(int analog, int digital)
        {
            if (analog>MAXRAWAXIS) {
                analog = MAXRAWAXIS;
            }
            if (digital>MAXRAWINPUTS) {
                digital = MAXRAWINPUTS;
            }
            RawAxes.Clear();
            RawInputs.Clear();
            for (int i = 0; i<analog; i++) {
                var axis = new RawAxis();
                axis.RawAxisIndex = i;
                RawAxes.Add(axis);
            }
            for (int i = 0; i<digital; i++) {
                var rawinput = new RawInput();
                rawinput.RawInputIndex = i;
                RawInputs.Add(rawinput);
            }
            ClearAll();
        }

        /// <summary>
        /// In case control set is changed, clear all mapped buttons and
        /// shifters states
        /// </summary>
        public void ClearAll()
        {
            this._HShifter.Clear();
            this._HShifterCurrentGear = 0;
            this._UpDnShifter.Clear();
            this._UpDnShifterCurrentGear = 0;
            var vJoy = SharedData.Manager.vJoy;
            if (vJoy!=null) {
                vJoy.UpodateAllButtons(0, 0);
            }
            this.ButtonsValues = 0;
            this._LastUpdatedRawInputsValues = (ulong)0xcafebabe;
        }


        /// <summary>
        /// Update all analog values from given values in counts.
        /// Will fire a trigger event if needed.
        /// </summary>
        /// <param name="axis"></param>
        public bool Update1Axes(int index, Int64 axis)
        {
            bool stt = false;
            if (axis!=RawAxes[index].RawValue_cts) {
                stt = true;
                // Set new value
                RawAxes[index].UpdateValue(axis);
                // Detect event?
            } else {
                // Do nothing
            }
            return stt;
        }


        /// <summary>
        /// Update all analog values from given values in counts.
        /// Will fire a trigger event if needed.
        /// </summary>
        /// <param name="axes"></param>
        public bool UpdateAllRawAxes(Int64[] axes)
        {
            bool stt = false;
            for (int i = 0; i<RawAxes.Count; i++) {
                RawAxes[i].RawAxisIndex = i;
                if (axes[i]!=RawAxes[i].RawValue_cts) {
                    stt = true;
                    // Set new value
                    RawAxes[i].UpdateValue(axes[i]);
                    // Detect event?
                } else {
                    // Do nothing
                }
            }
            return stt;
        }

        public void GetRawAxes(ref double[] axis_pct)
        {
            for (int i = 0; i<RawAxes.Count; i++) {
                axis_pct[i] = RawAxes[i].RawValue_pct;
            }
        }
        public void GetCorrectedAxes(ref double[] corrected_pct)
        {
            for (int i = 0; i<RawAxes.Count; i++) {
                corrected_pct[i] = RawAxes[i].Corrected_pct;
            }
        }



        /// <summary>
        /// Update all raw values from given state register.
        /// Will fire a trigger event.
        /// </summary>
        /// <param name="allbits"></param>
        public bool UpdateAllDigitalInputs(UInt64 newvalues)
        {
            bool stt = false;
            if (newvalues!=_LastUpdatedRawInputsValues) {
                _LastUpdatedRawInputsValues = newvalues;
                // There might be changes.
                // Prepare a new state just in case
                UInt64 newRawInputsStates = RawInputsStates;
                // Now loop over all inputs to see whether a state change happened
                for (int i = 0; i<RawInputs.Count; i++) {
                    bool newvalue = (newvalues & (ulong)(1<<i)) != 0;
                    // Check state update
                    if (RawInputs[i].UpdateValue(newvalue)) {
                        stt = true;
                        // Update buttons map given new input state
                        if (RawInputs[i].State) {
                            // Set
                            newRawInputsStates |= (ulong)1<<i;
                        } else {
                            // Clear
                            newRawInputsStates &= ~(ulong)1<<i;
                        }
                        // Perform deep input analysis and update vjoy
                        PerformDeepInputAnalysis(RawInputs[i], RawInputs[i].State, RawInputs[i].PrevState);
                    }
                }
                if (stt) {
                    // Rebuilt new inputstates
                    RawInputsValues = newvalues;
                    PrevRawInputsStates = RawInputsStates;
                    RawInputsStates = newRawInputsStates;
                    UInt64 buttons0_63 = 0;
                    UInt64 buttons64_127 = 0;
                    var vJoy = SharedData.Manager.vJoy;
                    if (vJoy!=null)
                        vJoy.GetButtonsStates(ref buttons0_63, ref buttons64_127);
                    this.ButtonsValues = buttons0_63;
                }
            }

            ProcessShifters();

            return stt;
        }

        // Internal values for special operation
        protected UInt64 autofire_mode_on = 0;

        // HShifter decoder map
        RawInputDB[] _HShifterDecoderMap = new RawInputDB[3];
        bool[] _HShifterPressedMap = new bool[3];
        protected HShifterDecoder _HShifter = new HShifterDecoder();
        protected int _HShifterCurrentGear = 0;

        // Up/Down shifter decoder map
        RawInputDB[] _UpDownShifterDecoderMap = new RawInputDB[2];
        bool[] _UpDownShifterPressedMap = new bool[2];
        protected UpDnShifterDecoder _UpDnShifter = new UpDnShifterDecoder();
        protected int _UpDnShifterCurrentGear = 0;

        bool _UpDnIsGearEngaged = false;
        ulong _UpDnlastTimePressed_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;

        protected void ProcessShifters()
        {
            var cs = BFFManager.CurrentControlSet;
            var vJoy = SharedData.Manager.vJoy;

            #region Refresh Shifter decoder map
            // Every 100 tick periods, rescan shifter map 
            if (SharedData.Manager.TickCount%100 == 0) {
                // Loop over all inputs to see we have a new shifter decoder map
                for (int i = 0; i<RawInputs.Count; i++) {
                    var rawdb = RawInputs[i].Config;
                    if (rawdb.ShifterDecoder!= ShifterDecoderMap.No) {
                        // Part of HShifter decoder map, just save the values
                        switch (rawdb.ShifterDecoder) {
                            case ShifterDecoderMap.HShifterLeftRight:
                            case ShifterDecoderMap.HShifterUp:
                            case ShifterDecoderMap.HShifterDown:
                                // rawdb
                                _HShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = rawdb;
                                break;
                            case ShifterDecoderMap.SequencialUp:
                            case ShifterDecoderMap.SequencialDown:
                                // rawdb
                                _UpDownShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = rawdb;
                                break;
                        }
                    }
                }
            }
            #endregion

            #region Decode HShifter map (if used)
            if (_HShifterDecoderMap[0]!=null && _HShifterDecoderMap[1]!=null && _HShifterDecoderMap[2]!=null) {
                // First left/right switch pressed?
                _HShifter.HSHifterLeftRightPressed = _HShifterPressedMap[0];
                // Up pressed?
                _HShifter.UpPressed = _HShifterPressedMap[1];
                // Down pressed?
                _HShifter.DownPressed = _HShifterPressedMap[2];
                // Now get decoded value
                int selectedshift = _HShifter.CurrentShift; //0=neutral

                // Detect change
                if (selectedshift!=_HShifterCurrentGear) {
                    Log("HShifter decoder from=" + _HShifterCurrentGear + " to " + selectedshift, LogLevels.DEBUG);
                    _HShifterCurrentGear = selectedshift;
                    var shifterrawdb = _HShifterDecoderMap[0];
                    if (shifterrawdb.MappedvJoyBtns.Count>0) {
                        // Clear previous buttons first
                        if (shifterrawdb.SequenceCurrentToSet>=0 && shifterrawdb.SequenceCurrentToSet<shifterrawdb.MappedvJoyBtns.Count) {
                            if (vJoy!=null)
                                vJoy.Clear1Button(shifterrawdb.MappedvJoyBtns[shifterrawdb.SequenceCurrentToSet]);
                        }
                        // Set indexer to new shift value
                        if (shifterrawdb.IsNeutralFirstBtn) {
                            // Neutral is first button
                            shifterrawdb.SequenceCurrentToSet = _HShifterCurrentGear;
                        } else {
                            // Neutral is not a button
                            shifterrawdb.SequenceCurrentToSet = _HShifterCurrentGear-1;
                        }
                        // Check min/max
                        if (shifterrawdb.SequenceCurrentToSet>=shifterrawdb.MappedvJoyBtns.Count) {
                            shifterrawdb.SequenceCurrentToSet = shifterrawdb.MappedvJoyBtns.Count-1;
                        }
                        if (shifterrawdb.SequenceCurrentToSet>=0) {
                            // Set only indexed one
                            if (vJoy!=null)
                                vJoy.Set1Button(shifterrawdb.MappedvJoyBtns[shifterrawdb.SequenceCurrentToSet]);
                        }
                    }
                }
            }
            #endregion

            #region Decode Up/Down shifter map
            if (_UpDownShifterDecoderMap[0]!=null && _UpDownShifterDecoderMap[1]!=null) {
                //UpDnShifter.MaxShift = UpDownShifterDecoderMap[0].SequenceCurrentToSet;
                //UpDnShifter.MinShift = UpDownShifterDecoderMap[1].SequenceCurrentToSet;
                _UpDnShifter.ValidateDelay_ms = (ulong)cs.vJoyButtonsDB.UpDownNeutralDelay_ms;
                // Up pressed?
                _UpDnShifter.UpPressed = _UpDownShifterPressedMap[0];
                // Down pressed?
                _UpDnShifter.DownPressed = _UpDownShifterPressedMap[1];
                // Now get decoded value
                int selectedshift = _UpDnShifter.CurrentShift; //0=neutral

                var shifterrawdb = _UpDownShifterDecoderMap[0];

                // Detect change and press mapped button
                if (selectedshift!=_UpDnShifterCurrentGear) {
                    Log("UpDnShifter decoder from=" + _UpDnShifterCurrentGear + " to " + selectedshift, LogLevels.DEBUG);
                    _UpDnShifterCurrentGear = selectedshift;
                    _UpDnlastTimePressed_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;

                    if (shifterrawdb.MappedvJoyBtns.Count>0) {
                        // Clear all buttons first
                        if (shifterrawdb.SequenceCurrentToSet>=0 && shifterrawdb.SequenceCurrentToSet<shifterrawdb.MappedvJoyBtns.Count) {
                            if (vJoy!=null)
                                vJoy.Clear1Button(shifterrawdb.MappedvJoyBtns[shifterrawdb.SequenceCurrentToSet]);
                        }
                        // Update max shift, just in case
                        if (shifterrawdb.IsNeutralFirstBtn)
                            _UpDnShifter.MaxShift = shifterrawdb.MappedvJoyBtns.Count-1;
                        else
                            _UpDnShifter.MaxShift = shifterrawdb.MappedvJoyBtns.Count;
                        // Set indexer to new shift value
                        if (shifterrawdb.IsNeutralFirstBtn) {
                            // Neutral is first button
                            shifterrawdb.SequenceCurrentToSet = _UpDnShifterCurrentGear;
                        } else {
                            // Neutral is not a button
                            shifterrawdb.SequenceCurrentToSet = _UpDnShifterCurrentGear-1;
                        }
                        // Check min/max
                        if (shifterrawdb.SequenceCurrentToSet>=shifterrawdb.MappedvJoyBtns.Count) {
                            shifterrawdb.SequenceCurrentToSet = shifterrawdb.MappedvJoyBtns.Count-1;
                        }
                        if (shifterrawdb.SequenceCurrentToSet>=0) {
                            // Set only indexed one
                            if (vJoy!=null) {
                                vJoy.Set1Button(shifterrawdb.MappedvJoyBtns[shifterrawdb.SequenceCurrentToSet]);
                                _UpDnIsGearEngaged = true;
                            }
                        }
                    }
                } else {
                    // Check for timeout to release button
                    if (_UpDnIsGearEngaged) {
                        if (cs.vJoyButtonsDB.UpDownMaintain_ms>0) {
                            ulong delay = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds - _UpDnlastTimePressed_ms;
                            if (delay>(ulong)cs.vJoyButtonsDB.UpDownMaintain_ms) {
                                if (vJoy!=null) {
                                    if (shifterrawdb.SequenceCurrentToSet>=0) {
                                        vJoy.Clear1Button(shifterrawdb.MappedvJoyBtns[shifterrawdb.SequenceCurrentToSet]);
                                    }
                                }
                                _UpDnIsGearEngaged = false;
                            }
                        }
                    }
                }
            }
            #endregion

        }
        public void PerformDeepInputAnalysis(RawInput input, bool newrawval, bool prevrawval)
        {
            var cs = BFFManager.CurrentControlSet;
            var vJoy = SharedData.Manager.vJoy;
            // Bit corresponding to this input
            var rawbit = (UInt32)(1<<input.RawInputIndex);
            var rawdb = input.Config;
            // Check if we toggle the bit (or autofire mode)
            if (rawdb.IsToggle) {
                // Toggle only if we detect a false->true transition in raw value
                if (newrawval && (!prevrawval)) {
                    // Toggle = xor on every vJoy buttons
                    if (vJoy!=null)
                        vJoy.ToggleButtons(rawdb.MappedvJoyBtns);
                }
            } else if (rawdb.IsAutoFire) {
                // Autofire set, if false->true transition, then toggle autofire state
                if (newrawval && (!prevrawval)) {
                    // Enable/disable autofire
                    autofire_mode_on ^= rawbit;
                }
                // No perform autofire toggle if autofire enabled
                if ((autofire_mode_on&rawbit)!=0) {
                    // Toggle = xor every n periods
                    ulong n = (ulong)(cs.vJoyButtonsDB.AutoFirePeriod_ms/BFFManager.GlobalRefreshPeriod_ms);
                    if ((SharedData.Manager.TickCount%n)==0) {
                        if (vJoy!=null)
                            vJoy.ToggleButtons(rawdb.MappedvJoyBtns);
                    }
                }
            } else if (rawdb.IsSequencedvJoy) {
                // Sequenced vJoy buttons - every rising edge, will trigger a new vJoy
                // if false->true transition, then toggle vJoy and move index
                if (newrawval && (!prevrawval)) {
                    // Clear previous button first
                    if (vJoy!=null)
                        vJoy.Clear1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                    // Move indexer
                    rawdb.SequenceCurrentToSet++;
                    if (rawdb.SequenceCurrentToSet>=rawdb.MappedvJoyBtns.Count) {
                        rawdb.SequenceCurrentToSet = 0;
                    }
                    if (rawdb.MappedvJoyBtns.Count<1)
                        return;
                    // Set only indexed one
                    if (vJoy!=null)
                        vJoy.Set1Button(rawdb.MappedvJoyBtns[rawdb.SequenceCurrentToSet]);
                }
            } else if (rawdb.ShifterDecoder!= ShifterDecoderMap.No) {
                // Part of HShifter decoder map, just save the values
                switch (rawdb.ShifterDecoder) {
                    case ShifterDecoderMap.HShifterLeftRight:
                    case ShifterDecoderMap.HShifterUp:
                    case ShifterDecoderMap.HShifterDown:
                        // rawdb
                        _HShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = rawdb;
                        // state of raw input
                        _HShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = newrawval;
                        break;
                    case ShifterDecoderMap.SequencialUp:
                    case ShifterDecoderMap.SequencialDown:
                        // rawdb
                        _UpDownShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = rawdb;
                        // state of raw input
                        _UpDownShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = newrawval;
                        break;
                }
            } else {
                // Nothing specific : perform simple mask set or clear button
                if (newrawval) {
                    if (vJoy!=null)
                        vJoy.SetButtons(rawdb.MappedvJoyBtns);
                } else {
                    if (vJoy!=null)
                        vJoy.ClearButtons(rawdb.MappedvJoyBtns);
                }
            }

        }


        public void Enforce(int idx, bool newvalue)
        {
            if (idx<0 || idx>=this.RawInputs.Count)
                return;
            if (newvalue)
                RawInputsValues |= ((UInt64)1<<idx);
            else
                RawInputsValues &= ~((UInt64)1<<idx);

            bool stt = false;
            // Check state update
            if (RawInputs[idx].UpdateValue(newvalue)) {
                UInt64 newRawInputsStates = RawInputsStates;
                // Perform deep input analysis and update vjoy
                PerformDeepInputAnalysis(RawInputs[idx], RawInputs[idx].State, RawInputs[idx].PrevState);

                // Rebuilt new inputstates
                PrevRawInputsStates = RawInputsStates;
                RawInputsStates = newRawInputsStates;
                UInt64 buttons0_63 = 0;
                UInt64 buttons64_127 = 0;
                var vJoy = SharedData.Manager.vJoy;
                if (vJoy!=null)
                    vJoy.GetButtonsStates(ref buttons0_63, ref buttons64_127);
                this.ButtonsValues = buttons0_63;
            }
        }

    }
}
