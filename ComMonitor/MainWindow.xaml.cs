using ComMonitor.Dialogs;
using ComMonitor.LocalTools;
using ComMonitor.MDIWindows;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();
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
        }

        /// <summary>
        /// MenuItem_Click_SaveConnections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_SaveConnections(object sender, RoutedEventArgs e)
        {
        }

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

        /// <summary>
        /// MenuItem_Click_Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Message(object sender, RoutedEventArgs e)
        {
            MdiChild tw = GetTopMDIWindow();
            if (tw == null) return;
                ((UserControlTCPMDIChild)tw.Content).SendMessage(LST.RandomByteArray());
        }
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
            MessageBox.Show("About ComMonitor "+ StrVersion, "ComMonitor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

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

        #endregion
    }
}
