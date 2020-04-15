using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeeder.Outputs
{
    /// <summary>
    /// MAME network output agent.
    /// Use local TCP connection on port 8000
    /// </summary>
    public class MAMEOutputNetAgent : MAMEOutputAgent
    {
        TcpClient Client;

        public MAMEOutputNetAgent() :
            base()
        {
        }

        protected override void ManagerThreadMethod()
        {
            Client = new TcpClient();
            while (Running) {
                Client.Connect("127.0.0.1", 8000);
                var stream = Client.GetStream();
                var reader = new StreamReader(stream, Encoding.ASCII);
                try {
                    while (Client.Connected) {
                        if (stream.DataAvailable) {
                            var line = reader.ReadLine();

                            // Process line/tokens
                            this.ProcessMessage(line);
                        } else {
                            Thread.Sleep(32);
                        }
                    }
                } catch (Exception ex) {
                    Logger.Log("[MAMENetOutput] got exception " + ex.Message, LogLevels.INFORMATIVE);
                }
            }
            Logger.Log("[MAMENetOutput] TCP connection terminated", LogLevels.INFORMATIVE);
        }
    }


}
