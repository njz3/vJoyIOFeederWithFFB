using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class RawInputDB :
        ICloneable
    {
        public List<int> vJoyBtns;
        public bool IsInvertedLogic = false;
        public bool IsToggle = false;
        public bool IsAutoFire = false;
        public bool IsSequencedvJoy = false;
        /// <summary>
        /// 0: not part of  HShifter decoder
        /// 1: first switch
        /// 2: second switch
        /// 3: third switch
        /// </summary>
        public uint HShifterDecoderMap = 0;

        [NonSerialized]
        public int SequenceCurrentToSet = 0;

        public RawInputDB()
        {
            vJoyBtns = new List<int>(1);
        }
        public object Clone()
        {
            var obj = vJoyIOFeeder.Utils.Files.DeepCopy<RawInputDB>(this);
            return obj;
        }
    }
}
