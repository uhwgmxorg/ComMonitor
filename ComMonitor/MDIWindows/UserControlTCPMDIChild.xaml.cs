using ComMonitor.Models;
using HexMessageViewerControl;
using System.ComponentModel;
using System.Windows.Controls;
using System;

namespace ComMonitor.MDIWindows
{

    /// <summary>
    /// Interaktionslogik für UserControlTCPMDIChild.xaml
    /// </summary>
    public partial class UserControlTCPMDIChild : UserControl, INotifyPropertyChanged
    {
        private NLog.Logger _logger;
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
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();

            MyConnection = connection;
            switch (MyConnection.ConnectionType)
            {
                case EConnectionType.TCPSocketServer:
                    StartServer(MyConnection);
                    break;
                case EConnectionType.TCPSocketCient:
                    StartClient(MyConnection);
                    break;
            }
        }

        /// <summary>
        /// StartServer
        /// </summary>
        /// <param name="myConnection"></param>
        private void StartServer(Connection myConnection)
        {
            _logger.Info(String.Format("StartServer Port {0} MultipleConnections {1}",myConnection.Port,myConnection.MultipleConnections));
        }

        /// <summary>
        /// StartClient
        /// </summary>
        /// <param name="myConnection"></param>
        private void StartClient(Connection myConnection)
        {
            _logger.Info(String.Format("StartClient Ip {0} Port {1} MultipleConnections {2}", myConnection.IPAdress, myConnection.Port, myConnection.MultipleConnections));
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
