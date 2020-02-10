using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Outputs
{
    /// <summary>
    /// Emulator Output agent
    /// </summary>
    public abstract class IOutput
    {
        public int DriveValue { get; protected set; }
        public int LampsValue { get; protected set; }
        public string GameProfile { get; protected set; }

        public abstract void Start();
        public abstract void Stop();

    }
}
