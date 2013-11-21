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
                boardProperties = new BoardConfiguration(Int32.Parse(lowChan.Text), Int32.Parse(qChan.Text), Int32.Parse(rate.Text), Int32.Parse(pRead.Text)); 
                return boardProperties; }
            set { boardProperties = value; }
        }
        
        public BoardConfig(BoardConfiguration bc)
        {
            InitializeComponent();
            rate.Text = bc.Rate.ToString();
            pRead.Text = bc.PointsRead.ToString();
            lowChan.Text = bc.LowChannel.ToString();
            qChan.Text = bc.QChanns.ToString();
            num.Text = bc.BoardName;
            BoardProperties = bc;
        }


    }
}
