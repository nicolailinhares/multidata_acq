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
    /// Interaction logic for ChnNameWindow.xaml
    /// </summary>
    public partial class ChnNameWindow : Window
    {
        public bool Aborted { get; set; }
        public ChnNameWindow()
        {
            InitializeComponent();
            Aborted = true;
        }
        public string ChannelName
        {
            get { return chnNameTxt.Text; }
            set { chnNameTxt.Text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Aborted = false;
            Close();
        }
    }
    
}
