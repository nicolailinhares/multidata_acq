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

namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for BoardDetection.xaml
    /// </summary>
    public partial class BoardDetection : Window
    {
        private bool[] selected;
        public bool[] Selected
        {
            get { return selected;}
        }
        private bool aborted;
        public bool Aborted
        {
            get { return aborted; }
        }
        public BoardDetection(int qBoards)
        {
            InitializeComponent();
            selected = new bool[qBoards];
            aborted = true;
            AddBoxes(qBoards);
        }

        private void AddBoxes(int qBoards)
        {
            for (int i = 0; i < qBoards; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Auto);
                boxHolder.RowDefinitions.Add(rowDef);
                CheckBox box = new CheckBox();
                box.IsChecked = true;
                box.Margin = new Thickness(10);
                box.HorizontalAlignment = HorizontalAlignment.Left;
                box.VerticalAlignment = VerticalAlignment.Top;
                box.Content = string.Format("Board {0}", i);
                box.Click += CheckBox_Click;
                selected[i] = true;
                box.SetValue(Grid.RowProperty, i);
                boxHolder.Children.Add(box);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox) sender;
            string[] parts = cb.Content.ToString().Split(' ');
            int index = Int16.Parse(parts[1]);
            selected[index] = !selected[index];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            aborted = false;
            Close();
        }
    }
}
