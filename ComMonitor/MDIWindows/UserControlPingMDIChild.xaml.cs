using System;
using System.ComponentModel;
using System.Windows.Controls;
using WPF.MDI;

namespace ComMonitor.MDIWindows
{
    /// <summary>
    /// Interaktionslogik für UserControlPingMDIChild.xaml
    /// </summary>
    public partial class UserControlPingMDIChild : UserControl, INotifyPropertyChanged
    {
        private NLog.Logger _logger;
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindow _mainWindow;

        public MdiChild TheMdiChild { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserControlPingMDIChild(MainWindow mainWindow)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();
            _mainWindow = mainWindow;
            phtUC.IpChange += IPTargetChange;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~UserControlPingMDIChild()
        {
            phtUC.IpChange -= IPTargetChange;
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

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// UserControl_MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IPTargetChange(object sender, EventArgs e)
        {
            TheMdiChild.Title = phtUC.PingTarget;
        }

        /// <summary>
        /// mDIWindow_MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDIWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mainWindow != null)
                _mainWindow.StatusPannelOut(String.Format(phtUC.PingTarget));
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        #region Public Functions

        #endregion

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
