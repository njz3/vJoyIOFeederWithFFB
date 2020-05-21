//#define HAS_DATALENGTH_FIELD


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using vJoyInterfaceWrap;
using vJoyIOFeeder.vJoyIOFeederAPI;
using System.Threading;
using vJoyIOFeeder.Utils;
using System.Diagnostics;

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
    /// - ?XXXXYYYY = send protocol version XXXX=major, YYYY=minor. Should be 
    ///       acknowledged by a response protocole version "?"
    ///   *Cannot be used while IO board streaming is on.*
    ///   
    /// - VX.Y.Z.W = send software version. Should be acknowledged by a response
    ///       version
    ///   *Cannot be used while IO board streaming is on.*
    ///   
    /// - G = get description of hardware (gadgets)
    ///   *Cannot be used while IO board streaming is on.*
    ///   
    /// - U = give single status update (polling mode)
    ///   *Cannot be used while IO board streaming is on.*
    ///   
    /// - W = start/refresh periodic watchdog (could be used to detect failure
    ///       on PC side and apply safety outputs, like zeroing torque commands)
    /// - T = terminate/disable periodic watchdog (debug mode only!)
    /// - S = start streaming and 
    /// - H = halt(stop) streaming
    /// - ~ = reset board (reboot)
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
    /// - ? = Protocol version, answer to a master '?' frame
    ///   *Will not be received while IO board streaming is on.*
    ///   
    /// - V = IOBoard version, answer to a master 'V' frame
    ///   *Will not be received while IO board streaming is on.*
    ///   
    /// - GXXXX = hardware description of board (gadgets).
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
    ///                   <-  "GI1A3O1P1E1F1\n"
    /// => IO board has 1 digital input block (x8), 3 analog inputs, 1 digital
    /// output block (x8), 1 PWM output, 1 Encoder input, 1 additional state vector
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
        public byte[] DigitalInputs8 { get; protected set; }
        public byte[] DigitalOutputs8 { get; protected set; }
        public UInt16[] AnalogInputs { get; protected set; }
        public UInt16[] AnalogOutputs { get; protected set; }
        public UInt32[] EncoderInputs { get; protected set; }
        public float[] WheelStates { get; protected set; }

        protected SerialPort ComIOBoard = null;
        public bool IsOpen { get { return ComIOBoard.IsOpen; } }
        public bool ProtocolVersionReceived { get; protected set; } = false;
        public bool BoardVersionReceived { get; protected set; } = false;
        public bool HardwareDescriptorReceived { get; protected set; } = false;
        public bool HandShakingDone { get; protected set; } = false;
        public bool InitDone { get; protected set; } = false;

        public string COMPortName {
            get {
                if (ComIOBoard != null)
                    return ComIOBoard.PortName;
                else
                    return "Undef";
            }
        }
        protected static void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[USBSerial] " + text, level);
        }

        protected static void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[USBSerial] " + text, args);
        }

        #region Constructor
        public USBSerialIO(string port, int baudrate = 1000000)
        {
            ComIOBoard = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            ComIOBoard.NewLine = "\n";
            ComIOBoard.DtrEnable = true;
            ComIOBoard.RtsEnable = true;

            // High timeout for handshaking and basic IO
            ComIOBoard.ReadTimeout = 100;
            ComIOBoard.WriteTimeout = 100;
            ComIOBoard.ReceivedBytesThreshold = 100;
            // Not usefull for USB (buffer is already reaallly large)
            //ComIOBoard.ReadBufferSize = 15;

            DigitalInputs8 = new byte[0];
            DigitalOutputs8 = new byte[0];
            AnalogInputs = new UInt16[0];
            AnalogOutputs = new UInt16[0];
            EncoderInputs = new UInt32[0];
            WheelStates = new float[0];
        }
        #endregion

        #region Open/close/dispose

        protected bool openDone = false;
        protected System.IO.Stream Stream;

        public void OpenComm()
        {
            if (vJoyManager.Config.Application.VerboseSerialIO) {
                Log("Opening " + this.ComIOBoard.PortName);
            }
            ComIOBoard.Open();
            DiscardBuffers();
            // Do know why, but a sleep of 1s is required to make serial port working...
            Thread.Sleep(1000);
            Stream = ComIOBoard.BaseStream;
            if (vJoyManager.Config.Application.VerboseSerialIO) {
                Log("Opened, now performing handshaking");
            }

            // Should discover automatically what is available
            // after connected (handshaking)
            HandShaking();
            if (vJoyManager.Config.Application.VerboseSerialIO) {
                Log("Done, ioboard ready");
            }

            if ((HardwareDescriptor!=null) && HandShakingDone) {
                openDone = true;
            }
        }

        public void CloseComm()
        {
            DiscardBuffers();

            HaltStreaming();
            openDone = false;
            HandShakingDone = false;
            InitDone = false;
            if (ComIOBoard.IsOpen) {
                ComIOBoard.Close();
            }
        }

        public void Dispose()
        {
            if (Stream!=null)
                Stream.Dispose();
            Stream = null;
            if (ComIOBoard!=null)
                ComIOBoard.Dispose();
            ComIOBoard = null;
        }

        public void DiscardBuffers()
        {
            if (ComIOBoard!=null) {
                ComIOBoard.DiscardInBuffer();
                ComIOBoard.DiscardOutBuffer();
            }
        }
        #endregion

        #region Handshaking/version

        protected string HardwareDescriptor = null;
        protected bool ParseHardwareDescriptor(string hwddescriptor)
        {
            HardwareDescriptorReceived = false;
            bool stt = true;
            int index = 0;
            uint dinblock = 0;
            uint doutblock = 0;
            uint ain = 0;
            uint fullstate = 0;
            uint pwm = 0;
            uint enc = 0;
            try {
                while (index < hwddescriptor.Length) {
                    var blocktype = hwddescriptor[index++];
                    int dataLength = 0;
                    switch (blocktype) {
                        case '\r':
                            // End of frame !
                            index = hwddescriptor.Length;
                            break;
                        case 'I': {
                                // Digital input block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var di8);
                                dinblock += di8;
                            }
                            break;
                        case 'O': {
                                // Digital output block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var do8);
                                doutblock += do8;
                            }
                            break;
                        case 'A': {
                                // Analog input block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var ai);
                                ain += ai;
                            }
                            break;
                        case 'P': {
                                // PWM output block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var pw);
                                pwm += pw;
                            }
                            break;
                        case 'F': {
                                // Full state block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var FS);
                                fullstate += FS;
                            }
                            break;
                        case 'E': {
                                // Encoder block
                                dataLength = 1;
                                uint.TryParse(hwddescriptor.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var en);
                                enc += en;
                            }
                            break;
                        default: {
                                Log("Unknown hardware descriptor:" + hwddescriptor, LogLevels.IMPORTANT);
                                index = hwddescriptor.Length;
                            }
                            break;
                    }
                    index += dataLength;
                }

                // Memory allocation for blocks
                DigitalInputs8 = new byte[dinblock];
                DigitalOutputs8 = new byte[doutblock];
                AnalogInputs = new UInt16[ain];
                AnalogOutputs = new UInt16[pwm];
                EncoderInputs = new UInt32[enc];
                WheelStates = new float[2 * fullstate];

                // Save hardware descriptor
                HardwareDescriptor = hwddescriptor;
                HardwareDescriptorReceived = true;
            } catch (Exception ex) {
                Log("Parse hardware descriptor got exception " + ex.Message, LogLevels.INFORMATIVE);
                // Wrong format for version give up
                stt = false;
            }
            return stt;
        }

        public UInt32 BoardProtocoleMajor { get; protected set; } = 0;
        public UInt32 BoardProtocoleMinor { get; protected set; } = 0;
        protected bool ParseProtocolVersion(string version)
        {
            bool stt = true;
            BoardProtocoleMajor = 0;
            BoardProtocoleMinor = 0;
            ProtocolVersionReceived = false;
            try {
                BoardProtocoleMajor = UInt32.Parse(version.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                BoardProtocoleMinor = UInt32.Parse(version.Substring(4, 4), System.Globalization.NumberStyles.HexNumber);
                ProtocolVersionReceived = true;
            } catch (Exception ex) {
                Log("Parse protocol version got exception " + ex.Message, LogLevels.INFORMATIVE);
                // Wrong format for version give up
                stt = false;
            }
            return stt;
        }

        public Version BoardVersion { get; protected set; } = new Version(0, 0, 0, 0);
        public string BoardDescription { get; protected set; } = "";

        protected bool ParseBoardVersion(string version)
        {
            bool stt = true;
            try {
                int idx_spc = version.IndexOf(' ');
                if (idx_spc > 0) BoardVersion = new Version(version.Substring(0, idx_spc));
                BoardDescription = version.Substring(idx_spc + 1, version.Length - idx_spc - 1);
                BoardVersionReceived = true;
            } catch (Exception ex) {
                Log("Parse board version got exception " + ex.Message, LogLevels.INFORMATIVE);
                // Wrong format for version give up
                stt = false;
            }
            return stt;
        }

        public void HandShaking()
        {
            // Just in case...
            HaltStreaming();

            // Wait a little for processing
            Thread.Sleep(16);
            // Activate debugging on IOboard ?
            //SendOneMessage("D");

            // Empty input buffer
            ProcessAllMessages(100);

            // Clear handshaking flags
            HandShakingDone = false;
            ProtocolVersionReceived = false;
            BoardVersionReceived = false;
            HardwareDescriptorReceived = false;

            // Exchange version ID and protocol version

            // Send protocole version
            var timeout = Stopwatch.StartNew();

            SendOneMessage("?" +
                String.Format("{0:X4}", vJoyManager.Config.Hardware.ProtocolVersionMajor) +
                String.Format("{0:X4}", vJoyManager.Config.Hardware.ProtocolVersionMinor));

            // Wait a little for a reply and check result
            while (!this.ProtocolVersionReceived) {
                Thread.Sleep(16);
                ProcessAllMessages(10);
                if (timeout.ElapsedMilliseconds>200) {
                    // Timeout
                    // Error accepted for now...
                    if (vJoyManager.Config.Hardware.EnforceHandshakingVersionChecks) {
                        throw new InvalidOperationException("Handshaking failed with no protocole message");
                    }
                    break;
                }
            }

            // Send version
            SendOneMessage("V1.0.0.0");
            // Wait a little for a reply and check result
            timeout.Restart();
            while (!this.BoardVersionReceived) {
                Thread.Sleep(16);
                ProcessAllMessages(10);
                if (timeout.ElapsedMilliseconds>200) {
                    // Timeout
                    throw new InvalidOperationException("Handshaking failed with no version message");
                }
            }

            // Exchange description of available IOs
            SendOneMessage("G");
            // Wait a little for a reply and check result
            timeout.Restart();
            while (!this.HardwareDescriptorReceived) {
                Thread.Sleep(32);
                ProcessAllMessages(10);
                if (timeout.ElapsedMilliseconds>500) {
                    // Timeout
                    throw new InvalidOperationException("Handshaking failed with no version message");
                }
            }

            // active debug mode
            if (vJoyManager.Config.Application.VerboseSerialIO) {
                SendOneMessage("D");
            }
            HandShakingDone = true;
        }

        /// <summary>
        /// Perform initialization on IO board.
        /// This must be done after handshaking.
        /// Warning, this is a blocking function!
        /// </summary>
        public void PerformInit()
        {
            InitDone = false;
            Log("Performing IO board initialization", LogLevels.IMPORTANT);
            // Send "I" message
            SendOneMessage("I");
            // Wait for Init flag to be set by IO board or timeout
            var timeout = Stopwatch.StartNew();
            while (!InitDone) {
                Thread.Sleep(32);
                ProcessAllMessages();
                if (timeout.ElapsedMilliseconds > vJoyManager.Config.Hardware.TimeoutForInit_ms) {
                    Log("IO board initialization failed !!", LogLevels.ERROR);
                    return;
                }
            }
            Log("IO board initialization done", LogLevels.IMPORTANT);
        }


        #endregion

        #region Process incoming
        public delegate void MessageHandlerMethod(string msg);
        public event MessageHandlerMethod MessageHandlerEvent;

        protected bool ProcessOneMessage()
        {
            bool atLeastOneProcessed = false;
            if (ComIOBoard.BytesToRead < 2)
                return false;
            // Parse message from IO board
            var mesg = ComIOBoard.ReadLine();
            if (vJoyManager.Config.Application.VerboseSerialIODumpFrames) {
                Log("Recv<<" + mesg);
            }
            try {
                uint dinblock = 0;
                uint ain = 0;
                uint enc = 0;

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

                    switch (commandCode) {
                        case '\r': {
                                // End of frame !
                                index = mesg.Length;
                            }
                            break;
                        case '?': {
                                // Protocol Version
                                ParseProtocolVersion(mesg.Substring(index, mesg.Length - index));
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received protocol version " + mesg);
                                }
                                index = mesg.Length;
                            }
                            break;
                        case 'V': {
                                // Software Version
                                ParseBoardVersion(mesg.Substring(index, mesg.Length - index));
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received board version " + mesg);
                                }
                                index = mesg.Length;
                            }
                            break;
                        case 'G': {
                                // Hardware descriptor
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received hardware description " + mesg);
                                }
                                ParseHardwareDescriptor(mesg.Substring(index, mesg.Length - index));
                                index = mesg.Length;
                            }
                            break;
                        case 'S': {
                                // Error code SXXXX
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received error " + mesg);
                                }
                                index = mesg.Length;
                            }
                            break;
                        case 'R': {
                                // Ready message
                                Log("IOBOARD:" + mesg.Substring(index, mesg.Length - index), LogLevels.DEBUG);
                                this.InitDone = true;
                                index = mesg.Length;
                            }
                            break;

                        case 'M': {
                                var message = mesg.Substring(index, mesg.Length - index);
                                Log("IOBOARD:" + message, LogLevels.DEBUG);
                                if (MessageHandlerEvent!=null)
                                    MessageHandlerEvent(message);
                                index = mesg.Length;
                            }
                            break;

                        case 'I': {
                                // IXX = digital inputs on 2 nibbles, 8 binary inputs (equal to 1 PORT)
                                // Or IO board gives a value between 0..0xFF
#if !HAS_DATALENGTH_FIELD
                                dataLength = 2;
#endif
                                if (openDone) {
                                    uint.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var dig8);
                                    if (dinblock<this.DigitalInputs8.Length) {
                                        this.DigitalInputs8[dinblock++] = (byte)dig8;
                                    } else {
                                        Log("Wrong number of digital inputs returned by IO board, mismatch between hardware descriptor and received frame", LogLevels.IMPORTANT);
                                    }
                                }
                            }
                            break;

                        case 'A': {
                                // AXXX = analog input on 3 nibbles (12bits resolution), 0..0xFFF input range not scaled yet
                                // IO board gives a value between 0..3FF (1023), scale it to axis min/max afterwards
#if !HAS_DATALENGTH_FIELD
                                dataLength = 3;
#endif
                                if (openDone) {
                                    uint.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var analog12);
                                    if (ain<this.AnalogInputs.Length) {
                                        this.AnalogInputs[ain++] = (UInt16)analog12;
                                    } else {
                                        Log("Wrong number of analog inputs returned by IO board, mismatch between hardware descriptor and received frame", LogLevels.IMPORTANT);
                                    }
                                }
                            }
                            break;
                        case 'E': {
                                // EXXXXXXXX = encoder position on 8 nibbles (32bits resolution), 0..0xFFFFFFFF input range not scaled yet
                                // IO board gives a value between 0..0xFFFFFFFFF, no scaling
                                dataLength = 8;
                                if (openDone) {
                                    ulong.TryParse(mesg.Substring(index, dataLength), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var encoder);
                                    if (enc<this.EncoderInputs.Length) {
                                        this.EncoderInputs[enc++] = (UInt32)encoder;
                                    } else {
                                        Log("Wrong number of encoder inputs returned by IO board, mismatch between hardware descriptor and received frame", LogLevels.IMPORTANT);
                                    }
                                }
                            }
                            break;
                        case 'F': {
                                // FYYYYYYYYZZZZZZZZ = additional states of wheel as a 2x32bits float vector: Y=vel, Z=accel
                                // IO board gives a value in 32bits float that must be converted
                                dataLength = 16;
                                if (openDone) {
                                    // Get 2x32 bits uint
                                    ulong.TryParse(mesg.Substring(index, 8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var vel_int);
                                    ulong.TryParse(mesg.Substring(index + 8, 8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var acc_int);
                                    var vel_bytes = BitConverter.GetBytes((UInt32)vel_int);
                                    var acc_bytes = BitConverter.GetBytes((UInt32)acc_int);
                                    // Convert to floats
                                    float vel = BitConverter.ToSingle(vel_bytes, 0);
                                    float acc = BitConverter.ToSingle(acc_bytes, 0);
                                    this.WheelStates[0] = vel;
                                    this.WheelStates[1] = acc;
                                }
                            }
                            break;

                        default: {
                                Log("Unknown command:" + mesg, LogLevels.DEBUG);
                                index = mesg.Length;
                            }
                            break;
                    }
                    index += dataLength;
                    atLeastOneProcessed = true;

                }
            } catch (Exception ex) {
                Log("ProcessOneMessage got exception " + ex.Message, LogLevels.DEBUG);
            }
            return atLeastOneProcessed;
        }

        StringBuilder myline = new StringBuilder(255);
        byte[] RecvBuffer = new byte[255];
        byte[] RecvByte = new byte[1];
        protected int ReadByte()
        {
            var task = Stream.ReadAsync(RecvByte, 0, 1);
            task.Wait();
            return task.Result;
        }
        protected int ReadFixedLength(int ReadFixedlength)
        {
            if (ReadFixedlength>RecvBuffer.Length)
                return -1;
            var task = Stream.ReadAsync(RecvBuffer, 0, ReadFixedlength);
            task.Wait();
            return task.Result;
        }

        protected bool ReadUntilNewline(out string line)
        {
            myline.Clear();
            do {
                var res = ReadByte();
                // Eol?
                if (res==-1)
                    break;
                var c = (char)RecvByte[0];
                // Newline?
                if (c=='\n')
                    break;

                myline.Append(c);

            } while (true);

            line = myline.ToString();

            if (line.Length>0)
                return true;

            return false;
        }

        protected bool ReadFixedLength(int readFixedlength, out string token)
        {
            /*
            var stream = ComIOBoard.BaseStream;
            myline.Clear();
            while (readFixedlength>0) {
                var c = ReadByte();
                // Eol?
                if (c==-1)
                    break;

                // Newline?
                if (c=='\n')
                    break;

                myline.Append((char)c);
                readFixedlength--;
            }
            */
            var res = ReadFixedLength(readFixedlength);
            if (res<0) {
                token = null;
                return false;
            }
            token = System.Text.Encoding.Default.GetString(RecvBuffer, 0, res);
            return true;
        }


        protected bool ProcessOneMessageStream()
        {
            bool atLeastOneProcessed = false;
            if (!this.IsOpen)
                return false;
            var stream = this.Stream;
            try {
                uint dinblock = 0;
                uint ain = 0;
                uint enc = 0;

                bool done = false;
                do {
                    // Read one command (multiple bytes) at a time, until message is consumed.
                    var c = stream.ReadByte();
                    // Eol?
                    if (c==-1)
                        break;

                    // Newline?
                    if (c=='\n')
                        break;

                    // Read command code (header)
                    char commandCode = (char)c;
                    // Length of data is fixed as of today (header does not include any hint)
                    // In the future, could add a datalength field as in a CAN frame
#if HAS_DATALENGTH_FIELD
                    int.TryParse(mesg[index++].ToString(), out var dataLength);
#else
                    int dataLength = 0;
#endif

                    switch (commandCode) {
                        case 'V': {
                                // Version read until newline
                                ReadUntilNewline(out var version);
                                ParseBoardVersion(version);
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received version " + version);
                                }
                                done = true;
                            }
                            break;
                        case 'G': {
                                // Hardware descriptor
                                ReadUntilNewline(out var gadget);
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received hardware description " + gadget);
                                }
                                ParseHardwareDescriptor(gadget);
                            }
                            break;
                        case 'S': {
                                // Error code SXXXX
                                ReadUntilNewline(out var error);
                                if (vJoyManager.Config.Application.VerboseSerialIO) {
                                    Log("Received error " + error);
                                }
                            }
                            break;
                        case 'M': {
                                ReadUntilNewline(out var msg);
                                Log("IOBOARD:" + msg, LogLevels.INFORMATIVE);
                            }
                            break;

                        case 'I': {
                                // IXX = digital inputs on 2 nibbles, 8 binary inputs (equal to 1 PORT)
                                // Or IO board gives a value between 0..0xFF
#if !HAS_DATALENGTH_FIELD
                                dataLength = 2;
#endif
                                if (openDone) {
                                    ReadFixedLength(dataLength, out var token);
                                    uint.TryParse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var dig8);
                                    this.DigitalInputs8[dinblock++] = (byte)dig8;
                                }
                            }
                            break;

                        case 'A': {
                                // AXXX = analog input on 3 nibbles (12bits resolution), 0..0xFFF input range not scaled yet
                                // IO board gives a value between 0..3FF (1023), scale it to axis min/max afterwards
#if !HAS_DATALENGTH_FIELD
                                dataLength = 3;
#endif
                                if (openDone) {
                                    ReadFixedLength(dataLength, out var token);
                                    uint.TryParse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var analog12);
                                    this.AnalogInputs[ain++] = (UInt16)analog12;
                                }
                            }
                            break;
                        case 'E': {
                                // EXXXXXXXX = encoder position on 8 nibbles (32bits resolution), 0..0xFFFFFFFF input range not scaled yet
                                // IO board gives a value between 0..0xFFFFFFFFF, no scaling
                                dataLength = 8;
                                if (openDone) {
                                    ReadFixedLength(dataLength, out var token);
                                    ulong.TryParse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var encoder);
                                    this.EncoderInputs[enc++] = (UInt32)encoder;
                                }
                            }
                            break;
                        case 'F': {
                                // FYYYYYYYYZZZZZZZZ = additional states of wheel as a 2x32bits float vector: Y=vel, Z=accel
                                // IO board gives a value in 32bits float that must be converted
                                dataLength = 16;
                                if (openDone) {
                                    // Get 2x32 bits uint
                                    ReadFixedLength(8, out var token);
                                    ulong.TryParse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var vel_int);
                                    ReadFixedLength(8, out token);
                                    ulong.TryParse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var acc_int);
                                    var vel_bytes = BitConverter.GetBytes((UInt32)vel_int);
                                    var acc_bytes = BitConverter.GetBytes((UInt32)acc_int);
                                    // Convert to floats
                                    float vel = BitConverter.ToSingle(vel_bytes, 0);
                                    float acc = BitConverter.ToSingle(acc_bytes, 0);
                                    this.WheelStates[0] = vel;
                                    this.WheelStates[1] = acc;
                                }
                            }
                            break;

                        default: {
                                Log("Unknown command:" + c, LogLevels.DEBUG);
                                done = true;
                            }
                            break;
                    }
                    atLeastOneProcessed = true;

                } while (!done);

            } catch (Exception ex) {
                Log("ProcessOneMessage got exception " + ex.Message, LogLevels.DEBUG);
            }

            return atLeastOneProcessed;
        }

        protected int ProcessAllMessages(int maxCount = 10)
        {
            if (!this.IsOpen)
                return 0;

            int processed = 0;
            // Process incoming messages
            while ((processed < maxCount) && ProcessOneMessage()) {
                processed++;
            }
            return processed;
        }
        #endregion

        #region Commands/sending
        protected void SendOneMessage(string mesg)
        {
            if (vJoyManager.Config.Application.VerboseSerialIODumpFrames) {
                Log("Send>>" + mesg);
            }
            if (ComIOBoard.IsOpen) {
                ComIOBoard.WriteLine(mesg);
            } else {
                if (vJoyManager.Config.Application.VerboseSerialIO) {
                    Log("Serial port not ready !");
                }
            }
        }

        public int UpdateSingle()
        {
            SendOneMessage("U");
            return ProcessAllMessages();
        }

        public int UpdateOnStreaming(int nbmsg = 1)
        {
            return ProcessAllMessages(nbmsg);
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

        /// <summary>
        /// Start streaming
        /// </summary>
        public void StartStreaming()
        {
            SendOneMessage("S");
        }

        /// <summary>
        /// Stop streaming
        /// </summary>
        public void HaltStreaming()
        {
            SendOneMessage("H");
        }
        public void DebugMode(bool enable)
        {
            if (enable)
                SendOneMessage("D");
            else 
                SendOneMessage("d");
        }
        /// <summary>
        /// Reset board
        /// </summary>
        public void ResetBoard()
        {
            SendOneMessage("~");
        }

        /// <summary>
        /// Send command line for interpreter, like a configuration command
        /// </summary>
        public void SendCommand(string commandline)
        {
            SendOneMessage("C"+commandline);
        }



        List<string> AllKeywords;
        List<string> AllParams;
        bool HelpDone = false;
        void HelpDecoder(string line)
        {
            var tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (line.StartsWith("Help listed ")) {
                int.TryParse(tokens[2], out var keywords);
                int.TryParse(tokens[5], out var @params);
                if (keywords != AllKeywords.Count) {
                    Log("Pb enumering keywords", LogLevels.IMPORTANT);
                }
                if (@params != AllParams.Count) {
                    Log("Pb enumering params", LogLevels.IMPORTANT);
                }
                Log("Help command done");
                HelpDone = true;
            } else if (line.StartsWith("Keyword ")) {
                AllKeywords.Add(tokens[1]);
            } else if (line.StartsWith("Param ")) {
                AllParams.Add(tokens[1]);
            }
        }

        /// <summary>
        /// Retrieve available commands through 'help'
        /// </summary>
        public void RetrieveCommands(List<string> command, List<string> param)
        {
            AllKeywords = command;
            AllParams = param;

            HaltStreaming();
            ProcessAllMessages();

            HelpDone = false;
            AllKeywords.Clear();
            AllParams.Clear();
            MessageHandlerEvent += HelpDecoder;

            SendOneMessage("Chelp");

            var timeout = Stopwatch.StartNew();
            do {
                Thread.Sleep(16);
                if (timeout.ElapsedMilliseconds>200) {
                    break;
                }
            } while (ProcessAllMessages()>0 && !HelpDone);

            MessageHandlerEvent -= HelpDecoder;
            if (HelpDone) {
                foreach (var key in AllKeywords) {
                    Log(key);
                    Console.WriteLine(key);
                }
                foreach (var key in AllParams) {
                    Log(key);
                    Console.WriteLine(key);
                }
            } else {
                Log("Error help");
            }
        }


        string GetParam;
        UInt32 GetValue;
        bool GetDone;
        void GetDecoder(string line)
        {
            if (line.Contains("=")) {
                var tokens = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                GetParam = tokens[0];
                UInt32.TryParse(tokens[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out GetValue);
                Log("Get command done");
                GetDone = true;
            }
        }

        /// <summary>
        /// Get a parameter value
        /// </summary>
        public void GetParameter(string param, out UInt32 value)
        {
            ProcessAllMessages();

            GetDone = false;
            MessageHandlerEvent += GetDecoder;

            SendOneMessage("Cget " + param);

            var timeout = Stopwatch.StartNew();
            do {
                Thread.Sleep(16);
                if (timeout.ElapsedMilliseconds>200) {
                    break;
                }
            } while (ProcessAllMessages()>0 && !GetDone);

            MessageHandlerEvent -= GetDecoder;
            value = GetValue;
            if (GetDone) {
                Log(GetParam + "=" + value);
            } else {
                Log("Error get on " + GetParam);
            }
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        public void SetParameter(string param, UInt32 value)
        {
            ProcessAllMessages();

            SendOneMessage("Cset " + param + "=" + value.ToString("X2"));
        }

        public void SendDigitalOutputs(uint[] output8)
        {
            // Outputs
            // OXX = Digital outputs on 2 nibbles = 8 binary inputs (equal to 1 PORT)
            StringBuilder mesg = new StringBuilder();
            for (int i = 0; i < this.DigitalOutputs8.Length; i++) {
                mesg.Append("O");
                mesg.Append(output8[i].ToString("X2").Substring(0, 2));
            }
            SendOneMessage(mesg.ToString());
        }

        public void SendAnalogOutputs(uint[] analog12)
        {
            // Outputs
            // PXXX = PWM outputs on 3 nibbles = 1024
            StringBuilder mesg = new StringBuilder();
            for (int i = 0; i < this.DigitalOutputs8.Length; i++) {
                mesg.Append("P");
                mesg.Append(analog12[i].ToString("X3").Substring(0, 3));
            }
            SendOneMessage(mesg.ToString());
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
            StringBuilder mesg = new StringBuilder();
            for (int i = 0; i < this.DigitalOutputs8.Length; i++) {
                mesg.Append("O");
                mesg.Append(this.DigitalOutputs8[i].ToString("X2").Substring(0, 2));
            }
            for (int i = 0; i < this.AnalogOutputs.Length; i++) {
                mesg.Append("P");
                mesg.Append(this.AnalogOutputs[i].ToString("X3").Substring(0, 3));
            }
            SendOneMessage(mesg.ToString());
        }

        #endregion

        #region Utilities
        public static USBSerialIO[] ScanAllCOMPortsForIOBoards()
        {
            string[] ports = SerialPort.GetPortNames();
            List<USBSerialIO> ioboards = new List<USBSerialIO>();

            Log("The following serial ports were found:");
            // Display each port name to the console.
            foreach (string port in ports) {
                Log(port);
            }
            Log("Attempting to connect each with " + vJoyManager.Config.Hardware.SerialPortSpeed + "bauds...");
            // Display each port name to the console.
            foreach (string port in ports) {
                // Do a tentative to open it with handshaking
                USBSerialIO board = new USBSerialIO(port, (int)vJoyManager.Config.Hardware.SerialPortSpeed);
                try {
                    board.OpenComm();
                    if (board.HandShakingDone)
                        ioboards.Add(board);
                } catch (Exception ex) {
                    Log("Error on port " + port + ", reason " + ex.Message);
                    try {
                        board.CloseComm();
                    } catch (Exception) {

                    }
                }
            }
            return ioboards.ToArray();
        }
        #endregion
    }
}

