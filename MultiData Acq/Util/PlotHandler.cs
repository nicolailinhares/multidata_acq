using MccDaq;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MultiData_Acq.Util
{
    class PlotHandler : AbstractHandler
    {
        private List<PlotModel> models;
        private Dispatcher uiDispatcher;
        private int count;
        private int qChans;
        private const int MAX = 10000;
        public PlotHandler(List<ChannelControl> ms, Dispatcher ui){
            models = new List<PlotModel>();
            ms.ForEach(m => models.Add(m.PlotModel));
            qChans = models.Count;
            FileLock = new object();
            uiDispatcher = ui;
        }
        public override void Handling(ushort[] data, MccBoard Board)
        {
            uiDispatcher.Invoke(() =>
			{
                float engUnits;
                reading = true;
                for (int i = 0; i < data.Length; i += qChans)
                {
                    for (int j = 0; j < qChans; j++)
                    {
                        Board.ToEngUnits(Range.Bip10Volts, data[i + j], out engUnits);
                        var lineSeries = models[j].Series[0] as LineSeries;
                        if (lineSeries != null)
                        {
                            lineSeries.Points.Add(new DataPoint(count, engUnits));
                        }
                        count++;
                    }
                }
                foreach (PlotModel model in models)
                {
                    var lineSeries = model.Series[0] as LineSeries;
                    if (lineSeries != null && count >= MAX)
                        lineSeries.Points.Clear();
                    model.InvalidatePlot(true);
                }
                if (count >= MAX) count = 0;

			});
                
        }

    }
}
