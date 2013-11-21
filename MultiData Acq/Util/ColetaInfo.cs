using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MccDaq;
namespace MultiData_Acq.Util
{
    public class ColetaInfo
    {
        private int duration;
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        private string patientName;
        public string PatientName
        {
            get { return patientName; }
            set { patientName = value; }
        }
        public ColetaInfo(int dur, string pName)
        {
            patientName = pName;
            duration = dur;
        }
    }
}
