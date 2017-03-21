using Mina.Filter.Codec.Demux;

namespace ComMonitor.LocalTools
{
    class ProtocolCodecFactory : DemuxingProtocolCodecFactory
    {
        public ProtocolCodecFactory()
        {
            AddMessageDecoder<TCPClientProtocolManager>();
            AddMessageEncoder<byte[], TCPClientProtocolManager>();
        }
    }
}
