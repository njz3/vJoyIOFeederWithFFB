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
    public abstract class AOutput
    {
        /// <summary>
        /// Unused for now
        /// </summary>
        public int DriveValue { get; protected set; }
        /// <summary>
        /// Bit 0..1 are unused (coins lamps)
        /// Bit 2: Lamp Start
        /// Bit 3: Lamp View 1
        /// Bit 4: Lamp View 1
        /// Bit 5: Lamp View 1
        /// Bit 6: Lamp View 1
        /// Bit 7: Lamp Leader
        /// </summary>
        public int LampsValue { get; protected set; }
        /// <summary>
        /// Known profiles:
        /// - outrun
        /// - scud/lemans/daytona2
        /// </summary>
        public string GameProfile { get; protected set; }

        public abstract void Start();
        public abstract void Stop();

    }
}
