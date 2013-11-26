using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiData_Acq.Util
{
    class FileHandler : AbstractHandler
    {
        private int qChans;
        private List<string> lines;
        private string boardName;
        private List<string> chnNames;
        //private int parts;
        public string PatientName { get; set; }
        public int Rate { get; set; }
        
        public FileHandler(int qC, string bN, int rate, string patientName, List<string> chnns)
        {
            chnNames = chnns;
            boardName = bN;
            Rate = rate;
            PatientName = patientName;
            qChans = qC;
            lines = new List<string>();
            //parts = 0;
            FileLock = new object();
            reading = false;
            string names = "";
            chnNames.ForEach(cn => names += (cn + ", "));
            string[] headerLines = { boardName, String.Format("Sampling rate: {0}", Rate), "Channels: " + names, String.Format("Date and Time: {0}", DateTime.Now.ToString("d/MM/yyyy HH:mm:ss")), String.Format("Patient: {0}", PatientName), "Dados" };
            System.IO.File.WriteAllLines(boardName + ".txt", headerLines);
        }

        public override void Handling(ushort[] data, MccBoard board)
        {
            lock (FileLock)
            {
                if (reading)
                    Monitor.Wait(FileLock);
                string line = "";
                reading = true;
                for (int i = 0; i < data.Length; i++)
                {
                    line += String.Format("{0}{1}", data[i], (i + 1) % qChans == 0 ? "" : " ");
                    if ((i + 1) % qChans == 0)
                    {
                        lines.Add(line);
                        line = "";
                    }
                }
                //if (parts > 20)
               // {
                System.IO.File.AppendAllLines(boardName + ".txt", lines);
                lines.Clear();
                   // parts = 0;
                //}
                //parts++;
                reading = false;
                Monitor.Pulse(FileLock);
            }
        }

        public void SetChanName(string chn, int ind)
        {
            chnNames[ind] = chn;
        }
    }
}
