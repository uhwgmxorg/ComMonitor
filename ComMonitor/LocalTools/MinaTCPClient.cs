using Mina.Core.Session;
using NLog;
using System;
using System.Net;
using System.Timers;

namespace ComMonitor.LocalTools
{
    public class MinaTCPClient
    {
        public delegate void DProcessMessage(byte[] message, HexMessageViewerControl.Direction direction);
        public delegate void DEventHandlerConnectionStateChaneged(bool conState);

        private Logger _logger;
        private object _lockObject = new Object();
        private object _lockObject2 = new Object();
        private Timer _timer;

        public event DEventHandlerConnectionStateChaneged ConnectionStateChaneged;

        private TCPClientProtocolManager Manager { get; set; }
        private bool connected;
        public bool Connected
        {
            get
            {
                lock (_lockObject)
                {
                    return connected;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    if (value != Connected)
                        connected = value;
                }
            }
        }
        private bool AutoConnections { get; set; }

        private IPAddress _serverIpAddress;
        private Int32 _port;
        DProcessMessage CallProcessMessage;

        /// <summary>
        /// Constrguctor
        /// </summary>
        /// <param name="serverIPAddress"></param>
        /// <param name="port"></param>
        /// <param name="autoConnect"></param>
        public MinaTCPClient(IPAddress serverIPAddress, Int32 port, DProcessMessage callProcessMessage, bool autoConnect = false)
        {
            _logger = LogManager.GetCurrentClassLogger();
            Connected = false;

            _serverIpAddress = serverIPAddress;
            _port = port;
            CallProcessMessage = callProcessMessage;
            AutoConnections = autoConnect;

            Manager = new TCPClientProtocolManager();
        }

