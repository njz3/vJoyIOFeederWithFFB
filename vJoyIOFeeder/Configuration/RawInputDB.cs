﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

namespace BackForceFeeder.Configuration
{

    public enum ShifterDecoderMap : uint
    {
        No = 0,

        HShifterLeftRight,
        HShifterUp,
        HShifterDown,

        SequencialUp,
        SequencialDown,
    }

    [Serializable]
    public class RawInputDB :
        ICloneable
    {
        public List<int> MappedvJoyBtns;
        public bool IsInvertedLogic = false;
        public bool IsToggle = false;
        public bool IsAutoFire = false;
        public bool IsSequencedvJoy = false;
        public bool IsNeutralFirstBtn = false;

        /// <summary>
        /// 0: not part of shifter decoder
        /// </summary>
        public ShifterDecoderMap ShifterDecoder = ShifterDecoderMap.No;

        [NonSerialized]
        public int SequenceCurrentToSet = 0;

        public RawInputDB()
        {
            MappedvJoyBtns = new List<int>(1);
        }
        public object Clone()
        {
            var obj = BackForceFeeder.Utils.Files.DeepCopy<RawInputDB>(this);
            return obj;
        }
    }
}
