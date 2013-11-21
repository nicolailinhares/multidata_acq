using MccDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiData_Acq.Util
{
    class FileHandler : AbstractHandler
    {
        private int qChans;
        private List<string> lines;
        private string boardName;
        private int parts;
        public FileHandler(int qC, string bN, int rate, string patientName)
        {
            boardName = bN;
            qChans = qC;
            lines = new List<string>();
            string[] headerLines = { boardName, String.Format("Sampling rate: {0}", rate), String.Format("Channels: {0}", qChans), String.Format("Date and Time: {0}", DateTime.Now.ToString("d/MM/yyyy HH:mm:ss")), String.Format("Patient: {0}", patientName), "Dados" };
            System.IO.File.WriteAllLines(boardName + ".txt", headerLines);
            parts = 0;
        }
        public override void Handling(ushort[] data, MccBoard board)
        {
            string line = "";
            for (int i = 0; i < data.Length; i++)
            {
                line += String.Format("{0}{1}", data[i],(i+1)%qChans == 0 ? "" : " ");
                if ((i + 1) % qChans == 0)
                {
                    lines.Add(line);
                    line = "";
                }
            }
            //if (parts > 20)
            //{
            System.IO.File.AppendAllLines(boardName + ".txt", lines);
            lines.Clear();
                //parts = 0;
            //}
            //parts++;
        }
    }
}
