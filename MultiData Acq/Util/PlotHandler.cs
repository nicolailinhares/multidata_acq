using MccDaq;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiData_Acq.Util
{
    class PlotHandler : AbstractHandler
    {
        private List<PlotModel> models;
        private int count;
        private int qChans;
        private const int MAX = 10000;
        public PlotHandler(List<PlotModel> ms){
            models = ms;
            qChans = models.Count;
        }
        public override void Handling(ushort[] data, MccBoard Board)
        {
            float engUnits;
            
            for (int i = 0; i < data.Length; i+=qChans)
            {
                for (int j = 0; j < qChans; j++)
                {
                    Board.ToEngUnits(Range.Bip10Volts, data[i+j], out engUnits);
                    var lineSeries = models[j].Series[0] as LineSeries;
                    if (lineSeries != null)
                    {
                        lineSeries.Points.Add(new DataPoint(count , engUnits));
                    }
                    count++;
                }             
            }
            foreach (PlotModel model in models)
            {
                var lineSeries = model.Series[0] as LineSeries;
                if (lineSeries != null && count >= MAX)
                    lineSeries.Points.Clear();
                model.RefreshPlot(true); 
            }
            if (count  >= MAX) count = 0;
        }

    }
}
