using ComMonitor.LocalTools;
using ComMonitor.MDIWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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

            MdiChild MdiChild = new MdiChild()
            {
                Height = (AcParentWindoHeight - MainMenu.ActualHeight - MainToolBar.ActualHeight) * 0.6,
                Width = AcParentWindoWidth * 0.6,
                Content = new UserControlTCPMDIChild()
            };
            MainMdiContainer.Children.Add(MdiChild);
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
            Close();
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
        /// Window_MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SendATestMessagToEachMDIWindow(e);
        }

        private void SendATestMessagToEachMDIWindow(MouseButtonEventArgs e)
        {
            // just for develoment
            foreach (var w in MainMdiContainer.Children)
            {
                UserControlTCPMDIChild wmdiuc = w.Content as UserControlTCPMDIChild;
                wmdiuc.ProcessMessage(LST.RandomByteArray(), e.ChangedButton == MouseButton.Right ? HexMessageViewerControl.Direction.In : HexMessageViewerControl.Direction.Out);
                Debug.WriteLine("Send Debug Message");
            }
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        #endregion
    }
}
