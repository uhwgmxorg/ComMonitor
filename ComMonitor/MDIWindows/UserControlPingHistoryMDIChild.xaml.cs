using System.ComponentModel;
using System.Windows.Controls;
using WPF.MDI;

namespace ComMonitor.MDIWindows
{
    /// <summary>
    /// Interaktionslogik für UserControlPingHistoryMDIChild.xaml
    /// </summary>
    public partial class UserControlPingHistoryMDIChild : UserControl, INotifyPropertyChanged
    {
        private NLog.Logger _logger;
        public event PropertyChangedEventHandler PropertyChanged;

        public MdiChild TheMdiChild { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserControlPingHistoryMDIChild()
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

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

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
