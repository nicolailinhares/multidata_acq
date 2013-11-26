using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiData_Acq.Util
{
    public class DataEventArgs : EventArgs
    {
        public ushort[] Data {get; set;}
        public MccBoard Board { get; set; }

    }
}
