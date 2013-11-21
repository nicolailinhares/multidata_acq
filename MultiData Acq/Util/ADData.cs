using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MccDaq;
using System.Threading;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Series;
namespace MultiData_Acq.Util
{
    public delegate void FinishedEventHandler(object sender, EventArgs e);
    public class ADData
    {
        private const int MAX = 10000;
        private int lowChannel;
        private int maxParts;
        private int pontos;
        private string boardName;
        private List<string> lines;
        private int boardNum;
        private int parts;
        private List<ushort> dados;
        private Object thisLock = new Object();
        private int qChans;
        private int rate;
        private List<int> count;
        private ErrorInfo ULStat;
        private MccBoard Board;
        private int numPoints;
        public int NumPoints
        {
            get { return numPoints; }
            set { numPoints = value; }
        }
        private ushort[] adData;
        private IntPtr MemHandle;
        private List<PlotModel> models;
        private CallbackFunction mCb;
        public ADData(MccBoard board, BoardConfiguration bc, int bN, int lC, int qCh)
        {
            lowChannel = lC;
            qChans = qCh;
            NumPoints = bc.PointsRead;
            MemHandle = MccDaq.MccService.WinBufAllocEx(qChans*NumPoints);
            Board = board;
            rate = bc.Rate;
            adData = new ushort[qChans*NumPoints];
            dados = new List<ushort>();
            models = new List<PlotModel>();
            count = new List<int>();
            mCb = new CallbackFunction(this.CreateBackground);
            lines = new List<string>();
            parts = 0;
            pontos = 0;
            boardNum = bN;
            boardName = String.Format("Board {0}", boardNum);
        }

        public event FinishedEventHandler Finished;

        protected virtual void OnFinished(EventArgs e)
        {
            if (Finished != null)
                Finished(this, e);
        }

        public void Start(ColetaInfo coletaInfo)
        {
            maxParts = coletaInfo.Duration > 0 ? (coletaInfo.Duration * rate) : 0;
            string[] headerLines = { boardName, String.Format("Sampling rate: {0}", rate), String.Format("Channels: {0}", qChans), String.Format("Date and Time: {0}", DateTime.Now.ToString("d/MM/yyyy HH:mm:ss")), String.Format("Patient: {0}", coletaInfo.PatientName), "Dados" };
            System.IO.File.WriteAllLines(boardName+".txt", headerLines);
            ULStat = Board.EnableEvent(EventType.OnEndOfAiScan, EventParameter.Default, mCb, MemHandle);
            ULStat = Board.AInScan(lowChannel, lowChannel + qChans - 1, NumPoints, ref rate, Range.Bip10Volts, MemHandle, ScanOptions.Background);
            //consumer.Start();
        }

        public void AddPlotModel(PlotModel model)
        {
            models.Add(model);
            count.Add(0);
        }

        public void Stop()
        {
            ULStat = Board.DisableEvent(EventType.OnEndOfAiScan);
            //consumer.Stop();
            System.IO.File.AppendAllLines(boardName + ".txt", lines);
        }

        /*public void PushData(object sender, EventArgs e)
        {
            lock (thisLock)
            {
                int aux;
                float engUnits;
                string line = "";

                if (dados.Count >= NumPoints * qChans)
                {
                    ushort[] currDados = dados.ToArray();
                    dados.Clear();
                    
                }
                else
                {
                    Console.WriteLine(dados.Count.ToString());
                }
            }
        }*/

        public void SplitData(object sender, EventArgs e)
        {
            lock (thisLock)
            {
                int aux;
                float engUnits;
                string line = "";
                ULStat = Board.StopBackground(MccDaq.FunctionType.AiFunction);
                ULStat = MccDaq.MccService.WinBufToArray(MemHandle, adData, 0, qChans*NumPoints);
                pontos += NumPoints;
                for (int i = 0; i < adData.Length; i++)
                {
                    Board.ToEngUnits(Range.Bip10Volts, adData[i], out engUnits);
                    var lineSeries = models[i % (qChans)].Series[0] as LineSeries;
                    if (lineSeries != null)
                    {
                        line += String.Format("{0} ", adData[i]);
                        if ((i + 1) % qChans == 0)
                        {
                            lines.Add(line);
                            line = "";
                        }
                        if (count[i % (qChans)] >= MAX)
                        {
                            lineSeries.Points.Clear();
                            count[i % (qChans)] = 0;
                        }
                        aux = count[i % (qChans)]++;
                        if (aux % 10 == 0)
                            lineSeries.Points.Add(new DataPoint(aux, engUnits));
                    }
                }
                if (parts == 20)
                {
                    System.IO.File.AppendAllLines(String.Format(boardName + ".txt", boardNum), lines);
                    lines.Clear();
                    parts = 0;
                }
                parts++;
                foreach (PlotModel model in models)
                { model.RefreshPlot(true); }
                if (maxParts > 0 && pontos >= maxParts)
                    OnFinished(EventArgs.Empty);
                ULStat = Board.AInScan(lowChannel, lowChannel + qChans - 1, qChans * NumPoints, ref rate, Range.Bip10Volts, MemHandle, ScanOptions.Background);    
            }
            DispatcherTimer d = (DispatcherTimer)sender;
            d.Stop();
        }

        public void CreateBackground(int BoardNum, EventType EventType, int EventData, IntPtr pUserData)
        {
            DispatcherTimer task = new DispatcherTimer();
            task.Tick += SplitData;
            task.Start();
        }
    }
}
