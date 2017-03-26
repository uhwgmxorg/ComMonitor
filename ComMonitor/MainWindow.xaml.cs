using ComMonitor.Dialogs;
using ComMonitor.LocalTools;
using ComMonitor.MDIWindows;
using ComMonitor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WPF.MDI;

namespace ComMonitor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private NLog.Logger _logger;
        private Random _random = new Random();

        public byte[] FocusMessage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();

            FocusMessage = null;
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        /// <summary>
        /// MenuItem_Click_Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>MenuItem_Click_NewMDIWindow
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            _logger.Info("Closing ComMonitor");
            Environment.Exit(0);
        }

        #region ToolBar
        /// <summary>
        /// MenuItem_Click_NewMDIWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_NewConnectionsWindow(object sender, RoutedEventArgs e)
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
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild(ConfigNewConnectionDlg.ConnectionObj)
            };
            MainMdiContainer.Children.Add(MdiChild);
        }

        /// <summary>
        /// MenuItem_Click_LoadConnections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_LoadConnections(object sender, RoutedEventArgs e)
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
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild(newConnection)
            };
            MainMdiContainer.Children.Add(MdiChild);
        }

        /// <summary>
        /// MenuItem_Click_SaveConnections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_SaveConnections(object sender, RoutedEventArgs e)
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

            Connection.Save(((UserControlTCPMDIChild)tw.Content).MyConnection, configFileName);
            _logger.Info(String.Format("Save Connection File {0}", configFileName));
        }

        private void MenuItem_Click_OpenMessageFile(object sender, RoutedEventArgs e)
        {
            Console.Beep();
        }

        private void MenuItem_Click_SaveMessageFile(object sender, RoutedEventArgs e)
        {
            Console.Beep();
        }

        private void MenuItem_Click_SaveMessageFileAs(object sender, RoutedEventArgs e)
        {
            Console.Beep();
        }

        /// <summary>
        /// MenuItem_Click_AddNewMessage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_AddNewMessage(object sender, RoutedEventArgs e)
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

        private void MenuItem_Click_EditMessage(object sender, RoutedEventArgs e)
        {
            Console.Beep();
        }

        /// <summary>
        /// MenuItem_Click_AddMessage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_AddMessage(object sender, RoutedEventArgs e)
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

        private void MenuItem_Click_EditAndReplaceMessage(object sender, RoutedEventArgs e)
        {
            Console.Beep();
        }

        /// <summary>
        /// MenuItem_Click_Send
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Send(object sender, RoutedEventArgs e)
        {
            MdiChild tw = GetTopMDIWindow();

            if (tw == null) { Console.Beep(); return; }

            ((UserControlTCPMDIChild)tw.Content).FocusMessage = FocusMessage;
            ((UserControlTCPMDIChild)tw.Content).SendMessage(((UserControlTCPMDIChild)tw.Content).FocusMessage);
        }

        /// <summary>
        /// MenuItem_Click_DeleteAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_DeleteAll(object sender, RoutedEventArgs e)
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) { Console.Beep(); return; }
            ((UserControlTCPMDIChild)tw.Content).DeleteAllMessages();
        }

        /// <summary>
        /// MenuItem_Click_About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>MenuItem_Click_ChangeLog
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            string StrVersion;
#if DEBUG
            StrVersion = "Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#else
            StrVersion = "Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#endif
            MessageBox.Show("About ComMonitor " + StrVersion, "ComMonitor", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        /// <summary>
        /// MenuItem_Click_Tideled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Tideled(object sender, RoutedEventArgs e)
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
        /// MenuItem_Click_Cascade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>MenuItem_Click_CloseAll
        private void MenuItem_Click_Cascade(object sender, RoutedEventArgs e)
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
        /// MenuItem_Click_CloseAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_CloseAll(object sender, RoutedEventArgs e)
        {
            for (int i = MainMdiContainer.Children.Count - 1; i >= 0; i--)
                MainMdiContainer.Children[i].Close();
        }

        /// <summary>
        /// MenuItem_Click_ChangeLog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_ChangeLog(object sender, RoutedEventArgs e)
        {
            string File = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ChangeLog.txt";
            ChangeLogUtilityDll.ChangeLogTxtToolWindow ChangeLogTxtToolWindowObj = new ChangeLogUtilityDll.ChangeLogTxtToolWindow(this);
            ChangeLogTxtToolWindowObj.ShowChangeLogWindow(File);
        }

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
            EnableDisableContols();
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
        /// EnableDisableContols
        /// </summary>
        private void EnableDisableContols()
        {
            tb_MenuItem_NewConnectionsWindow.IsEnabled = true;
            tb_MenuItem_Click_LoadConnections.IsEnabled = true;
            tb_MenuItem_SaveConnections.IsEnabled = true;
            tb_MenuItem_OpenMessageFile.IsEnabled = true;
            tb_MenuItem_SaveMessageFile.IsEnabled = true;
            tb_MenuItem_SaveMessageFileAs.IsEnabled = true;
            tb_MenuItem_AddNewMessage.IsEnabled = true;
            tb_MenuItem_EditMessage.IsEnabled = true;
            tb_MenuItem_AddMessage.IsEnabled = true;
            tb_MenuItem_EditAndReplaceMessage.IsEnabled = true;
            tb_MenuItem_Send.IsEnabled = true;
            tb_MenuItem_DeleteAll.IsEnabled = true;
        }

        #endregion
    }
}
