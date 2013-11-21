using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace MultiData_Acq.Util
{
    abstract class AbstractHandler
    {
        public void CreateBackground(DataEventArgs e)
        {
            DispatcherTimer task = new DispatcherTimer();
            task.Tick += (s, args) =>
            {
                Handling(e.Data, e.Board);
                DispatcherTimer d = (DispatcherTimer)s;
                d.Stop();
            };
            task.Start();
        }

        public abstract void Handling(ushort[] data, MccBoard board);
    }
}
