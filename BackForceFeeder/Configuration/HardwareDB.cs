using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackForceFeeder.Utils;

namespace BackForceFeeder.Configuration
{
    /// <summary>
    /// Translating mode for force feedback commands
    /// </summary>
    public enum FFBTranslatingModes : int
    {
        /// <summary>
        /// PWM + Dir (Fwd/Rev)
        /// </summary>
        PWM_DIR = 0,
        /// <summary>
        /// Centered PWM signal (50%=0 force)
        /// </summary>
        PWM_CENTERED,

        // COMPATIBILITY MODE

        /// <summary>
        /// Indy Model 2/Touring car/Le mans drive board
        /// </summary>
        COMP_M2_INDY_STC,
        /// <summary>
        /// Le mans drive board
        /// </summary>
        COMP_M3_LEMANS,
        /// <summary>
        /// Scud Race Model 3 drive board
        /// </summary>
        COMP_M3_SCUD,
        /// <summary>
        /// Scud Race Model 3 drive board
        /// </summary>
        COMP_M3_SR2,
        /// <summary>
        /// Model 3 generic drive board (unknown EEPROM)
        /// Use parallel port communication (8bits TX, 8bits RX)
        /// All Effects emulated using constant torque effect
        /// with codes 0x50 and 0x60.
        /// </summary>
        COMP_M3_UNKNOWN = 100,
        /// <summary>
        /// RAW M2pac mode : raw sending of drive board command
        /// WARNING: on non compatible board it will not work!
        /// Compatible games:
        /// - Indy Model 2/Touring car/Le mans
        /// - Scud Race/Daytona2/Emergency Call Ambulance/Dirt Devil
        /// </summary>
        RAW_M2PAC_MODE,
        /// <summary>
        /// Lindbergh RS422 drive board through RS232
        /// </summary>
        //LINDBERGH_GENERIC_DRVBD = 300,

    }


    [Serializable]
    public class HardwareDB
    {
        public FFBTranslatingModes TranslatingModes = FFBTranslatingModes.PWM_DIR;

        public UInt32 SerialPortSpeed = 1000000;
        // Major/Minor
        public UInt32 ProtocolVersionMajor = 0x0001;
        public UInt32 ProtocolVersionMinor = 0x0000;
        public bool EnforceHandshakingVersionChecks = false;
        public bool UseStreamingMode = true;

        public double TimeoutForInit_ms = 30000;

        /// <summary>
        /// False (+1.0) if turning wheel left increments position value (= positive speed)
        /// True (-1.0) if turning wheel left decrements position value (= negative speed)
        /// </summary>
        public bool InvertWheelDirection = false;
        /// <summary>
        /// False (+1.0) if positive torque command turn wheel left
        /// True (-1.0) if positive torque command turn wheel right
        /// </summary>
        public bool InvertTrqDirection = false;

        /// <summary>
        /// True if dual mode PWM is to be used (2 PWM outputs)
        /// False if single mode PWM (1 PWM output)
        /// </summary>
        public bool DualModePWM = false;
        /// <summary>
        /// True if digital PWM is to be used (send torque on serial port)
        /// </summary>
        public bool DigitalPWM = false;
        /// <summary>
        /// True when using alternative pin mapping for FFB controller shield
        /// </summary>
        public bool AlternativePinFFBController = false;

        public double WheelCenterOffset_u = -1.0;
        public double WheelScaleFactor_u_per_cts = -2.0/(0xFFF);
        /// <summary>
        /// Global motor Strengh
        /// </summary>
        public double MotorStrengh = 1.0;

    }
}