        /// <summary>
        /// OpenMinaSocket
        /// </summary>
        public void OpenMinaSocket()
        {
            if (Connected)
                return;
            try
            {
                Manager.InitializeClient();

                if (Manager.Connector == null)
                    throw new Exception("This should not happen!");

                Manager.Connector.ExceptionCaught += HandleException;
                Manager.Connector.SessionOpened += HandeleSessionOpened;
                Manager.Connector.SessionClosed += HandeleSessionClosed;
                Manager.Connector.SessionIdle += HandleIdle;
                Manager.Connector.MessageReceived += HandleReceived;

                Manager.ServerIpAddress = _serverIpAddress;
                Manager.Port = _port;
                Manager.ConnectToServer();
            }
            catch (Exception ex)
            {
                _logger.Error(String.Format("Exception in {0} {1}", LST.GetCurrentMethod(), ex.Message));
            }
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="message"></param>
        public void Send(byte[] message)
        {
            try
            {
                Manager.Send(message);

                _logger.Info(String.Format("Send data {0} Bytes", message.Length));
                _logger.Trace(String.Format("Send data => {0} | {1} |", ByteArrayToHexString(message), ByteArrayToAsciiString(message)));
            }
            catch (Exception ex)
            {
                _logger.Error(String.Format("Exception in {0} {1}", LST.GetCurrentMethod(), ex.Message));
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            try
            {
                StopReconnectTimer();

                if(Manager.Session != null)
                    Manager.Session.Close(true);

                Manager.Connector.ExceptionCaught -= HandleException;
                Manager.Connector.SessionOpened -= HandeleSessionOpened;
                Manager.Connector.SessionClosed -= HandeleSessionClosed;
                Manager.Connector.SessionIdle -= HandleIdle;
                Manager.Connector.MessageReceived -= HandleReceived;

                Manager.Connector.Dispose();
                Manager = null;
            }
            catch (Exception ex)
            {
                _logger.Error(String.Format("Exception in {0} {1}", LST.GetCurrentMethod(), ex.Message));
            }
        }

        /// <summary>
        /// StartReconnectTimer
        /// </summary>
        private void StartReconnectTimer()
        {
            if (AutoConnections)
            {
                _timer = new Timer(500);
                _timer.Elapsed += new ElapsedEventHandler(AutoReConnect);
                _timer.Enabled = true;
            }
        }

        /// <summary>
        /// StopReconnectTimer
        /// </summary>
        private void StopReconnectTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// AutoReConnect
        /// </summary>
        private void AutoReConnect(object sender, ElapsedEventArgs e)
        {
            lock (_lockObject2)
            {
                if (Connected) return;

                if (AutoConnections)
                {
                    _logger.Info(String.Format("AutoConnections ON try to connect to {0}:{1}", _serverIpAddress, _port));
                    Manager = new TCPClientProtocolManager();
                    OpenMinaSocket();
                }
            }
        }

        /******************************/
        /*          Events            */
        /******************************/
        #region Events

        /// <summary>
        /// HandleException
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleException(Object sender, IoSessionExceptionEventArgs e)
        {
            _logger.Info(String.Format("Exception {0}", e.Exception.Message));
        }

        /// <summary>
        /// HandeleSessionOpened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandeleSessionOpened(Object sender, IoSessionEventArgs e)
        {
            Connected = true;

            _logger.Info(String.Format("SessionOpened {0}", e.Session.RemoteEndPoint));
            _logger.Debug(String.Format("#1 {0} IsConnected={1} ThreadId={2} hashcode={3}", LST.GetCurrentMethod(), Connected, System.Threading.Thread.CurrentThread.ManagedThreadId, GetHashCode()));
            if (ConnectionStateChaneged != null)
                ConnectionStateChaneged(Connected);
            else
                _logger.Error(String.Format("Call HandeleSessionOpened but ConnectionStateChaneged Event is null"));

            if (AutoConnections)
                StopReconnectTimer();
        }

        /// <summary>
        /// HandeleSessionClosed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandeleSessionClosed(Object sender, IoSessionEventArgs e)
        {
            Connected = false;
            _logger.Info(String.Format("SessionClosed {0}", e.Session.RemoteEndPoint));
            _logger.Debug(String.Format("#1 {0} IsConnected={1} ThreadId={2} hashcode={3}", LST.GetCurrentMethod(), Connected, System.Threading.Thread.CurrentThread.ManagedThreadId, GetHashCode()));
            if (ConnectionStateChaneged != null)
                ConnectionStateChaneged(Connected);
            else
                _logger.Error(String.Format("Call HandeleSessionOpened but ConnectionStateChaneged Event is null"));

            Manager.Connector.ExceptionCaught -= HandleException;
            Manager.Connector.SessionOpened -= HandeleSessionOpened;
            Manager.Connector.SessionClosed -= HandeleSessionClosed;
            Manager.Connector.SessionIdle -= HandleIdle;
            Manager.Connector.MessageReceived -= HandleReceived;

            Manager.Connector.Dispose();
            Manager = null;

            if (AutoConnections)
                StartReconnectTimer();
        }

        /// <summary>
        /// HandleIdle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleIdle(Object sender, IoSessionIdleEventArgs e)
        {
            _logger.Info(String.Format("Idle {0}", e.Session.BothIdleCount));
        }

        /// <summary>
        /// HandleReceived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleReceived(Object sender, IoSessionMessageEventArgs e)
        {
            var bytes = (byte[])e.Message;

            if (CallProcessMessage != null)
                CallProcessMessage(bytes, HexMessageViewerControl.Direction.In);

            _logger.Info(String.Format("Received data {0} Bytes", bytes.Length));
            _logger.Trace(String.Format("Received data <= {0} | {1} |", ByteArrayToHexString(bytes), ByteArrayToAsciiString(bytes)));
        }
        private string ByteArrayToHexString(byte[] buf)
        {
            System.Text.StringBuilder hex = new System.Text.StringBuilder(buf.Length * 2);
            foreach (byte b in buf)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }
        private string ByteArrayToAsciiString(byte[] buf)
        {
            char[] carray = new char[buf.Length];
            char c;

            for (int i = 0; i < buf.Length; i++)
            {
                if (33 <= buf[i] && buf[i] <= 127)
                    c = (char)buf[i];
                else
                    c = '.';
                carray[i] = c;
            }

            return new String(carray);
        }

        #endregion
    }
}
