using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyIOFeeder.Utils;
using static vJoyIOFeeder.vJoyManager;

namespace vJoyIOFeeder.Configuration
{

    [Serializable]
    public class HardwareDB
    {
        public FFBTranslatingModes TranslatingModes = FFBTranslatingModes.PWM_DIR;

        public UInt32 SerialPortSpeed = 1000000;
        // Major/Minor
        public UInt32 ProtocolVersionMajor = 0x0001;
        public UInt32 ProtocolVersionMinor = 0x0000;
        public bool EnforceHandshakingVersionChecks = false;


        public bool RunWithoutIOBoard = true;
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

        public double WheelCenterOffset_u = -1.0;
        public double WheelScaleFactor_u_per_cts = -2.0/(0xFFF);
    }
}
