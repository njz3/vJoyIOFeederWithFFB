using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackForceFeeder.Utils;

namespace BackForceFeeder.Outputs
{
    /// <summary>
    /// MAME network output agent.
    /// Use local TCP connection on port 8000
    /// </summary>
    public class MAMEOutputsNetAgent : MAMEOutputsAgent
    {
        TcpClient Client;
        UdpClient UdpListener;
        int TcpPort = 8000;
        int UdpPort = 8001;

        public MAMEOutputsNetAgent() :
            base()
        {
            Log("NetAgent created", LogLevels.DEBUG);
        }

        bool detected = false;
        protected override void ManagerThreadMethod()
        {
            Log("Entering thread", LogLevels.INFORMATIVE);
            Client = new TcpClient();
            
            detected = false;
            while (Running) {

                if (detected) {
                    ConnectToOutput();
                } else {
                    // UDP packet received ?
                    ListenForUDP();
                    // Try TCP ?
                    ConnectToOutput();
                }
                Thread.Sleep(32);
            }
            Log("Thread done", LogLevels.INFORMATIVE);
        }
        protected void ListenForUDP()
        {
            UdpListener = new UdpClient(UdpPort);
            UdpListener.Client.ReceiveTimeout = 1000;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, UdpPort);
            try {
                while (!detected && Running) {
                    byte[] bytes = UdpListener.Receive(ref groupEP);
                    string msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    string text = "Received broadcast from " + groupEP.ToString() + Environment.NewLine;
                    text += msg;
                    Logger.Log("[MAMENetOutput] " + text, LogLevels.INFORMATIVE);
                    if (LineSplitter(msg, out var lines) && lines.Count==2) {
                        if (EquSplitter(lines[0], out var gametokens)) {
                            string game = gametokens[1];
                            this.GameProfile = game;
                        }
                        if (EquSplitter(lines[1], out var tcptokens)) {
                            string tcp = tcptokens[1];
                            int.TryParse(tcp, out TcpPort);
                        }
                    }
                    detected = true;
                }
            } catch (SocketException e) {
                Logger.Log("[MAMENetOutput] ListenForUDP failed with " + e.Message, LogLevels.ERROR);
            } finally {
                UdpListener.Close();
            }
        }

        protected char[] LineSplitters = new char[] { '\n', '\r' };
        protected bool LineSplitter(string text, out List<string> lines)
        {
            var split = text.Split(LineSplitters, StringSplitOptions.RemoveEmptyEntries);
            lines = split.ToList<string>();
            if (lines.Count>0)
                return true;
            return false;
        }

        protected char[] EquSplitters = new char[] { ' ', '=', '\n', '\t' };
        protected bool EquSplitter(string text, out List<string> tokens)
        {
            var split = text.Split(EquSplitters, StringSplitOptions.RemoveEmptyEntries);
            tokens = split.ToList<string>();
            if (tokens.Count==2)
                return true;
            return false;
        }

        protected void ConnectToOutput()
        {
            try {
                Client.Connect("127.0.0.1", TcpPort);
                var stream = Client.GetStream();
                var reader = new StreamReader(stream, Encoding.ASCII);
                bool isFirst = true;
                // First string is game profile
                while (Client.Connected && Running) {
                    if (stream.DataAvailable) {
                        var line = reader.ReadLine();
                        Logger.Log("[MAMENetOutput] "+ line, LogLevels.DEBUG);
                        // Process line/tokens
                        if (isFirst) {
                            if (EquSplitter(line, out var gametokens)) {
                                string game = gametokens[1];
                                this.GameProfile = game;
                            }
                            isFirst = false;
                        } else {
                            // Convert value to hex, as net output is in decimal
                            if (EquSplitter(line, out var paramtokens)) {
                                string param = paramtokens[0];
                                if (int.TryParse(paramtokens[1], out var result)) {
                                    this.ProcessMessage(line);
                                }
                            }
                        }
                    } else {
                        Thread.Sleep(32);
                    }
                }
            } catch (SocketException ex) {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused) {
                    // Server does not exist yet
                } else {
                    // Other kind of error
                    Logger.Log("[MAMENetOutput] got exception " + ex.Message, LogLevels.INFORMATIVE);
                }
            }
            detected = false;
        }
    }


}
