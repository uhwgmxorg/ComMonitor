using ComMonitor.LocalTools;
using System.Xml.Serialization;

namespace ComMonitor.Models
{
    public class Message
    {
        [XmlElement("MessageName")]
        public string MessageName { get; set; }
        [XmlElement("ProxyContent")]
        public string ProxyContent { get; set; }
        [XmlIgnore]
        public byte[] Content
        {
            get
            {
                return LST.HexStringToByteArray(ProxyContent);
            }
            set
            {
                ProxyContent = LST.ByteArrayToHexString(value);
            }
        }
    }
}
