using MultiData_Acq.Util;
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
using System.Windows.Shapes;

namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for ColetaWindow.xaml
    /// </summary>
    public partial class ColetaWindow : Window
    {
        private bool aborted = true;
        private List<BoardConfig> bcWindows;
        public List<BoardConfiguration> BoardConfigs
        {
            get
            {
                List<BoardConfiguration> bcs = new List<BoardConfiguration>();
                foreach (BoardConfig bcw in bcWindows)
                {
                    bcs.Add(bcw.BoardProperties);
                }
                return bcs;
            }
        }
        public bool Aborted
        {
            get { return aborted; }
            set { aborted = value; }
        }
        private ColetaInfo coletaInfo;
        public ColetaInfo ColetaInformation
        {
            get { 
                ColetaInfo ci = new ColetaInfo(Int32.Parse(duration.Text),pName.Text);
                if(isContinuos.IsChecked.Value)
                    ci.Duration = 0;
                return ci;
            }
            set { coletaInfo = value; }
        }
        public ColetaWindow(ColetaInfo ci, List<BoardConfiguration> bcs)
        {
            InitializeComponent();
            duration.Text = ci.Duration.ToString();
            pName.Text = ci.PatientName.ToString();
            bcWindows = new List<BoardConfig>();
            for (int i = 0; i < bcs.Count; i++)
            {
                BoardConfig bc = new BoardConfig(bcs[i]);
                bc.SetValue(Grid.RowProperty, i);
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(100, GridUnitType.Pixel);
                boardConfigGrid.RowDefinitions.Add(rowDef);
                boardConfigGrid.Children.Add(bc);
                bcWindows.Add(bc);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Aborted = false;
            Close();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender; 
            cb.IsEnabled = false;
            duration.IsEnabled = !duration.IsEnabled;
            cb.IsEnabled = true;
        }

    }
}
