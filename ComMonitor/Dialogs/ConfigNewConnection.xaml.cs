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

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigNewConnection()
        {
            InitializeComponent();
            DataContext = this;

            ConnectionObj = new Connection();
            SetProperties();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_Click_Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Button_Click_Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SetConnectionObj();
            Close();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// SetConnectionObj
        /// </summary>
        private void SetConnectionObj()
        {
            ConnectionObj.ConnectionType = ConnectonType;
            ConnectionObj.IPAdress = iP;
            ConnectionObj.Port = Port;
            ConnectionObj.MultipleConnections = MultipleConnections;
        }

        /// <summary>
        /// SetProperties
        /// </summary>
        private void SetProperties()
        {
            ConnectonType = ConnectionObj.ConnectionType;
            iP = ConnectionObj.IPAdress;
            Port = ConnectionObj.Port;
            MultipleConnections = ConnectionObj.MultipleConnections;
        }

        /// <summary>
        /// EnableDisableControls
        /// </summary>
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

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        #endregion
    }
}
