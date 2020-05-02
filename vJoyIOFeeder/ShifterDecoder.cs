using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder
{
    public class ShifterDecoder
    {


    }

    public class HShifterDecoder : ShifterDecoder
    {
        protected bool _LeftRightPressed = false;
        protected bool _UpPressed = false;
        protected bool _DownPressed = false;
        public bool HSHifterLeftRightPressed {
            get { return _LeftRightPressed; }
            set {
                if (_LeftRightPressed!=value) {
                    _LeftRightPressed = value;
                    DecodeValue();
                }
            }
        }
        public bool UpPressed {
            get { return _UpPressed; }
            set {
                if (_UpPressed!=value) {
                    _UpPressed = value;
                    DecodeValue();
                }
            }
        }
        public bool DownPressed {
            get { return _DownPressed; }
            set {
                if (_DownPressed!=value) {
                    _DownPressed = value;
                    DecodeValue();
                }
            }
        }

        protected int _LastDecoded = 0; //0=neutral
        public int CurrentShift { get { return _LastDecoded; } }

        protected void DecodeValue()
        {
            _LastDecoded = DecodeValue(_LeftRightPressed, _UpPressed, _DownPressed);
        }
        protected int DecodeValue(bool leftRight, bool up, bool down)
        {
            int selectedshift = 0; //0=neutral
                                   // First switch pressed?
            if (leftRight) {
                // Left up or down?
                if (up) {
                    // Left Up = 1
                    selectedshift = 1;
                } else if (down) {
                    // Left Down = 2
                    selectedshift = 2;
                } else {
                    // Neutral
                }
            } else {
                // Right up or down?
                if (up) {
                    // Right Up = 3
                    selectedshift = 3;
                } else if (down) {
                    // Right Down = 4
                    selectedshift = 4;
                } else {
                    // Neutral
                }
            }
            return selectedshift;
        }
    }

    public class UpDnShifterDecoder : ShifterDecoder
    {
        protected int _LastDecoded = 0; //0=neutral
        public int CurrentShift {
            get {
                DecodeValue();
                return _LastDecoded;
            }
        }

        protected bool _UpPressed;
        public bool UpPressed {
            get { return _UpPressed; }
            set {
                // Detect transition
                if (_UpPressed!=value) {
                    _UpPressed = value;
                    // if positive edge, increase shift
                    if (_UpPressed) {
                        Increase();
                    } else {
                        // Update last refresh timer on release if we stayed in neutral before
                        if (StayInNeutralWhilePressed) {
                            _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                        }
                    }
                }
            }
        }
        public bool _DnPressed;
        public bool DownPressed {
            get { return _DnPressed; }
            set {
                // Detect transition
                if (_DnPressed!=value) {
                    _DnPressed = value;
                    // if positive edge, increase shift
                    if (_DnPressed) {
                        Decrease();
                    } else {
                        // Update last refresh timer on release if we stayed in neutral before
                        if (StayInNeutralWhilePressed) {
                            _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Option: we can stay in neutral while paddle is pressed
        /// </summary>
        public bool StayInNeutralWhilePressed = false;

        protected int _InternalShift = 0;
        /// <summary>
        /// Minimal shift in the sequence, 0=neutral
        /// </summary>
        public int MinShift = 0;
        /// <summary>
        /// Maximum shift in the sequence, 1..4
        /// </summary>
        public int MaxShift = 4;

        public ulong ValidateDelay_ms = 300;
        protected ulong _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        protected ulong _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;

        protected void Increase()
        {
            _InternalShift++;
            if (_InternalShift>MaxShift)
                _InternalShift = MaxShift;
            // Update last refresh timer
            _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
            DecodeValue();
        }

        protected void Decrease()
        {
            _InternalShift--;
            if (_InternalShift<MinShift)
                _InternalShift = MinShift;
            // Update last refresh timer
            _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
            DecodeValue();
        }
        protected void DecodeValue()
        {
            // If option to stay in neutral while still pressed
            if (StayInNeutralWhilePressed && (_UpPressed || DownPressed)) {
                // stay in minimal (neutral) shift until released
                _LastDecoded = MinShift;
            } else {
                // Update last refresh timer
                _lastTimeRefresh_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
                var elapsed_ms = _lastTimeRefresh_ms -_lastTimeChange_ms;
                if (elapsed_ms<ValidateDelay_ms) {
                    // stay in minimal (neutral) shift until timeout
                    _LastDecoded = MinShift;
                } else {
                    // timeout, validate current shift
                    _LastDecoded = _InternalShift;
                }
            }
        }
    }
}

