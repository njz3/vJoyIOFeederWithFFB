using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Combined value for all raw digital inputs
        /// </summary>
        public UInt64 RawInputsValues { get; protected set; }
        /// <summary>
        /// Combined value for all raw digital states
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
        /// Update a single raw input
        /// Will fire a trigger event.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        public bool UpdateSingleDigitalInput(int idx, bool newvalue)
        {
            bool stt = false;
            if (RawInputs[idx].UpdateValue(newvalue))
                stt = true;
            return stt;
        }


        /// <summary>
        /// Update all raw values from given state register.
        /// Will fire a trigger event.
        /// </summary>
        /// <param name="allbits"></param>
        public bool UpdateAllDigitalInputs(UInt64 newvalues)
        {
            bool stt = false;
            if (newvalues!=RawInputsValues) {
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
                    SharedData.Manager.vJoy.GetButtonsStates(ref buttons0_63, ref buttons64_127);
                    this.ButtonsValues = buttons0_63;
                }
            }
            return stt;
        }

        // Internal values for special operation
        protected UInt64 autofire_mode_on = 0;

        protected HShifterDecoder _HShifter = new HShifterDecoder();
        protected int _HShifterCurrentGear = 0;

        protected UpDnShifterDecoder _UpDnShifter = new UpDnShifterDecoder();
        protected int _UpDownShifterCurrentGear = 0;

        // HShifter decoder map
        RawInputDB[] HShifterDecoderMap = new RawInputDB[3];
        bool[] HShifterPressedMap = new bool[3];
        // Up/Down shifter decoder map
        RawInputDB[] UpDownShifterDecoderMap = new RawInputDB[2];
        bool[] UpDownShifterPressedMap = new bool[2];

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
                        HShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = rawdb;
                        // state of raw input
                        HShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.HShifterLeftRight] = newrawval;
                        break;
                    case ShifterDecoderMap.SequencialUp:
                    case ShifterDecoderMap.SequencialDown:
                        // rawdb
                        UpDownShifterDecoderMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = rawdb;
                        // state of raw input
                        UpDownShifterPressedMap[(int)rawdb.ShifterDecoder-(int)ShifterDecoderMap.SequencialUp] = newrawval;
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

        public void GetRawInputsValue(ref UInt64 rawinputs)
        {
            rawinputs = RawInputsValues;
            return;
            /*
            rawinputs = 0;
            for (int i = 0; i<RawInputs.Count; i++) {
                if (RawInputs[i].RawValue)
                    rawinputs |= ((UInt64)1<<RawInputs[i].RawInputIndex);
            }*/
        }
        public void GetRawInputsStates(ref UInt64 rawstates)
        {
            rawstates = RawInputsStates;
            return;
            /*
            rawinputs = 0;
            for (int i = 0; i<RawInputs.Count; i++) {
                if (RawInputs[i].Value)
                    rawinputs |= ((UInt64)1<<RawInputs[i].RawInputIndex);
            }*/
        }
        public void GetButtons(ref UInt64 buttons)
        {
            buttons = ButtonsValues;
            return;
            /*
            buttons = 0;
            for (int i = 0; i<RawInputs.Count; i++) {
                if (RawInputs[i].State)
                    buttons |= ((UInt64)1<<RawInputs[i].RawInputIndex);
            }*/
        }

    }
}
