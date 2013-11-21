using MccDaq;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<PlotModel> models;
        public DataHandler(MccBoard board, BoardConfiguration bc)
        {
            dataCont = new ADData(board, bc);
            boardConfig = bc;
            models = new List<PlotModel>();
        }

        public void Start(ColetaInfo ci)
        {
            maxPontos = ci.Duration * boardConfig.Rate * boardConfig.QChanns;
            fileHand = new FileHandler(boardConfig.QChanns, boardConfig.BoardName, boardConfig.Rate, ci.PatientName);
            plotHand = new PlotHandler(models);
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
            Processing.Invoke(e);
            pontos += e.Data.Length;
            if (maxPontos > 0 && pontos >= maxPontos)
                OnFinished(EventArgs.Empty);
        }

        public void AddPlotModel(PlotModel pm)
        {
            models.Add(pm);
        }
    }
}
