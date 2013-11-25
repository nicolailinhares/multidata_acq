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
using System.ComponentModel;
namespace MultiData_Acq.Util
{
    public delegate void ScannedEventHandler(object sender, DataEventArgs e);
    public class ADData
    {
        private int lowChannel;
        private Object thisLock = new Object();
        private int qChans;
        private int rate;
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
        private CallbackFunction mCb;
        public ADData(MccBoard board, BoardConfiguration bc)
        {
            lowChannel = bc.LowChannel;
            qChans = bc.QChanns;
            NumPoints = 186;//bc.PointsRead;
            MemHandle = MccDaq.MccService.WinBufAllocEx(10*qChans*NumPoints);
            Board = board;
            rate = bc.Rate;
            adData = new ushort[qChans*NumPoints];
            mCb = new CallbackFunction(this.CreateBackground);
        }

        public void Start()
        {
            ULStat = Board.EnableEvent(EventType.OnDataAvailable, qChans*NumPoints, mCb, MemHandle);
            ULStat = Board.AInScan(lowChannel, lowChannel + qChans - 1, qChans*NumPoints, ref rate, Range.Bip10Volts, MemHandle, ScanOptions.Background | ScanOptions.Continuous);
        }

        public void Stop()
        {
            ULStat = Board.DisableEvent(EventType.OnDataAvailable);
            ULStat = Board.StopBackground(FunctionType.AiFunction);
            //System.IO.File.AppendAllLines(boardName + ".txt", lines);
        }

        public event ScannedEventHandler Scanned;

        protected virtual void OnScanned(DataEventArgs e)
        {
            if (Scanned != null)
                Scanned(this, e);
        }

        public void SplitData(object sender, DoWorkEventArgs e)
        {
            lock (thisLock)
            {
                ULStat = MccDaq.MccService.WinBufToArray(MemHandle, adData, 0, qChans * NumPoints);
                DataEventArgs args = new DataEventArgs();
                args.Data = adData;
                args.Board = Board;
                OnScanned(args);
            }
            
        }

        public void CreateBackground(int BoardNum, EventType EventType, int EventData, IntPtr pUserData)
        {
            BackgroundWorker task = new BackgroundWorker();
            task.DoWork += SplitData;
            task.RunWorkerAsync();
        }
    }
}
