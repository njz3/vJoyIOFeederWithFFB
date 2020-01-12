//#define CONSOLE_DUMP
//#define HAS_DATALENGTH_FIELD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using vJoyInterfaceWrap;
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace vJoyIOFeeder.IOCommAgents
{
    /// <summary>
    /// The IOBoard "Simple" 1-byte protocol (human readable).
    /// 
    /// Each frame is composed by one or multiple command code on 1 byte + data
    /// The end-of-frame is given by a '\n' char.
    /// The IO board can implement a watchdog mechanism that switches to internal
    /// safety state, like for example shutting down torque control of a motor.
    /// 
    /// 
    /// From PC (master) to IOBoard (slave)
    /// -----------------------------------
    /// Single command frame
    /// - VX.Y.Z.W = send version. Should be acknowledged by a response version
    ///   *Cannot be used while IO board streaming is on.*
    /// - G = get description of hardware
    ///   *Cannot be used while IO board streaming is on.*
    /// - U = give single status update (polling mode)
    ///   *Cannot be used while IO board streaming is on.*
    /// - W = start/refresh periodic watchdog (could be used to detect failure
    ///       on PC side and apply safety outputs, like zeroing torque commands)
    /// - T = terminate/disable periodic watchdog (debug mode only!)
    /// - S = start streaming and 
    /// - H = halt(stop) streaming
    /// 
    /// Combined packed commands frame:
    /// - OXX = set 8bits digital output (8x1)
    /// - PXXX = set 12bits PWM (=analog) output (1x12)
    /// - EXXXXXXXX = force 32 bits encoder value to given position
    ///   (like a 0 or homing)
    ///   
    /// 
    /// From IOBoard (slave) to PC (master)
    /// -----------------------------------
    /// 
    /// Single command frame
    /// - V = IOBoard version, answer to a master 'V' frame
    ///   *Will not be received while IO board streaming is on.*
    /// - SXXXX = 16 bits status/error code when something unexpected happen.
    /// - MZZZZZZZ = Debug/text message, will terminate the frame
    /// 
    /// Combined packed commands frame:
    /// - IXX = 8bits digital input (8x1) 0..0xFF
    /// - AXXX = 12bits Analog input (1x12) 0..0xFFF
    /// - EXXXXXXXX = 32bits Encoder input (1x32) 0..0xFFFFFFFF
    /// - FYYYYYYYYZZZZZZZZ = state vector of wheel, as a 2x32bits float vector: Y=vel, Z=accel
    /// 
    /// Example for Handshaking:
    //    [Master]             [Slave]
    ///   "V1.0.0.10\n"   ->  (check version)
    ///                   <-  "V1.0.0.1\n" (reply with a V if ok)
    /// => PC version is 1.0.0.10. Slave version is 1.0.0.1
    ///                   or
    ///                   <-  "S0001 WRONG VERSION\n" (reply with error code and
    ///                       a message if not ok)
    /// 
    /// Example for get hardware description:
    ///   "G\n            ->
    ///                   <-  "1I3A1O1P1E1F1\n"
    /// => IO board has 1 digital input block (x8), 3 analog inputs, 1 digital
    /// output block, 1 PWM output, 1 Encoder input, 1 additional state vector
    ///
    /// Example for starting streaming:
    //    [Master]             [Slave]
    ///   "S\n"   ->           (start streaming)
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <- ...
    /// => IOBoard sends a data frame regularly
    /// 
    /// Example for stoping streaming:
    ///    (Master)             (Slave)
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <-  "I00A000A000A000E00000000\n"
    ///   "H\n"           ->  (stops periodic sending)
    /// => IO board stops sending data frames
    /// 
    /// Example for sending outputs:
    ///    (Master)             (Slave)
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <-  "I00A000A000A000E00000000\n"
    ///   "W\n"           ->   (enable/refresh watchdog)
    ///   "O01P00F\n"     ->   (1 output + 1 PWM set to 0xF)
    ///                   <-  "I00A000A000A000E00000000\n"
    ///                   <-  "I00A000A000A000E00000000\n"
    ///   "W\n"           ->   (refresh watchdog)
    ///                   <-  "I00A000A000A000E00000000\n"
    /// => IO board activated outputs (not seen in inputs)
    ///
    /// In case the watchdog is triggered, the IO board may sends an unexpected
    /// "SXXXX WATCHDOG TRIGGERED\n" error code with a status indicating that 
    /// the watchdog has triggered.
    /// 
    /// </summary>
    public class USBSerialIO :
        IDisposable
    {
        public uint[] DigitalInputs8 { get; protected set; }
        public uint[] AnalogInputs { get; protected set; }
        public ulong[] EncoderInputs { get; protected set; }
        public uint[] DigitalOutputs8 { get; protected set; }
        public uint[] AnalogOutputs { get; protected set; }
        public float[] WheelStates { get; protected set; }

        protected SerialPort ComIOBoard = null;
        public bool IsOpen { get { return ComIOBoard.IsOpen; } }

        public USBSerialIO(string port, int baudrate = 115200)
        {
            ComIOBoard = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            ComIOBoard.NewLine = "\n";
            ComIOBoard.DtrEnable = true;
            ComIOBoard.RtsEnable = true;

            ComIOBoard.ReadTimeout = 100;
            ComIOBoard.WriteTimeout = 100;
            // Not usefull for USB (buffer is already reaallly large)
            //ComIOBoard.ReadBufferSize = 15;
            DigitalInputs8 = new uint[0];
            AnalogInputs = new uint[0];
            EncoderInputs = new ulong[0];
            DigitalOutputs8 = new uint[0];
            AnalogOutputs = new uint[0];
            WheelStates = new float[0];
        }

        protected bool initDone = false;

        public void OpenComm()
        {
//#if CONSOLE_DUMP
            Console.WriteLine("Opening " + this.ComIOBoard.PortName);
//#endif
            ComIOBoard.Open();
            ComIOBoard.DiscardInBuffer();
            ComIOBoard.DiscardOutBuffer();
//#if CONSOLE_DUMP
            Console.WriteLine("Opened, now performing handshaking");
//#endif

            // Should discover automatically what is available
            // after connected (handshaking)
            VersionHandShaking();
            initDone = true;
        }

        public void CloseComm()
        {
            HaltStreaming();
            initDone = false;
            ComIOBoard.Close();
        }

        public void Dispose()
        {
            ComIOBoard.Dispose();
        }


        protected bool ProcessOneMessage()
        {
            bool atLeastOneProcessed = false;
            if (!this.IsOpen)
                return false;
            if (ComIOBoard.BytesToRead < 2)
                return false;
            // Parse message from IO board
            var mesg = ComIOBoard.ReadLine();
            //Console.WriteLine("Received Input mesg " + mesg);

            int dinblock = 0;
            int ain = 0;
            int enc = 0;

            int index = 0;
            while (index < mesg.Length) {
                // Read one command (multiple bytes) at a time, until message is consumed.

                // Read command code (header)
                var commandCode = mesg[index++];
                // Length of data is fixed as of today (header does not include any hint)
                // In the future, could add a datalength field as in a CAN frame
#if HAS_DATALENGTH_FIELD
                int.TryParse(mesg[index++].ToString(), out var dataLength);
#else
                int dataLength = 0;
#endif
                try {
                    switch (commandCode) {
                        case '\r':
                            // End of frame !
                            index = mesg.Length;
                            break;
                        case 'V':
                            // Version
#if CONSOLE_DUMP
                            Console.WriteLine("Received version " + mesg);
#endif
                            index = mesg.Length;
                            break;
                        case 'G':
                            // Hardware
#if CONSOLE_DUMP
                            Console.WriteLine("Received hardware description " + mesg);
#endif
                            index = mesg.Length;
                            break;
                        case 'S':
                            // Error code SXXXX
#if CONSOLE_DUMP
                            Console.WriteLine("Received error " + mesg);
#endif
                            index = mesg.Length;
                            break;
                        case 'M':
                            Console.WriteLine("Debug:" + mesg.Substring(index, mesg.Length-index-1));
                            index = mesg.Length;
                            break;

                        case 'I':
                            // IXX = digital inputs on 2 nibbles, 8 binary inputs (equal to 1 PORT)
                            // Or IO board gives a value between 0..0xFF
#if !HAS_DATALENGTH_FIELD
                            dataLength = 2;
#endif
                            if (initDone) {
                                uint.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var dig8);
                                this.DigitalInputs8[dinblock++] = dig8;
                            }
                            break;

                        case 'A':
                            // AXXX = analog input on 3 nibbles (12bits resolution), 0..0xFFF input range not scaled yet
                            // IO board gives a value between 0..3FF (1023), scale it to axis min/max afterwards
#if !HAS_DATALENGTH_FIELD
                            dataLength = 3;
#endif
                            if (initDone) {
                                uint.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var analog12);
                                this.AnalogInputs[ain++] = analog12;
                            }
                            break;
                        case 'E':
                            // EXXXXXXXX = encoder position on 8 nibbles (32bits resolution), 0..0xFFFFFFFF input range not scaled yet
                            // IO board gives a value between 0..0xFFFFFFFFF, no scaling
                            dataLength = 8;
                            if (initDone) {
                                ulong.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var encoder);
                                this.EncoderInputs[enc++] = encoder;
                            }
                            break;
                        case 'F':
                            // FYYYYYYYYZZZZZZZZ = additional states of wheel as a 2x32bits float vector: Y=vel, Z=accel
                            // IO board gives a value in 32bits float that must be converted
                            dataLength = 16;
                            if (initDone) {
                                // Get 2x32 bits uint
                                ulong.TryParse(mesg.Substring(index, 8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var vel_int);
                                ulong.TryParse(mesg.Substring(index+8, 8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var acc_int);
                                var vel_reversed_bytes = BitConverter.GetBytes((UInt32)vel_int);
                                var acc_reversed_bytes = BitConverter.GetBytes((UInt32)acc_int);
                                /*var vel_bytes = new byte[4];
                                var acc_bytes = new byte[4];
                                for (int i=0; i<4; i++) {
                                    vel_bytes[i] = vel_reversed_bytes[3 - i];
                                    acc_bytes[i] = acc_reversed_bytes[3 - i];
                                }*/
                                // Convert to floats
                                float vel = BitConverter.ToSingle(vel_reversed_bytes, 0);
                                float acc = BitConverter.ToSingle(acc_reversed_bytes, 0);
                                this.WheelStates[0] = vel;
                                this.WheelStates[1] = acc;
                            }
                            break;

                        default:
                            Console.WriteLine("Unknown command:" + mesg);
                            index = mesg.Length;
                            break;
                    }
                    index += dataLength;
                    atLeastOneProcessed = true;
                } catch (Exception ex) {
                    Console.WriteLine("Error " + ex.Message + "while processing command");
                }
            }

            return atLeastOneProcessed;
        }

        protected int ProcessAllMessages(int maxCount = 10)
        {
            int processed = 0;
            // Process incoming messages
            while ((processed < maxCount) && ProcessOneMessage()) {
                processed++;
            }
            return processed;
        }

        protected void SendOneMessage(string mesg)
        {
#if CONSOLE_DUMP
            Console.WriteLine("Send>>" + mesg);
#endif
            if (ComIOBoard.IsOpen)
                ComIOBoard.WriteLine(mesg);
        }

        public void UpdateSingle()
        {
            SendOneMessage("U");
            ProcessAllMessages();
        }

        public void UpdateOnStreaming()
        {
            ProcessAllMessages();
        }

        public void VersionHandShaking()
        {
            // Just in case...
            HaltStreaming();

            // Empty buffer
            ProcessAllMessages();
            // Exchange version ID and protocol version
            SendOneMessage("V1.0.0.0");
            // Exchange description of available IOs
            ProcessAllMessages();

            SendOneMessage("G");
            // Exchange description of available IOs
            ProcessAllMessages();

            DigitalInputs8 = new uint[1];
            AnalogInputs = new uint[3];
            EncoderInputs = new ulong[1];
            DigitalOutputs8 = new uint[1];
            AnalogOutputs = new uint[1];
            WheelStates = new float[2];
        }

        public void GetStreaming()
        {
            SendOneMessage("S");
        }
        /// <summary>
        /// Enable or refresh watchdog
        /// </summary>
        public void EnableWD()
        {
            SendOneMessage("W");
        }
        /// <summary>
        /// Disable or terminate watchdog
        /// </summary>
        public void DisableWD()
        {
            SendOneMessage("T");
        }
        public void StartStreaming()
        {
            SendOneMessage("S");
        }

        public void HaltStreaming()
        {
            SendOneMessage("H");
        }

        public void SendDigitalOutput(uint output8)
        {
            // Outputs
            // OXX = Digital outputs on 2 nibbles = 8 binary inputs (equal to 1 PORT)
            var mesg = "O" + output8.ToString("X2").Substring(0, 2);
            SendOneMessage(mesg);
        }

        public void SendAnalogOutput(uint analog12)
        {
            // Outputs
            // PXXX = PWM outputs on 3 nibbles = 1024
            var mesg = "P" + analog12.ToString("X3").Substring(0, 3);
            SendOneMessage(mesg);
        }
        public void SendEncoderPosition(uint pos32)
        {
            // Outputs
            // EXXXXXXXX = encoder position on 8 nibbles = 1024
            var mesg = "E" + pos32.ToString("X8").Substring(0, 8);
            SendOneMessage(mesg);
        }


        public void SendOutputs()
        {
            var mesg = "O" + this.DigitalOutputs8[0].ToString("X2").Substring(0, 2) + "P" + this.AnalogOutputs[0].ToString("X3").Substring(0, 3);
            SendOneMessage(mesg);
        }
    }
}

