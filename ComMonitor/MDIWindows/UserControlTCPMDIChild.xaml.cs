using HexMessageViewerControl;
using System.ComponentModel;
using System.Windows.Controls;

namespace ComMonitor.MDIWindows
{
    /// <summary>
    /// Interaktionslogik für UserControlTCPMDIChild.xaml
    /// </summary>
    public partial class UserControlTCPMDIChild : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region INotify Propertie Changed
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public UserControlTCPMDIChild()
        {
            InitializeComponent();

            //ContainerUCAction = HexMessageContainerUCAction.Instance;
        }

        /// <summary>
        /// ProcessMessage
        /// </summary>
        public void ProcessMessage(byte[] message,Direction direction)
        {
            hexUC.AddMessage(message, direction);
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
    }
}
