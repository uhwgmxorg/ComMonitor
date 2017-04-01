using System;
using System.Diagnostics;
using System.IO;
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
        [XmlElement("ConnectionName")]
        public string ConnectionName { get; set; }
        [XmlElement("ConnectionType")]
        public EConnectionType ConnectionType { get; set; }
        [XmlElement("IPAdress")]
        public string IPAdress { get; set; }
        [XmlElement("Port")]
        public int Port { get; set; }
        [XmlElement("MultipleConnections")]
        public bool MultipleConnections { get; set; }
        [XmlElement("AutoConnections")]
        public bool AutoConnections { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Connection()
        {
            SetDefault();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        public Connection(string fileName)
        {
            ConnectionName = Path.GetFileName(fileName);
            ConnectionType = EConnectionType.TCPSocketServer;
            SetDefault();
            FileName = fileName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port"></param>
        /// <param name="fileName"></param>
        public Connection(int port,string fileName)
        {
            ConnectionName = Path.GetFileName(fileName);
            ConnectionType = EConnectionType.TCPSocketServer;
            SetDefault();
            Port = port;
            FileName = fileName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iPAdress"></param>
        /// <param name="port"></param>
        /// <param name="fileName"></param>
        public Connection(string iPAdress, int port, string fileName)
        {
            ConnectionName = Path.GetFileName(fileName);
            ConnectionType = EConnectionType.TCPSocketServer;
            IPAdress = iPAdress;
            Port = port;
            FileName = fileName;
        }

        /// <summary>
        /// SetDefault
        /// </summary>
        public void SetDefault()
        {
            FileName = "New Connection.cmc";
            ConnectionName = Path.GetFileName(FileName);
            ConnectionType = EConnectionType.TCPSocketServer;
            IPAdress = "127.0.0.1";
            Port = 4711;
            MultipleConnections = false;
            AutoConnections = true;
        }

        /// <summary>
        /// SaveClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        static public void Save(Connection obj, string fileName)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Connection));
                using (StreamWriter wr = new StreamWriter(fileName))
                {
                    xs.Serialize(wr, obj);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// LoadClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        static public Connection Load(string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Connection));

                using (StreamReader rd = new StreamReader(fileName))
                {
                    var Obj = serializer.Deserialize(rd);
                    return (Connection)Obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Connection con = new Connection();
                con.SetDefault();
                return con;
            }
        }
    }
}
