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

        public RelayCommand TideledCommand { get; private set; }
        public RelayCommand CascadeCommand { get; private set; }
        public RelayCommand CloseAllCommand { get; private set; }
        public RelayCommand ChangeLogCommand { get; private set; }
        public RelayCommand ListOfKnownBugsCommand { get; private set; }


        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand DeleteListCommand { get; private set; }
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

        #region RecentFileList Properties and Vars
        const string NoFile = "No file";
        const string NewFile = "New nameless file";

        public static DependencyProperty FilepathProperty =
            DependencyProperty.Register(
            "Filepath",
            typeof(string),
            typeof(MainWindow),
            new PropertyMetadata(NoFile));
        public string Filepath
        {
            get { return (string)GetValue(FilepathProperty); }
            set { SetValue(FilepathProperty, value); }
        }

        bool _IsFileNamed = false;
        bool _IsFileLoaded = false;
        MemoryStream _MemoryStream = new MemoryStream();
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();
            DataContext = this;

            ExitCommand = new RelayCommand(ExitCommandCF, CanExitCommand);
            DeleteListCommand = new RelayCommand(DeleteListCommandCF, CanDeleteListCommand);
            TideledCommand = new RelayCommand(TideledCommandCF, CanTideledCommand);
            CascadeCommand = new RelayCommand(CascadeCommandCF, CanCascadeCommand);
            CloseAllCommand = new RelayCommand(CloseAllCommandCF, CanCloseAllCommand);
            ChangeLogCommand = new RelayCommand(ChangeLogCommandCF, CanChangeLogCommand);
            ListOfKnownBugsCommand = new RelayCommand(ListOfKnownBugsCommandCF, CanListOfKnownBugsCommand);
            NewConnectionsWindowCommand = new RelayCommand(NewConnectionsWindowCommandCF, CanNewConnectionsWindowCommand);
            LoadConnectionsCommand = new RelayCommand(LoadConnectionsCommandCF, CanLoadConnectionsCommand);
            SaveConnectionsCommand = new RelayCommand(SaveConnectionsCommandCF, CanSaveConnectionsCommand);
            OpenMessageFileCommand = new RelayCommand(OpenMessageFileCommandCF, CanOpenMessageFileCommand);
            SaveMessageFileCommand = new RelayCommand(SaveMessageFileCommandCF, CanSaveMessageFileCommand);
            SaveMessageFileAsCommand = new RelayCommand(SaveMessageFileAsCommandCF, CanSaveMessageFileAsCommand);
            AddNewMessageCommand = new RelayCommand(AddNewMessageCommandCF, CanAddNewMessageCommand);
            EditMessageCommand = new RelayCommand(EditMessageCommandCF, CanEditMessageCommand);
            AddMessageCommand = new RelayCommand(AddSelectedMessageCommandCF, CanAddSelectedMessageCommand);
            EditAndReplaceMessageCommand = new RelayCommand(EditAndReplaceMessageCommandCF, CanEditAndReplaceMessageCommand);
            SendCommand = new RelayCommand(SendCommandCF, CanSendCommand);
            DeleteAllCommand = new RelayCommand(DeleteAllCommandCF, CanDeleteAllCommand);
            AboutCommand = new RelayCommand(AboutCommandCF, CanAboutCommand);

            RecentFileList.MenuClick += (s, e) => LoadRecentFile(e.Filepath);
        }

        /******************************/
        /*     Command Functions      */
        /******************************/
        #region Command Functions

        #region MenueBar Only
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
        /// 
        /// </summary>
        private void ListOfKnownBugsCommandCF()
        {
            string File = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ListOfKnownBugs.txt";
            ChangeLogUtilityDll.ChangeLogTxtToolWindow ListOfKnownBugsTxtToolWindowObj = new ChangeLogUtilityDll.ChangeLogTxtToolWindow(this);
            ListOfKnownBugsTxtToolWindowObj.BackgroundDisplayText = "L.O.K. Bugs";
            ListOfKnownBugsTxtToolWindowObj.ShowChangeLogWindow(File);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanListOfKnownBugsCommand()
        {
            return true;
        }

        /// <summary>
        /// ExitCommandCF
        /// </summary>
        private void ExitCommandCF()
        {
            Properties.Settings.Default.Save();
            Close();
            Environment.Exit(0);
        }

        /// <summary>
        /// CanExitCommand
        /// </summary>
        /// <returns></returns>
        private bool CanExitCommand()
        {
            return true;
        }

        /// <summary>
        /// DeleteListCommandCF
        /// </summary>
        private void DeleteListCommandCF()
        {
            foreach (var v in RecentFileList.RecentFiles)
                RecentFileList.RemoveFile(v);
        }

        /// <summary>
        /// CanDeleteListCommand 
        /// </summary>
        /// <returns></returns>
        private bool CanDeleteListCommand()
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

            RecentFileList.InsertFile(configFileName);
            Filepath = configFileName;
            _IsFileLoaded = true;
            _IsFileNamed = true;

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
                _logger.Info("Nothing to save!!");
                return;
            }

            string configFileName = LST.SaveFileDialog("Cmc Datein (*.cmc)|*.cmc;|Alle Dateien (*.*)|*.*\"");
            if (String.IsNullOrEmpty(configFileName))
                return;

            RecentFileList.InsertFile(configFileName);
            Filepath = configFileName;
            _IsFileLoaded = true;
            _IsFileNamed = true;

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
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;

            string messageFileName = LST.OpenFileDialog("Cmm Datein (*.cmm)|*.cmm;|Alle Dateien (*.*)|*.*\"");
            if (String.IsNullOrEmpty(messageFileName))
                return;

            ((UserControlTCPMDIChild)tw.Content).MessageList = LST.LoadList<Message>(messageFileName);
            ((UserControlTCPMDIChild)tw.Content).MessageFileName = messageFileName;
            if (((UserControlTCPMDIChild)tw.Content).MessageList.Count > 0)
            {
                ((UserControlTCPMDIChild)tw.Content).FocusMessageIndex = 0;
                ((UserControlTCPMDIChild)tw.Content).FocusMessage = ((UserControlTCPMDIChild)tw.Content).MessageList[0];
            }
            _logger.Info(String.Format("Lode MessageFile File {0}", messageFileName));
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
            MdiChild tw = GetTopMDIWindow();
            if (tw == null)
            {
                _logger.Info("Nothing to save!!");
                return;
            }

            if(String.IsNullOrEmpty(((UserControlTCPMDIChild)tw.Content).MessageFileName) || ((UserControlTCPMDIChild)tw.Content).MessageFileName.Contains("New MessageFile"))
            {
                SaveMessageFileAsCommandCF();
                return;
            }

            LST.SaveList<Message>(((UserControlTCPMDIChild)tw.Content).MessageList, ((UserControlTCPMDIChild)tw.Content).MessageFileName);
            _logger.Info(String.Format("Save MessageFile File {0}", (((UserControlTCPMDIChild)tw.Content).MessageFileName)));
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
            return uctmc.IsConnected && uctmc.MessageList.Count > 0;
        }

        /// <summary>
        /// SaveMessageFileAsCommandCF
        /// </summary>
        private void SaveMessageFileAsCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null)
            {
                _logger.Info("Nothing to save!!");
                return;
            }

            string messageFileName = LST.SaveFileDialog("Cmm Datein (*.cmm)|*.cmm;|Alle Dateien (*.*)|*.*\"");
            if (String.IsNullOrEmpty(messageFileName))
                return;

            ((UserControlTCPMDIChild)tw.Content).MessageFileName = messageFileName;
            LST.SaveList<Message>(((UserControlTCPMDIChild)tw.Content).MessageList, messageFileName);
            _logger.Info(String.Format("Save MessageFile File As {0}", messageFileName));
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
            return uctmc.IsConnected && uctmc.MessageList.Count > 0;
        }

        /// <summary>
        /// AddNewMessageCommandCF
        /// </summary>
        private void AddNewMessageCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;

            CreateNewMessage CreateNewMessageDlg = new CreateNewMessage();
            CreateNewMessageDlg.Owner = Window.GetWindow(this);
            var res = CreateNewMessageDlg.ShowDialog();
            if (!res.Value)
                return;

            uctmc.FocusMessage = new Message { Content = CreateNewMessageDlg.FocusMessage };
            uctmc.MessageList.Add( uctmc.FocusMessage);
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
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;

            EditMessages EditMessagesDlg = new EditMessages();
            EditMessagesDlg.SelectedTabItemsIndex = uctmc.FocusMessageIndex;
            EditMessagesDlg.MessagesToEdit = ((UserControlTCPMDIChild)tw.Content).MessageList;
            EditMessagesDlg.Owner = Window.GetWindow(this);
            var res = EditMessagesDlg.ShowDialog();
            if (!res.Value)
                return;

            uctmc.FocusMessageIndex = EditMessagesDlg.SelectedTabItemsIndex;
            uctmc.FocusMessage = EditMessagesDlg.MessagesToEdit[EditMessagesDlg.SelectedTabItemsIndex];
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
            if (uctmc.MessageList == null) return false;
            return uctmc.MessageList.Count > 0;
        }

        /// <summary>
        /// AddMessageCommandCF
        /// </summary>
        private void AddSelectedMessageCommandCF()
        {
            int count = 0;
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            List<byte[]> allSelectetMessages = ((UserControlTCPMDIChild)tw.Content).GetAllSelectetMessages();
            List<Message> allSelectetMessagesWithDefaultName = new List<Message>();
            foreach (var b in allSelectetMessages)
                allSelectetMessagesWithDefaultName.Add(new Message { MessageName = String.Format("New Seleced Message {0}", ++count), Content = b });

            EditMessages EditMessagesDlg = new EditMessages();
            EditMessagesDlg.MessagesToEdit = allSelectetMessagesWithDefaultName;
            EditMessagesDlg.Owner = Window.GetWindow(this);
            var res = EditMessagesDlg.ShowDialog();
            if (!res.Value)
                return;

            uctmc.FocusMessage = new Message { Content = EditMessagesDlg.FocusMessage };
            foreach(var m in EditMessagesDlg.MessagesToEdit)
                uctmc.MessageList.Add(new Message { MessageName = m.MessageName, Content = m.Content });
        }

        /// <summary>
        /// CanAddMessageCommand
        /// </summary>
        /// <returns></returns>
        private bool CanAddSelectedMessageCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected && uctmc.GetAllMessages().Count > 0 && uctmc.GetAllSelectetMessages().Count > 0;
        }

        /// <summary>
        /// EditAndReplaceMessageCommandCF
        /// </summary>
        private void EditAndReplaceMessageCommandCF()
        {
            int count = 0;
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            List<byte[]> allSelectetMessages = ((UserControlTCPMDIChild)tw.Content).GetAllSelectetMessages();
            uctmc.MessageList.Clear();
            List<Message> allSelectetMessagesWithDefaultName = new List<Message>();
            foreach (var b in allSelectetMessages)
                allSelectetMessagesWithDefaultName.Add(new Message { MessageName = String.Format("New Seleced Message {0}", ++count), Content = b });

            EditMessages EditMessagesDlg = new EditMessages();
            EditMessagesDlg.MessagesToEdit = allSelectetMessagesWithDefaultName;
            EditMessagesDlg.Owner = Window.GetWindow(this);
            var res = EditMessagesDlg.ShowDialog();
            if (!res.Value)
                return;

            uctmc.FocusMessage = new Message { Content = EditMessagesDlg.FocusMessage };
            foreach (var m in EditMessagesDlg.MessagesToEdit)
                uctmc.MessageList.Add(new Message { MessageName = m.MessageName, Content = m.Content });
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
            return uctmc.IsConnected && uctmc.GetAllMessages().Count > 0 && uctmc.GetAllSelectetMessages().Count > 0;
        }

        /// <summary>
        /// SendCommandCF
        /// </summary>
        private void SendCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            ((UserControlTCPMDIChild)tw.Content).SendMessage(((UserControlTCPMDIChild)tw.Content).FocusMessage.Content);
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
            return uctmc.IsConnected && uctmc.MessageList.Count > 0;
        }

        /// <summary>
        /// DeleteAllCommandCF
        /// </summary>
        private void DeleteAllCommandCF()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
            ((UserControlTCPMDIChild)tw.Content).DeleteAllMessages();
        }

        /// <summary>
        /// CanDeleteAllCommand
        /// </summary>
        /// <returns></returns>
        private bool CanDeleteAllCommand()
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return false;
            UserControlTCPMDIChild uctmc = GetTopMDIWindow().Content as UserControlTCPMDIChild;
            return uctmc.IsConnected && uctmc.GetAllMessages().Count > 0;
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
            Dialogs.AboutBox AboutBox = new AboutBox(StrVersion);
            AboutBox.Owner = Window.GetWindow(this);
            var res = AboutBox.ShowDialog();
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
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// StatusPannelOut
        /// </summary>
        /// <param name="text"></param>
        public void StatusPannelOut(string text)
        {
            XTBStatus.Text = text;
        }

        /// <summary>
        /// LoadResentFile
        /// </summary>
        /// <param name="configFileName"></param>
        private void LoadRecentFile(string configFileName)
        {
            double AcParentWindoHeight = ActualHeight;
            double AcParentWindoWidth = ActualWidth;

            Connection newConnection = Connection.Load(configFileName);
            _logger.Info(String.Format("Load Connection File {0}", configFileName));

            MdiChild MdiChild = new MdiChild()
            {
                Title = String.Format("{0} ( )", Path.GetFileName(configFileName)),
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild(newConnection, this)
            };
            ((UserControlTCPMDIChild)MdiChild.Content).TheMdiChild = MdiChild;
            MainMdiContainer.Children.Add(MdiChild);
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

            //Debug.WriteLine("MDI-Windows ZIndexList:");
            //Debug.Write(String.Join("; ", iZIndexList));
            //Debug.WriteLine("");

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
