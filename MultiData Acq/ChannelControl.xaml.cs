using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MccDaq;
using System.Threading;
using MultiData_Acq.Util;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;
namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for ChannelControl.xaml
    /// </summary>
    public partial class ChannelControl : UserControl, INotifyPropertyChanged
    {
        private string chnName;
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; }
        }
        public string ChnName
        {
            get { return chnName; }
            set { chnName = value; Notify("ChnName"); }
        }
        public ChannelControl(string cn)
        {
            
            PlotModel = new PlotModel();
            SetUpModel();
            DataContext = this;
            InitializeComponent();
            ChnName = cn;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string nome)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nome));
        }
        private void SetUpModel()
        {
            //var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Time", "mm:ss:fff") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, IntervalLength = 80 };
            var dateAxis = new LinearAxis(AxisPosition.Bottom, 0) { MajorGridlineStyle = LineStyle.Solid, IntervalLength = 240, Maximum = 10000, Minimum = 0};
            PlotModel.Axes.Add(dateAxis);

            var valuesAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, Title = "Volts", Maximum = 10.0, Minimum = -10.0 };
            PlotModel.Axes.Add(valuesAxis);
            var lineSerie = new LineSeries
            {
                StrokeThickness = 1,
                CanTrackerInterpolatePoints = false,
                Title = "Data",
                Smooth = false
            };
            PlotModel.Series.Add(lineSerie);
            
        }

        private void Plot1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChnNameWindow chnNameWindow = new ChnNameWindow();
            chnNameWindow.Aborted = true;
            chnNameWindow.ChannelName = chnName;
            //chnNameWindow.Owner = (Window) this.Parent;
            chnNameWindow.ShowDialog();
            if (!chnNameWindow.Aborted)
                ChnName = chnNameWindow.ChannelName;
        }

        public void DisableClick()
        {
            Plot1.MouseDoubleClick -= Plot1_MouseDoubleClick;
        }
    }
}
