﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackForceFeeder.Outputs
{
    /// <summary>
    /// Emulator Output agent
    /// </summary>
    public abstract class OutputsAgent
    {
        
        /// <summary>
        /// Raw drive value. Unused for now.
        /// Maybe in the future, for only supported platforms, it would be
        /// possible to use it to have the "original" force feedback from
        /// each game.
        /// </summary>
        public Int32 DriveValue { get; protected set; }

        /// <summary>
        /// Bit 0..1 are unused (coins lamps)
        /// Bit 2: Lamp Start (yellow)
        /// Bit 3: Lamp View 1 (Red)
        /// Bit 4: Lamp View 2 (Blue)
        /// Bit 5: Lamp View 3 (Yellow)
        /// Bit 6: Lamp View 4 (Green)
        /// Bit 7: Lamp Leader (topper)
        /// 
        /// Bits 8-15 will be overwritten for the driveboard (Mega2560)
        /// but they can still be used in case no driveboard is connected
        /// </summary>
        public Int32 LampsValue { get; protected set; }

        /// <summary>
        /// Known profiles:
        /// - outrun
        /// - scud/lemans/daytona2
        /// </summary>
        public string GameProfile { get; protected set; }

        public virtual void ClearAll()
        {
            DriveValue = 0;
            LampsValue = 0;
        }

        public abstract void Start();
        public abstract void Stop();

    }
}
