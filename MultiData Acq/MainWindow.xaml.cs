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
using MccDaq;
using MultiData_Acq.Util;
using System.Threading;

namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MccBoard> boards;
        private List<ADData> adDatas;
        private bool started;
        private Window configWindow;
        private ColetaInfo coletaInfo;
        public MainWindow()
        {
            InitializeComponent();
            boards = new List<MccBoard>();
            adDatas = new List<ADData>();
            started = false;
            coletaInfo = new ColetaInfo(20, "Patient");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private TabItem createTabItem(MccBoard board, int num, BoardConfiguration bc)
        {
            TabItem item = new TabItem();
            int numOfAdChan;
            int lowOne;
            board.BoardConfig.GetNumAdChans(out numOfAdChan);
            lowOne = bc.LowChannel + bc.QChanns > numOfAdChan ? 0 : bc.LowChannel;
            numOfAdChan = bc.QChanns > numOfAdChan ? numOfAdChan : bc.QChanns;
            item.Header = String.Format("Board {0}", num);
            Grid grid = new Grid();
            ScrollViewer scrollView = new ScrollViewer();
            scrollView.Content = grid;
            item.Content = scrollView;
            ADData dataCont = new ADData(board, bc, num, lowOne, numOfAdChan);
            dataCont.Finished += ColetaFinished;
            adDatas.Add(dataCont);

            for (int i = 0; i < numOfAdChan; i++)
            { 
                ChannelControl chCont = new ChannelControl();
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(250, GridUnitType.Pixel);
                grid.RowDefinitions.Add(rowDef);
                chCont.SetValue(Grid.RowProperty, i);
                grid.Children.Add(chCont);
                dataCont.AddPlotModel(chCont.PlotModel);
            }
            return item;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            boards.ForEach(b => b.StopBackground(MccDaq.FunctionType.AiFunction));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            if (started)
            {
                adDatas.ForEach(ad => ad.Stop());
                started = false;
                bt.Content = "Start";
            }
            else
            {
                adDatas.ForEach(ad => ad.Start(coletaInfo));
                started = true;
                bt.Content = "Stop";
            }
        }

        private void ColetaFinished(object sender, EventArgs e)
        {
            ADData dataCont = (ADData)sender;
            dataCont.Stop();
            started = false;
            controlBt.Content = "Start";
        }

        private void MenuDetect_Click(object sender, RoutedEventArgs e)
        {
            int maxNumBoards = GlobalConfig.NumBoards;
            int currentBoard = 0;
            List<MccBoard> temp = new List<MccBoard>();
            ErrorInfo uLStat;
            int boardType;
            for (int i = 0; i < maxNumBoards; i++)
            {
                MccBoard board = new MccBoard(currentBoard);
                temp.Add(board);
                uLStat = board.BoardConfig.GetBoardType(out boardType);
                if (boardType == 0) break;
                currentBoard++;
            }
            if (temp.Count > 0)
            {
                SetupBoardSelection(temp);
                SetupBoardConfiguration();
            }
        }

        private void SetupBoardSelection(List<MccBoard> temp)
        {
            BoardDetection bd = new BoardDetection(temp.Count);
            bd.Owner = this;
            bd.ShowDialog();
            if (!bd.Aborted)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (bd.Selected[i])
                    {
                        boards.Add(temp.ElementAt(i));
                    }
                }
            }
        }

        private void SetupBoardConfiguration()
        {
            if (boards.Count == 0)
                return;
            configWindow = new Window();
            configWindow.SizeToContent = SizeToContent.WidthAndHeight;
            Grid grid = new Grid();
            List<BoardConfig> configs = new List<BoardConfig>();
            for (int i = 0; i < boards.Count; i++)
            {
                BoardConfig bc = new BoardConfig(new BoardConfiguration(0, 4, 2000, 80), i);
                bc.SetValue(Grid.RowProperty, i);
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Auto);
                grid.RowDefinitions.Add(rowDef);
                grid.Children.Add(bc);
                configs.Add(bc);
            }
            Button bt = new Button();
            bt.Click += configOk_Click;
            bt.Content = "Ok";
            bt.Margin = new Thickness(10);
            RowDefinition def = new RowDefinition();
            def.Height = new GridLength(1, GridUnitType.Auto);
            bt.SetValue(Grid.RowProperty, boards.Count);
            bt.HorizontalAlignment = HorizontalAlignment.Right;
            grid.RowDefinitions.Add(def);
            grid.Children.Add(bt);
            configWindow.Content = grid;
            configWindow.Owner = this;
            configWindow.ShowDialog();
            for (int i = 0; i < boards.Count; i++)
            {
                tabControl.Items.Add(createTabItem(boards[i], i, configs[i].BoardProperties));
            }
            tabControl.SelectedItem = tabControl.Items[0];
        }

        void configOk_Click(object sender, RoutedEventArgs e)
        {
            configWindow.Close();
        }

        private void MenuConfColeta_Click(object sender, RoutedEventArgs e)
        {
            ColetaWindow cw = new ColetaWindow(coletaInfo);
            cw.Owner = this;
            cw.ShowDialog();
            if (!cw.Aborted)
            {
                coletaInfo = cw.ColetaInformation;
            }
        }

        private void MenuSaveColeta_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            System.IO.File.WriteAllLines("Data.txt", new string [] {});
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                for (int i = 0; i < boards.Count; i++)
                {
                    System.IO.File.AppendAllLines("Data.txt", System.IO.File.ReadAllLines(String.Format("Board {0}.txt", i)));
                }
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                System.IO.File.Move("Data.txt", filename);
            }
        }

    }
}
