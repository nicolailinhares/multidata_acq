using System;
using System.ComponentModel;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using MultiData_Acq.Util;
using System.Threading;

namespace MultiData_Acq.ViewModels
{
    public class ChannelModel: INotifyPropertyChanged
    {
        private const int MAX = 60;
        private DateTime dt;
        private int chann;
        private int count;
        public int Chann
        {
            get { return chann; }
            set { chann = value; }
        }
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; NotifyPropertyChanged("PlotModel"); }
        }  
        public ChannelModel(int chann, ADData data)
        {
            PlotModel = new PlotModel();
            dt = new DateTime();
            SetUpModel();
            count = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
        
        private void SetUpModel()
        {
            //var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Time", "mm:ss:fff") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, IntervalLength = 80 };
            var dateAxis = new LinearAxis(AxisPosition.Bottom, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);

            var valuesAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Volts", Maximum = 10.0, Minimum = -10.0 };
            PlotModel.Axes.Add(valuesAxis);
            var lineSerie = new LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Title = "Data",
                Smooth = false
            };
            PlotModel.Series.Add(lineSerie);
        }

    }
}
