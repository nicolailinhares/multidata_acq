﻿using MultiData_Acq.Util;
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
        public ColetaWindow(ColetaInfo ci)
        {
            InitializeComponent();
            duration.Text = ci.Duration.ToString();
            pName.Text = ci.PatientName.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Aborted = false;
            Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            duration.IsEnabled = !duration.IsEnabled;
        }

    }
}
