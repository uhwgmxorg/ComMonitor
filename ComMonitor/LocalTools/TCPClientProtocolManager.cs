using Mina.Core.Future;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using System;
using System.Net;

namespace ComMonitor.LocalTools
{
    public class TCPClientProtocolManager : TCPProtocolManager
    {

        public IPAddress ServerIpAddress { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TCPClientProtocolManager()
        {
        }

        /// <summary>
        /// InitializeServer
        /// </summary>
        public void InitializeClient()
        {
            Connector = new AsyncSocketConnector();
            Connector.FilterChain.AddLast("logger", new LoggingFilter());
            Mina.Filter.Stream.StreamWriteFilter SIOFilter = new Mina.Filter.Stream.StreamWriteFilter();
            Connector.FilterChain.AddLast("codec", new ProtocolCodecFilter(new ProtocolCodecFactory()));

            Connector.SessionOpened += (s, e) =>
            {
                Session = e.Session;
            };
            Connector.SessionClosed += (s, e) =>
            {
                Session = null;
            };


            Connector.SessionConfig.ReadBufferSize = 1024;
            Connector.SessionConfig.SetIdleTime(IdleStatus.BothIdle, 10);
        }

        /// <summary>
        /// StartServer
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                IConnectFuture Future = Connector.Connect(new IPEndPoint(ServerIpAddress, Port));
                Future.Await();
                Session = Future.Session;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception {0}", ex.Message));
            }
        }
    }
}
