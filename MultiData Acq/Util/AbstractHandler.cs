using MccDaq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace MultiData_Acq.Util
{
    abstract class AbstractHandler
    {
        public event RunWorkerCompletedEventHandler Finished;
        
        protected object FileLock;
        protected bool reading;
        public void CreateBackground(DataEventArgs e)
        {
            BackgroundWorker task = new BackgroundWorker();
            task.RunWorkerCompleted += Finished;
            task.DoWork += (s, args) =>
            {
                Handling(e.Data, e.Board);
            };
            task.RunWorkerAsync();
        }

        public abstract void Handling(ushort[] data, MccBoard board);
    }
}
