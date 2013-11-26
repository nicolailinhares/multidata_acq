using MccDaq;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MultiData_Acq.Util
{
    public delegate void FinishedEventHandler(object sender, EventArgs e);
    public delegate void DataProcessingHandler(DataEventArgs e);
    class DataHandler
    {
        private int maxPontos;
        private int pontos;
        private ADData dataCont;
        private BoardConfiguration boardConfig;
        private FileHandler fileHand;
        private PlotHandler plotHand;
        private List<ChannelControl> models;
        private Dispatcher uiDispatcher;
        public DataHandler(MccBoard board, BoardConfiguration bc, Dispatcher ui)
        {
            dataCont = new ADData(board, bc);
            boardConfig = bc;
            models = new List<ChannelControl>();
            uiDispatcher = ui;
        }

        public void Start(ColetaInfo ci)
        {
            List<string> names = new List<string>();
            models.ForEach(delegate(ChannelControl cc){
                names.Add(cc.ChnName);
                cc.DisableClick();
            });
            maxPontos = ci.Duration * boardConfig.Rate * boardConfig.QChanns;
            fileHand = new FileHandler(boardConfig.QChanns, boardConfig.BoardName, boardConfig.Rate, ci.PatientName, names);
            plotHand = new PlotHandler(models, uiDispatcher);
            Processing += fileHand.CreateBackground;
            Processing += plotHand.CreateBackground;
            dataCont.Scanned += DispatchData;
            dataCont.Start();
        }

        public void Stop()
        {
            dataCont.Stop();
            Processing -= fileHand.CreateBackground;
            Processing -= plotHand.CreateBackground;
            dataCont.Scanned -= DispatchData;
        }

        public event FinishedEventHandler Finished;

        protected virtual void OnFinished(EventArgs e)
        {
            if (Finished != null)
                Finished(this, e);
        }

        public event DataProcessingHandler Processing;

        private void DispatchData(object sender, DataEventArgs e)
        {
            if (maxPontos > 0 && pontos >= maxPontos)
            {
                Stop();
                uiDispatcher.Invoke(() =>
                {
                    OnFinished(EventArgs.Empty);
                });
            }
            else
            {
                Processing.Invoke(e);
                pontos += e.Data.Length;
            }         
        }
        public void AddPlotModel(ChannelControl pm)
        {
            models.Add(pm);
        }
    }
}
