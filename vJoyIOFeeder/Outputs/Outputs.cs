using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Outputs
{
    /// <summary>
    /// Emulator Output agent
    /// </summary>
    public abstract class Outputs
    {
        /// <summary>
        /// Raw drive value. Unused for now.
        /// Maybe in the future, for only supported platforms, it would be
        /// possible to use it to have the "original" force feedback from
        /// each game.
        /// </summary>
        public Int32 DriveValue { get; protected set; }

        /// <summary>
        /// Bit 0..1 are unused (coins lamps) -> replace by fwd/rev directions
        /// Bit 2: Lamp Start
        /// Bit 3: Lamp View 1
        /// Bit 4: Lamp View 1
        /// Bit 5: Lamp View 1
        /// Bit 6: Lamp View 1
        /// Bit 7: Lamp Leader
        /// </summary>
        public Int32 LampsValue { get; protected set; }

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
