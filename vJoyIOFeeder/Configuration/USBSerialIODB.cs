using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Configuration
{
    [Serializable]
    public class USBSerialIODB
    {
        public string COMPOrt;
        public int Baudrate;
        public int IsFFBCompatible;
    }
}
