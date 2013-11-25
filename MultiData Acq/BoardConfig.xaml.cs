using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiData_Acq.Util;
namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for BoardConfig.xaml
    /// </summary>
    public partial class BoardConfig : UserControl
    {
        private BoardConfiguration boardProperties;
        public BoardConfiguration BoardProperties
        {
            get
            {
                boardProperties.LowChannel = (int) chnSlider.RangeStartSelected - 1;
                boardProperties.QChanns = (int) (chnSlider.RangeStopSelected - chnSlider.RangeStartSelected + 1);
                boardProperties.Rate = Int32.Parse(rate.Text);
                return boardProperties;
            }
            set { boardProperties = value; }

        }

        public BoardConfig(BoardConfiguration bc)
        {
            InitializeComponent();
            chnSlider.RangeStart = 1;
            chnSlider.RangeStop = bc.MaxChannels;
            rate.Text = bc.Rate.ToString();
            chnSlider.RangeStartSelected = bc.LowChannel + 1;
            chnSlider.RangeStopSelected = bc.LowChannel + bc.QChanns; 
            num.Text = bc.BoardName;
            BoardProperties = bc;
            lc.Content = chnSlider.RangeStartSelected.ToString();
            hc.Content = chnSlider.RangeStopSelected.ToString();
        }

        private void RangeSlider_RangeSelectionChanged(object sender, AC.AvalonControlsLibrary.Controls.RangeSelectionChangedEventArgs e)
        {
            if (e.NewRangeStart > e.NewRangeStop)
            {
                chnSlider.RangeStartSelected = e.NewRangeStop;
                chnSlider.RangeStopSelected = e.NewRangeStart;
            }

            if (lc != null)
            {
                lc.Content = chnSlider.RangeStartSelected.ToString();
                hc.Content = chnSlider.RangeStopSelected.ToString();
            }
        }
    }
}
