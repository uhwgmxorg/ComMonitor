using System.Xml.Serialization;

namespace ComMonitor.Models
{
    public class Message
    {
        [XmlElement("MessageName")]
        public string MessageName { get; set; }
        [XmlElement("Content")]
        public byte[] Content { get; set; }
    }
}
