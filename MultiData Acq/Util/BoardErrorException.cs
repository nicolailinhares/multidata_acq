using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiData_Acq.Util
{
    class BoardErrorException : Exception
    {
        public object Sender { get; set; }
        public string ErrMessage { get; set; }
        public BoardErrorException(object sender, string msg)
        {
            Sender = sender;
            ErrMessage = msg;
        }

        public static void TestException(MccDaq.ErrorInfo stat, object sender, string msg)
        {
            if (stat.Value != ErrorInfo.ErrorCode.NoErrors)
                throw new BoardErrorException(sender, msg);
        }
    }
}
