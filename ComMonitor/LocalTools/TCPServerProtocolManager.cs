using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using System;
using System.Net;

namespace ComMonitor.LocalTools
{
    public class TCPServerProtocolManager : TCPProtocolManager
    {
        public IoAcceptor Acceptor { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TCPServerProtocolManager()
        {
        }

        /// <summary>
        /// InitializeServer
        /// </summary>
        public void InitializeServer()
        {
            if (Acceptor != null)
                throw new Exception("This should not happen!");

            Acceptor = new AsyncSocketAcceptor();
            Acceptor.FilterChain.AddLast("logger", new LoggingFilter());
            Mina.Filter.Stream.StreamWriteFilter SIOFilter = new Mina.Filter.Stream.StreamWriteFilter();
            Acceptor.FilterChain.AddLast("codec", new ProtocolCodecFilter(new ProtocolCodecFactory()));

            Acceptor.SessionOpened += (s, e) =>
            {
                Session = e.Session;
            };
            Acceptor.SessionClosed += (s, e) =>
            {
                Session = null;
            };

            Acceptor.SessionConfig.ReadBufferSize = 1024;
            Acceptor.SessionConfig.SetIdleTime(IdleStatus.BothIdle, 10);
        }

        /// <summary>
        /// StartServer
        /// </summary>
        public void StartServer()
        {
            Acceptor.Bind(new IPEndPoint(IPAddress.Any, Port));
        }
    }
}
