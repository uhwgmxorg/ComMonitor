using Mina.Core.Buffer;
using Mina.Core.Future;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.Demux;

namespace ComMonitor.LocalTools
{
    public class TCPProtocolManager : IMessageDecoder, IMessageEncoder<byte[]>
    {
        public IoConnector Connector { get; set; }
        public IoSession Session { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(object data)
        {
            IWriteFuture WriteFuture;

            if (Session != null && data != null)
            {
                WriteFuture = Session.Write(data);
                return true;
            }
            else
                return false;
        }

        /******************************/
        /*      Ecoder and Decode     */
        /******************************/
        #region Ecoder and Decode

        public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
        {
            Encode(session, (byte[])message, output);
        }

        public void Encode(IoSession session, byte[] message, IProtocolEncoderOutput output)
        {
            var data = (byte[])message;
            IoBuffer buf = IoBuffer.Allocate(data.Length);
            buf.AutoExpand = true; // Enable auto-expand for easier encoding

            buf.Put(data);
            buf.Flip();
            output.Write(buf);
        }

        public MessageDecoderResult Decodable(IoSession session, IoBuffer input)
        {
            // Return NeedData if the whole header is not read yet.
            if (input.Remaining < 1)
                return MessageDecoderResult.NeedData;

            // Return OK if type and bodyLength matches.
            if (input.Remaining > 0)
                return MessageDecoderResult.OK;

            // Return NotOK if not matches.
            return MessageDecoderResult.NotOK;
        }

        public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            var value = input.GetRemaining().Array;
            input.Position = value.Length;
            if (value == null)
            {
                return MessageDecoderResult.NeedData;
            }

            output.Write(value);

            return MessageDecoderResult.OK;
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
        }
        #endregion
    }
}
