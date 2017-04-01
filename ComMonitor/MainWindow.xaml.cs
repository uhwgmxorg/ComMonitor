using ComMonitor.Dialogs;
using ComMonitor.LocalTools;
using ComMonitor.MDIWindows;
using ComMonitor.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using WPF.MDI;

namespace ComMonitor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private NLog.Logger _logger;
        private Random _random = new Random();

        public byte[] FocusMessage { get; set; }


        public RelayCommand TideledCommand { get; private set; }
        public RelayCommand CascadeCommand { get; private set; }
        public RelayCommand CloseAllCommand { get; private set; }
        public RelayCommand ChangeLogCommand { get; private set; }


        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand NewConnectionsWindowCommand { get; private set; }
        public RelayCommand LoadConnectionsCommand { get; private set; }
        public RelayCommand SaveConnectionsCommand { get; private set; }
        public RelayCommand OpenMessageFileCommand { get; private set; }
        public RelayCommand SaveMessageFileCommand { get; private set; }
        public RelayCommand SaveMessageFileAsCommand { get; private set; }
        public RelayCommand AddNewMessageCommand { get; private set; }
        public RelayCommand EditMessageCommand { get; private set; }
        public RelayCommand AddMessageCommand { get; private set; }
        public RelayCommand EditAndReplaceMessageCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }
        public RelayCommand DeleteAllCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();
            DataContext = this;

            ExitCommand = new RelayCommand(ExitCommandCF, CanExitCommand);
            TideledCommand = new RelayCommand(TideledCommandCF, CanTideledCommand);
            CascadeCommand = new RelayCommand(CascadeCommandCF, CanCascadeCommand);
            CloseAllCommand = new RelayCommand(CloseAllCommandCF, CanCloseAllCommand);
            ChangeLogCommand = new RelayCommand(ChangeLogCommandCF, CanChangeLogCommand);
            ExitCommand = new RelayCommand(ExitCommandCF, CanExitCommand);
            NewConnectionsWindowCommand = new RelayCommand(NewConnectionsWindowCommandCF, CanNewConnectionsWindowCommand);
            LoadConnectionsCommand = new RelayCommand(LoadConnectionsCommandCF, CanLoadConnectionsCommand);
            SaveConnectionsCommand = new RelayCommand(SaveConnectionsCommandCF, CanSaveConnectionsCommand);
            OpenMessageFileCommand = new RelayCommand(OpenMessageFileCommandCF, CanOpenMessageFileCommand);
            SaveMessageFileCommand = new RelayCommand(SaveMessageFileCommandCF, CanSaveMessageFileCommand);
            SaveMessageFileAsCommand = new RelayCommand(SaveMessageFileAsCommandCF, CanSaveMessageFileAsCommand);
            AddNewMessageCommand = new RelayCommand(AddNewMessageCommandCF, CanAddNewMessageCommand);
            EditMessageCommand = new RelayCommand(EditMessageCommandCF, CanEditMessageCommand);
            AddMessageCommand = new RelayCommand(AddMessageCommandCF, CanAddMessageCommand);
            EditAndReplaceMessageCommand = new RelayCommand(EditAndReplaceMessageCommandCF, CanEditAndReplaceMessageCommand);
            SendCommand = new RelayCommand(SendCommandCF, CanSendCommand);
            DeleteAllCommand = new RelayCommand(DeleteAllCommandCF, CanDeleteAllCommand);
            AboutCommand = new RelayCommand(AboutCommandCF, CanAboutCommand);

            FocusMessage = null;
        }

        /******************************/
        /*     Command Functions      */
        /******************************/
        #region Command Functions

        #region MenueBar
        /// <summary>
        /// TideledCommandCF
        /// </summary>
        private void TideledCommandCF()
        {
            double whh = SystemParameters.WindowCaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight;
            double wf = SystemParameters.ResizeFrameVerticalBorderWidth;
            double sy = ActualHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight - 2 * MainStatusBar.Height;
            double sx = ActualWidth;
            double anzW = MainMdiContainer.Children.Count;

            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                MainMdiContainer.Children[i].Width = sx - 4 * wf;
                MainMdiContainer.Children[i].Height = sy / anzW;
                MainMdiContainer.Children[i].Position = new Point(0, sy / anzW * i);
            }
        }

        /// <summary>
        /// CanTideledCommand
        /// </summary>
        /// <returns></returns>
        private bool CanTideledCommand()
        {
            return MainMdiContainer.Children.Count > 0;
        }

        /// <summary>
        /// CascadeCommandCF
        /// </summary>
        private void CascadeCommandCF()
        {
            double whh = SystemParameters.WindowCaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight;
            double sy = ActualHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight;
            double sx = ActualWidth;
            int anzW = MainMdiContainer.Children.Count;

            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                MainMdiContainer.Children[i].Width = sx * 0.6;
                MainMdiContainer.Children[i].Height = sy * 0.6;
                MainMdiContainer.Children[i].Position = new Point(whh * i, whh * i);
            }
        }

        /// <summary>
        /// CanCascadeCommand
        /// </summary>
        /// <returns></returns>
        private bool CanCascadeCommand()
        {
            return MainMdiContainer.Children.Count > 0;
        }

        /// <summary>
        /// CloseAllCommandCF
        /// </summary>
        private void CloseAllCommandCF()
        {
            for (int i = MainMdiContainer.Children.Count - 1; i >= 0; i--)
                MainMdiContainer.Children[i].Close();
        }

        /// <summary>
        /// CanCloseAllCommand
        /// </summary>
        /// <returns></returns>
        private bool CanCloseAllCommand()
        {
            return MainMdiContainer.Children.Count > 0;
        }

        /// <summary>
        /// ChangeLogCommandCF
        /// </summary>
        private void ChangeLogCommandCF()
        {
            string File = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ChangeLog.txt";
            ChangeLogUtilityDll.ChangeLogTxtToolWindow ChangeLogTxtToolWindowObj = new ChangeLogUtilityDll.ChangeLogTxtToolWindow(this);
            ChangeLogTxtToolWindowObj.ShowChangeLogWindow(File);
        }

        /// <summary>
        /// CanChangeLogCommand
        /// </summary>
        /// <returns></returns>
        private bool CanChangeLogCommand()
        {
            return true;
        }

        /// <summary>
        /// ExitCommandCF
        /// </summary>
        private void ExitCommandCF()
        {
            _logger.Info("Closing ComMonitor");
            Environment.Exit(0);
        }

        /// <summary>
        /// CanWelcomeCommand
        /// </summary>
        /// <returns></returns>
        private bool CanExitCommand()
        {
            return true;
        }
        #endregion

        #region ToolBar
        /// <summary>
        /// NewConnectionsWindowCommandCF
        /// </summary>
        private void NewConnectionsWindowCommandCF()
        {
            double AcParentWindoHeight = ActualHeight;
            double AcParentWindoWidth = ActualWidth;

            ConfigNewConnection ConfigNewConnectionDlg = new ConfigNewConnection();
            ConfigNewConnectionDlg.Owner = Window.GetWindow(this);
            var res = ConfigNewConnectionDlg.ShowDialog();
            if (!res.Value)
                return;

            MdiChild MdiChild = new MdiChild()
            {
                Title = String.Format("New Connection ( )"),
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild(ConfigNewConnectionDlg.ConnectionObj,this)
            };
            ((UserControlTCPMDIChild)MdiChild.Content).TheMdiChild = MdiChild;
            MainMdiContainer.Children.Add(MdiChild);
        }

        /// <summary>
        /// CanNewConnectionsWindowCommand
        /// </summary>
        /// <returns></returns>
        private bool CanNewConnectionsWindowCommand()
        {
            return true;
        }

        /// <summary>
        /// LoadConnectionsCommandCF
        /// </summary>
        private void LoadConnectionsCommandCF()
        {
            double AcParentWindoHeight = ActualHeight;
            double AcParentWindoWidth = ActualWidth;

            string configFileName = LST.OpenFileDialog("Cmc Datein (*.cmc)|*.cmc;|Alle Dateien (*.*)|*.*\"");
            if (String.IsNullOrEmpty(configFileName))
                return;

            Connection newConnection = Connection.Load(configFileName);
            _logger.Info(String.Format("Load Connection File {0}", configFileName));

            MdiChild MdiChild = new MdiChild()
            {
                Title = String.Format("{0} ( )", Path.GetFileName(configFileName)),
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild(newConnection,this)
            };
            ((UserControlTCPMDIChild)MdiChild.Content).TheMdiChild = MdiChild;
            MainMdiContainer.Children.Add(MdiChild);
        }

        /// <summary>
        /// CanLoadConnectionsCommand
        /// </summary>
        /// <returns></returns>
        private bool CanLoadConnectionsCommand()
        {
            return true;
        }

        /// <summary>
        /// SaveConnectionsCommandCF
        /// </summary>
        private void SaveConnectionsCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null)
            {
                Console.Beep();
                _logger.Info("Nothing to save!!");
                return;
            }

            string configFileName = LST.SaveFileDialog("Cmc Datein (*.cmc)|*.cmc;|Alle Dateien (*.*)|*.*\"");
            if (String.IsNullOrEmpty(configFileName))
                return;

            ((UserControlTCPMDIChild)tw.Content).SetNewConnectionName(Path.GetFileName(configFileName));
            tw.Title = Path.GetFileName(configFileName);
            if(((UserControlTCPMDIChild)tw.Content).IsConnected)
                tw.Title += " (!)";
            else
                tw.Title += " ( )";
            Connection.Save(((UserControlTCPMDIChild)tw.Content).MyConnection, configFileName);
            _logger.Info(String.Format("Save Connection File {0}", configFileName));
        }

        /// <summary>
        /// CanSaveConnectionsCommand
        /// </summary>
        /// <returns></returns>
        private bool CanSaveConnectionsCommand()
        {
            return MainMdiContainer.Children.Count > 0;
        }

        /// <summary>
        /// OpenMessageFileCommandCF
        /// </summary>
        private void OpenMessageFileCommandCF()
        {
            Console.Beep();
        }

        /// <summary>
        /// CanOpenMessageFileCommand
        /// </summary>
        /// <returns></returns>
        private bool CanOpenMessageFileCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// SaveMessageFileCommandCF
        /// </summary>
        private void SaveMessageFileCommandCF()
        {
            Console.Beep();
        }

        /// <summary>
        /// CanSaveMessageFileCommand
        /// </summary>
        /// <returns></returns>
        private bool CanSaveMessageFileCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// SaveMessageFileAsCommandCF
        /// </summary>
        private void SaveMessageFileAsCommandCF()
        {
            Console.Beep();
        }

        /// <summary>
        /// CanSaveMessageFileAsCommand
        /// </summary>
        /// <returns></returns>
        private bool CanSaveMessageFileAsCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// AddNewMessageCommandCF
        /// </summary>
        private void AddNewMessageCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) { Console.Beep(); return; }

            CreateNewMessage CreateNewMessageDlg = new CreateNewMessage();
            CreateNewMessageDlg.Owner = Window.GetWindow(this);
            var res = CreateNewMessageDlg.ShowDialog();
            if (!res.Value)
                return;

            FocusMessage = CreateNewMessageDlg.FocusMessage;
        }

        /// <summary>
        /// CanAddNewMessageCommand
        /// </summary>
        /// <returns></returns>
        private bool CanAddNewMessageCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            _logger.Debug(String.Format("#3 {0} IsConnected={1} ThreadId={2} hashcode={3}", LST.GetCurrentMethod(), uctmc.IsConnected, System.Threading.Thread.CurrentThread.ManagedThreadId, uctmc.GetHashCode()));
            return uctmc.IsConnected;
        }

        /// <summary>
        /// EditMessageCommandCF
        /// </summary>
        private void EditMessageCommandCF()
        {
            Console.Beep();
        }

        /// <summary>
        /// CanEditMessageCommand
        /// </summary>
        /// <returns></returns>
        private bool CanEditMessageCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// AddMessageCommandCF
        /// </summary>
        private void AddMessageCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) { Console.Beep(); return; }
            List<byte[]> allSelectetMessages = ((UserControlTCPMDIChild)tw.Content).GetAllSelectetMessages();

            EditMessages EditMessagesDlg = new EditMessages();
            EditMessagesDlg.MessagesToEdit = allSelectetMessages;
            EditMessagesDlg.Owner = Window.GetWindow(this);
            var res = EditMessagesDlg.ShowDialog();
            if (!res.Value)
                return;
        }

        /// <summary>
        /// CanAddMessageCommand
        /// </summary>
        /// <returns></returns>
        private bool CanAddMessageCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// EditAndReplaceMessageCommandCF
        /// </summary>
        private void EditAndReplaceMessageCommandCF()
        {
            Console.Beep();
        }

        /// <summary>
        /// CanEditAndReplaceMessageCommand
        /// </summary>
        /// <returns></returns>
        private bool CanEditAndReplaceMessageCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// SendCommandCF
        /// </summary>
        private void SendCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();

            if (tw == null) { Console.Beep(); return; }

            ((UserControlTCPMDIChild)tw.Content).FocusMessage = FocusMessage;
            ((UserControlTCPMDIChild)tw.Content).SendMessage(((UserControlTCPMDIChild)tw.Content).FocusMessage);
        }

        /// <summary>
        /// CanSendCommand
        /// </summary>
        /// <returns></returns>
        private bool CanSendCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected;
        }

        /// <summary>
        /// DeleteAllCommandCF
        /// </summary>
        private void DeleteAllCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) { Console.Beep(); return; }
            ((UserControlTCPMDIChild)tw.Content).DeleteAllMessages();
        }

        /// <summary>
        /// CanDeleteAllCommand
        /// </summary>
        /// <returns></returns>
        private bool CanDeleteAllCommand()
        {
            return true;
        }

        /// <summary>
        /// AboutCommandCF
        /// </summary>
        private void AboutCommandCF()
        {
            string StrVersion;
#if DEBUG
            StrVersion = "Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#else
            StrVersion = "Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#endif
            MessageBox.Show("About ComMonitor " + StrVersion, "ComMonitor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// CanAboutCommand
        /// </summary>
        /// <returns></returns>
        private bool CanAboutCommand()
        {
            return true;
        }
        #endregion

        #endregion
        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            this.Title += "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + Globals._revision.ToString();
#else
            this.Title += "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + Globals._revision.ToString();
#endif
        }

        /// <summary>
        /// Window_Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _logger.Info("Closing ComMonitor");
            Environment.Exit(0);
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions
        
        /// <summary>
        /// CallCanFuncions
        /// No better idea to update the ToolBar and Window-State yet
        /// </summary>
        public void UpdateWindow()
        {
        }

        /// <summary>
        /// GetTopMDIWindow
        /// </summary>
        /// <returns></returns>
        private MdiChild GetTopMDIWindow()
        {
            MdiChild tw = null;
            List<int> iZIndexList = new List<int>();

            if (MainMdiContainer.Children.Count == 0)
                return null;

            foreach (var w in MainMdiContainer.Children)
                iZIndexList.Add(System.Windows.Controls.Panel.GetZIndex(w));

            Debug.WriteLine(String.Join("; ", iZIndexList));
            int max = iZIndexList.Max();
            tw = MainMdiContainer.Children[iZIndexList.IndexOf(max)];

            return tw;
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
