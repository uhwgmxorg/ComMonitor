using ComMonitor.Models;
using HexMessageViewerControl;
using System.ComponentModel;
using System.Windows.Controls;
using System;
using ComMonitor.LocalTools;
using System.Net;
using System.Collections.Generic;

namespace ComMonitor.MDIWindows
{

    /// <summary>
    /// Interaktionslogik für UserControlTCPMDIChild.xaml
    /// </summary>
    public partial class UserControlTCPMDIChild : UserControl, INotifyPropertyChanged
    {
        private NLog.Logger _logger;
        public event PropertyChangedEventHandler PropertyChanged;

        private MinaTCPServer _minaTCPServer;
        private MinaTCPClient _minaTCPClient;

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

        public List<byte[]> PinnedMessages { get; set; }

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
            _minaTCPServer = new MinaTCPServer(MyConnection.Port, ProcessMessage);
            _logger.Info(String.Format("StartServer Port: {0} MultipleConnections: {1}",myConnection.Port,myConnection.MultipleConnections));
        }

        /// <summary>
        /// StartClient
        /// </summary>
        /// <param name="myConnection"></param>
        private void StartClient(Connection myConnection)
        {
            _minaTCPClient = new MinaTCPClient(IPAddress.Parse(MyConnection.IPAdress), MyConnection.Port, ProcessMessage);
            _logger.Info(String.Format("StartClient Ip: {0} Port: {1}", myConnection.IPAdress, myConnection.Port));
        }

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(byte[] message)
        {
            switch (MyConnection.ConnectionType)
            {
                case EConnectionType.TCPSocketServer:
                    _minaTCPServer.Send(message);
                    break;
                case EConnectionType.TCPSocketCient:
                    _minaTCPClient.Send(message);
                    break;
            }
            ProcessMessage(message, Direction.Out);
        }

        /// <summary>
        /// GetAllSelectedMessages
        /// </summary>
        /// <returns></returns>
        public List<byte[]> GetAllSelectetMessages()
        {
            return hexUC.GetAllSelectetMessages();
        }

        /// <summary>
        /// DeleteAllMessages
        /// </summary>
        public void DeleteAllMessages()
        {
            hexUC.ClearAllMessage();
        }

        /// <summary>
        /// ProcessMessage
        /// </summary>
        private void ProcessMessage(byte[] message,Direction direction)
        {
            hexUC.AddMessage(message, direction);
        }

        /// <summary>
        /// mDIWindow_Unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDIWindow_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (MyConnection.ConnectionType)
            {
                case EConnectionType.TCPSocketServer:
                    _minaTCPServer.Close();
                    break;
                case EConnectionType.TCPSocketCient:
                    _minaTCPClient.Close();
                    break;
            }
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
