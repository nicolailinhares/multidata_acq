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
using System.ComponentModel;
using System.Windows.Threading;

namespace MultiData_Acq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MccBoard> boards;
        private List<BoardConfiguration> boardConfigs;
        private List<DataHandler> dataHandlers;
        private bool started;
        private bool completed;
        private bool detected;
        private DispatcherTimer animTim;
        private ColetaInfo coletaInfo;
        private bool detecting;
        public MainWindow()
        {
            InitializeComponent();
            dataHandlers = new List<DataHandler>();
            started = false;
            completed = false;
            detected = false;
            animTim = new DispatcherTimer();
            animTim.Interval = TimeSpan.FromMilliseconds(200);
            animTim.Tick += Animate;
            animTim.Start();
            animTim.IsEnabled = false;
            detecting = false;
        }

        private void Animate(object sender, EventArgs e)
        {
            loader.Visibility = loader.Visibility == System.Windows.Visibility.Hidden ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;  
        }

        private TabItem createTabItem(MccBoard board, int num, BoardConfiguration bc)
        {
            TabItem item = new TabItem();
            item.Header = bc.BoardName;
            Grid grid = new Grid();
            ScrollViewer scrollView = new ScrollViewer();
            scrollView.Content = grid;
            item.Content = scrollView;
            DataHandler dataHand = new DataHandler(board, bc, Dispatcher.CurrentDispatcher);
            dataHand.Finished += ColetaFinished;
            dataHandlers.Add(dataHand);
            for (int i = 0; i < bc.QChanns; i++)
            { 
                ChannelControl chCont = new ChannelControl();
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(250, GridUnitType.Pixel);
                grid.RowDefinitions.Add(rowDef);
                chCont.SetValue(Grid.RowProperty, i);
                grid.Children.Add(chCont);
                dataHand.AddPlotModel(chCont.PlotModel);
            }
            return item;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(boards != null)
                boards.ForEach(b => b.StopBackground(MccDaq.FunctionType.AiFunction));
        }

        private void ColetaFinished(object sender, EventArgs e)
        {
            TrialCleaning("Acquisition completed");
        }

        private void SetupBoardSelection()
        {
            int numOfAdChans;
            boardConfigs = new List<BoardConfiguration>();
            BoardDetection bd = new BoardDetection(boards.Count);
            bd.Owner = this;
            bd.ShowDialog();
            if (!bd.Aborted)
            {
                for (int i = 0; i < boards.Count; i++)
                {
                    if (!bd.Selected[i])
                        boards.RemoveAt(i);
                    else
                    {
                        boards[i].BoardConfig.GetNumAdChans(out numOfAdChans);
                        if (numOfAdChans > 0)
                        {
                            BoardConfiguration bconf = new BoardConfiguration(0, 4, 2000);
                            bconf.MaxChannels = numOfAdChans;
                            boardConfigs.Add(bconf);
                        }else
                        {
                            boards.RemoveAt(i);
                        }
                    }
                }
                if (boards.Count > 0)
                {
                    detected = true;
                    stsMsg.Content = String.Format("{0} board(s) selected, click the second icon to create a trial", boards.Count);
                }
            }
            if(!detected)
                stsMsg.Content = "No board selected";
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = coletaInfo == null && !started && detected;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tabControl.Items.Clear();
            ColetaWindow cw = new ColetaWindow(new ColetaInfo(20, "Patient"), boardConfigs);
            cw.Owner = this;
            cw.ShowDialog();
            if (!cw.Aborted)
            {
                coletaInfo = cw.ColetaInformation;
                boardConfigs = cw.BoardConfigs;
                for (int i = 0; i < boards.Count; i++)
                {
                    tabControl.Items.Add(createTabItem(boards[i], i, boardConfigs[i]));
                }
                tabControl.SelectedItem = tabControl.Items[0];
                completed = false;
                stsMsg.Content = "Trial successfuly created, click the play icon to start acquisition";
            }
            
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = coletaInfo != null && !started;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dataHandlers.ForEach(ad => ad.Start(coletaInfo));
            started = true;
            stsMsg.Content = "Acquiring...";
            //animationToggle();
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = coletaInfo != null && started;   
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dataHandlers.ForEach(ad => ad.Stop());
            started = false;
            animationToggle();
            stsMsg.Content = "Paused, click the play icon to resume";
        }
        
        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = coletaInfo != null && started;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TrialCleaning("Trial aborted");
            boards.ForEach(b => b.StopBackground(FunctionType.AiFunction));
            dataHandlers.Clear();
            
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = completed;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            System.IO.File.WriteAllLines("Data.txt", new string[] { });
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

        private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !started && coletaInfo == null && !detecting;
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            detecting = true;
            stsMsg.Content = "Detecting boards, please wait...";
            animationToggle();
            BackgroundWorker task = new BackgroundWorker();
            task.RunWorkerCompleted += DetectionFinished;
            task.DoWork += (s, args) =>
            {
                DetectBoards();
            };
            task.RunWorkerAsync();
            //TODO : escrever no arquivo;
        }

        private void DetectBoards()
        {
            int maxNumBoards = GlobalConfig.NumBoards;
            int currentBoard = 0;
            boards = new List<MccBoard>();
            ErrorInfo uLStat;
            int boardType;
            for (int i = 0; i < maxNumBoards; i++)
            {
                MccBoard board = new MccBoard(currentBoard);
                uLStat = board.BoardConfig.GetBoardType(out boardType);
                if (boardType == 0) break;
                boards.Add(board);
                currentBoard++;
            }
        }

        private void DetectionFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            detecting = false;
            animationToggle();
            if (boards.Count > 0)
            {
                stsMsg.Content = String.Format("{0} board(s) detected",boards.Count);
                SetupBoardSelection();
            }
            else
            {
                stsMsg.Content = "Try to detect some board";
            }
            
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !started;
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void animationToggle()
        {
            animTim.IsEnabled = !animTim.IsEnabled;
            loader.Visibility = System.Windows.Visibility.Hidden;
        }

        private void TrialCleaning(string complement)
        {
            //animationToggle();
            coletaInfo = null;
            completed = true;
            started = false;
            stsMsg.Content = complement + ", click the third icon to save acquired data";
        }
    }
}
