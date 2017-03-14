using ComMonitor.Models;
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
        private HexMessageContainerUCAction containerUCAction;
        public HexMessageContainerUCAction ContainerUCAction
        {
            get { return containerUCAction; }
            set
            {
                if (value != ContainerUCAction)
                {
                    containerUCAction = value;
                    OnPropertyChanged("ContainerUCAction");
                };
            }
        }
        #endregion

        public Connection MyConnection { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserControlTCPMDIChild(Connection connection)
        {
            InitializeComponent();

            MyConnection = connection;
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
