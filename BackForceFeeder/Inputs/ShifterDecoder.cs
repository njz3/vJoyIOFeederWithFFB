namespace BackForceFeeder.Inputs
{
    /// <summary>
    /// Interface for shifter decoder
    /// </summary>
    public interface IShifterDecoder
    {
        int CurrentShift { get; }
    }

    /// <summary>
    /// H pattern Sega shifter decoder.
    /// 3 digital inputs -> 5 values: neutral and gears 1-4.
    /// 1 input for left/right side
    /// 1 input for up
    /// 1 input for down
    /// </summary>
    public class HShifterDecoder : IShifterDecoder
    {
        protected int _LastDecoded = 0; //0=neutral
        protected bool _LeftRightPressed = false;
        protected bool _UpPressed = false;
        protected bool _DownPressed = false;
        
        /// <summary>
        /// Decode shift value from Left/Right, Up, Down values
        /// </summary>
        public int CurrentShift { get { return _LastDecoded; } }

        /// <summary>
        /// Clear shifter state
        /// </summary>
        public void Clear()
        {
            _LastDecoded = 0;
            _LeftRightPressed = false;
            _UpPressed = false;
            _DownPressed = false;
        }

        /// <summary>
        /// Input for Left/Right side
        /// </summary>
        public bool HSHifterLeftRightPressed {
            get { return _LeftRightPressed; }
            set {
                if (_LeftRightPressed!=value) {
                    _LeftRightPressed = value;
                    DecodeValue();
                }
            }
        }
        /// <summary>
        /// Input for Up
        /// </summary>
        public bool UpPressed {
            get { return _UpPressed; }
            set {
                if (_UpPressed!=value) {
                    _UpPressed = value;
                    DecodeValue();
                }
            }
        }
        /// <summary>
        /// Input for Down
        /// </summary>
        public bool DownPressed {
            get { return _DownPressed; }
            set {
                if (_DownPressed!=value) {
                    _DownPressed = value;
                    DecodeValue();
                }
            }
        }


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

    /// <summary>
    /// Emulation of H shifter using a Up/Down shifter or +/- wheel padle shift
    /// 2 digital inputs -> 5 values: neutral and gears 1-4
    /// 1 input for up/+
    /// 1 input for down/-
    /// The emulated shifter goes from neutral up to gear 4.
    /// A small time delay is added to switch to neutral before switching to the new shift.
    /// </summary>
    public class UpDnShifterDecoder : IShifterDecoder
    {
        protected int _LastDecoded = 0; //0=neutral
        protected bool _UpPressed = false;
        protected bool _DnPressed = false;
        protected int _InternalShift = 0;

        public int CurrentShift {
            get {
                DecodeValue();
                return _LastDecoded;
            }
        }

        /// <summary>
        /// Clear shifter state
        /// </summary>
        public void Clear()
        {
            _InternalShift = 0;
            _LastDecoded = 0;
            _UpPressed = false;
            _DnPressed = false;
            _lastTimeChange_ms = (ulong)Utils.MultimediaTimer.RefTimer.ElapsedMilliseconds;
        }

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

        /// <summary>
        /// Minimal shift in the sequence, 0=neutral
        /// </summary>
        public int MinShift = 0;
        /// <summary>
        /// Maximum shift in the sequence, 1..4
        /// </summary>
        public int MaxShift = 4;

        /// <summary>
        /// Delay used to switch to the new shift (will be neutral during this time)
        /// </summary>
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

