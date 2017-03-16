using System.Xml.Serialization;

namespace ComMonitor.Models
{

    public enum EConnectionType
    {
        TCPSocketServer,
        TCPSocketCient
    }

    public class Connection
    {
        [XmlElement("ConnectionType")]
        public EConnectionType ConnectionType { get; set; }
        [XmlElement("IPAdress")]
        public string IPAdress { get; set; }
        [XmlElement("Port")]
        public int Port { get; set; }
        [XmlElement("MultipleConnections")]
        public bool MultipleConnections { get; set; }
    }
}
