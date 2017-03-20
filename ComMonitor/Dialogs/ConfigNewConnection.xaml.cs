using ComMonitor.Models;
using System.ComponentModel;
using System.Windows;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für ConfigNewConnection.xaml
    /// </summary>
    public partial class ConfigNewConnection : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private EConnectionType connectonType = EConnectionType.TCPSocketServer;
        public EConnectionType ConnectonType
        {
            get
            {
                return this.connectonType;
            }
            set
            {
                connectonType = value;
                EnableDisableControls();
            }
        }
        public bool Server
        {
            get { return ConnectonType == EConnectionType.TCPSocketServer; }
            set
            {
                ConnectonType = value ? EConnectionType.TCPSocketServer : ConnectonType;
            }
        }
        public bool Client
        {
            get { return ConnectonType == EConnectionType.TCPSocketCient; }
            set
            {
                ConnectonType = value ? EConnectionType.TCPSocketCient : ConnectonType;
            }
        }

        private string iP;
        public string IP
        {
            get { return iP; }
            set
            {
                if (value != IP)
                {
                    iP = value;
                    OnPropertyChanged("IP");
                };
            }
        }
        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                if (value != Port)
                {
                    port = value;
                    OnPropertyChanged("Port");
                };
            }
        }
        private bool multipleConnections;
        public bool MultipleConnections
        {
            get { return multipleConnections; }
            set
            {
                if (value != MultipleConnections)
                {
                    multipleConnections = value;
                    OnPropertyChanged("MultipleConnections");
                };
            }
        }

        public Connection ConnectionObj { get; set; }

        public ConfigNewConnection()
        {
            InitializeComponent();
            DataContext = this;

            ConnectionObj = new Connection();
            SetProperties();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SetConnectionObj();
            Connection.Save(ConnectionObj);
            Close();
        }

        private void SetConnectionObj()
        {
            ConnectionObj.ConnectionType = ConnectonType;
            ConnectionObj.IPAdress = iP;
            ConnectionObj.Port = Port;
            ConnectionObj.MultipleConnections = MultipleConnections;
        }

        private void SetProperties()
        {
            ConnectonType = ConnectionObj.ConnectionType;
            iP = ConnectionObj.IPAdress;
            Port = ConnectionObj.Port;
            MultipleConnections = ConnectionObj.MultipleConnections;
        }

        private void EnableDisableControls()
        {
            switch(ConnectonType)
            {
                case EConnectionType.TCPSocketServer:
                    textBoxIp.IsEnabled = false;
                    textBoxPort.IsEnabled = true;
                    checkBoxMultipleConnections.IsEnabled = true;
                    break;
                case EConnectionType.TCPSocketCient:
                    textBoxIp.IsEnabled = true;
                    textBoxPort.IsEnabled = true;
                    MultipleConnections = false;
                    checkBoxMultipleConnections.IsEnabled = false;
                    break;
            }
        }

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
