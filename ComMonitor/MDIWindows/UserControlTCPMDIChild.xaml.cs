using ComMonitor.Models;
using HexMessageViewerControl;
using System.ComponentModel;
using System.Windows.Controls;
using System;
using ComMonitor.LocalTools;
using System.Net;
using System.Collections.Generic;
using WPF.MDI;

namespace ComMonitor.MDIWindows
{

    /// <summary>
    /// Interaktionslogik für UserControlTCPMDIChild.xaml
    /// </summary>
    public partial class UserControlTCPMDIChild : UserControl, INotifyPropertyChanged
    {
        private NLog.Logger _logger;
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindow _mainWindow;

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

        public bool IsConnected { get; set; }
        public Connection MyConnection { get; set; }
        public MdiChild TheMdiChild { get; set; }

        public int FocusMessageIndex { get; set; }
        public Message FocusMessage { get; set; }
        public List<Message> MessageList { get; set; }
        public string MessageFileName { get; set; }

        public virtual System.Windows.Threading.Dispatcher DispatcherObjectForTaskDispatcher { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserControlTCPMDIChild(Connection connection,MainWindow mainWindow)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            InitializeComponent();

            DispatcherObjectForTaskDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            MyConnection = connection;
            _mainWindow = mainWindow;
            var w = System.Windows.Window.GetWindow(this);
            switch (MyConnection.ConnectionType)
            {
                case EConnectionType.TCPSocketServer:
                    StartServer(MyConnection);
                    break;
                case EConnectionType.TCPSocketCient:
                    StartClient(MyConnection);
                    break;
            }

            FocusMessage = null;
            IsConnected = false;

            FocusMessage = null;
            MessageList = new List<Message>();
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
        /// ConStateChaneged
        /// </summary>
        /// <param name="conState"></param>
        private void ConStateChaneged(bool conState)
        {
            // Unlink the threads between TCP thread and UI thread:
            // http://stackoverflow.com/questions/2403972/c-sharp-events-between-threads-executed-in-their-own-thread-how-to
            DispatcherObjectForTaskDispatcher.BeginInvoke(new Action(
                () =>
                {
                    IsConnected = conState;
                    if(IsConnected)
                        TheMdiChild.Title = String.Format("{0} (!)", MyConnection.ConnectionName);
                    else
                        TheMdiChild.Title = String.Format("{0} ( )", MyConnection.ConnectionName);
                    _mainWindow.UpdateWindow();
                    _logger.Debug(String.Format("#2 {0} IsConnected={1} ThreadId={2} hashcode={3}", LST.GetCurrentMethod(), IsConnected, System.Threading.Thread.CurrentThread.ManagedThreadId, GetHashCode()));
                }));
        }

        /// <summary>
        /// mDIWindow_MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDIWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(_mainWindow != null)
                if(MyConnection.ConnectionType == EConnectionType.TCPSocketCient)
                    _mainWindow.StatusPannelOut(String.Format("{0} {1} {2}", MyConnection.ConnectionType, MyConnection.IPAdress, MyConnection.Port));
                else
                    _mainWindow.StatusPannelOut(String.Format("{0} {1}", MyConnection.ConnectionType, MyConnection.Port));
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
                    _minaTCPServer.ConnectionStateChaneged -= ConStateChaneged;
                    break;
                case EConnectionType.TCPSocketCient:
                    _minaTCPClient.Close();
                    _minaTCPClient.ConnectionStateChaneged -= ConStateChaneged;
                    break;
            }
            _logger.Debug(String.Format("{0} ------------------------------- IsConnected={1} ThreadId={2} hashcode={3}", LST.GetCurrentMethod(), IsConnected, System.Threading.Thread.CurrentThread.ManagedThreadId, GetHashCode()));
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        #region Public Functions
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
        /// GetAllMessages
        /// </summary>
        /// <returns></returns>
        public List<byte[]> GetAllMessages()
        {
            return hexUC.GetAllMessages();
        }

        /// <summary>
        /// DeleteAllMessages
        /// </summary>
        public void DeleteAllMessages()
        {
            hexUC.ClearAllMessage();
        }
        #endregion

        /// <summary>
        /// StartServer
        /// </summary>
        /// <param name="myConnection"></param>
        private void StartServer(Connection myConnection)
        {
            _minaTCPServer = new MinaTCPServer(MyConnection.Port, ProcessMessage);
            _minaTCPServer.ConnectionStateChaneged += ConStateChaneged;
            _minaTCPServer.StartMinaListener();
            _logger.Info(String.Format("StartServer Port: {0} MultipleConnections: {1}",myConnection.Port,myConnection.MultipleConnections));
        }

        /// <summary>
        /// SetNewConnectionName
        /// </summary>
        /// <param name="newName"></param>
        public void SetNewConnectionName(string newName)
        {
            MyConnection.ConnectionName = newName;
        }

        /// <summary>
        /// StartClient
        /// </summary>
        /// <param name="myConnection"></param>
        private void StartClient(Connection myConnection)
        {
            _minaTCPClient = new MinaTCPClient(IPAddress.Parse(MyConnection.IPAdress), MyConnection.Port, ProcessMessage);        
            _minaTCPClient.ConnectionStateChaneged += ConStateChaneged;
            _minaTCPClient.OpenMinaSocket();
            _logger.Info(String.Format("StartClient Ip: {0} Port: {1}", myConnection.IPAdress, myConnection.Port));
        }

        /// <summary>
        /// ProcessMessage
        /// </summary>
        private void ProcessMessage(byte[] message,Direction direction)
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

        #endregion
    }
}
