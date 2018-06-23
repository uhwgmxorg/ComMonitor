using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using PingLib.Models;
using System.Windows.Media;
using PingLib.LocalTools;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using System.Collections.ObjectModel;
using System.Linq;

namespace PingLib
{
    /// <summary>
    /// Interaktionslogik für PingToolUC.xaml
    /// </summary>
    public partial class PingToolUC : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer _dispatcherTimer = null;
        private int _counter = 0;
        private double _lastValue = 0;

        #region INotifyPropertyChanged Properties
        private ColumnSeries measurements;
        public ColumnSeries Measurements
        {
            get { return measurements; }
            set { SetField(ref measurements, value, nameof(Measurements)); }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }

        private SeriesCollection series;
        public SeriesCollection Series
        {
            get { return series; }
            set { SetField(ref this.series, value, nameof(Series)); }
        }

        private ObservableCollection<string> ipList;
        public ObservableCollection<string> IpList
        {
            get { return ipList; }
            set { SetField(ref this.ipList, value, nameof(IpList)); }
        }
        private string newIp;
        public string NewIp
        {
            get
            {
                return newIp;
            }
            set
            {
                if (newIp != value)
                {
                    newIp = value;
                    var item = IpList.SingleOrDefault(x => x == newIp);
                    if (item == null)
                        IpList.Insert(0, newIp);
                    SelectedIp = newIp;
                }
            }
        }
        private string selectedIp;
        public string SelectedIp
        {
            get
            {
                return selectedIp;
            }
            set
            {
                if (selectedIp != value)
                {
                    selectedIp = value;
                    if (selectedIp == IpListToXml.DELETE_COMMAND)
                    {
                        IpList.Clear();
                        IpList.Add(IpListToXml.DELETE_COMMAND);
                    }
                    SetField(ref this.selectedIp, value, nameof(SelectedIp));
                }
            }
        }
        #endregion

        public IpListToXml ItemListToXml { get; set; }


        public Func<double, string> Formatter { get; set; }

        /// <summary>
        /// Constuctor
        /// </summary>
        public PingToolUC()
        {
            InitializeComponent();
            DataContext = this;

            // Init chart
            var config = Mappers.Xy<ChartDataModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(dayModel => dayModel.Value);
            Series = new SeriesCollection(config);

            Series.Add(new ColumnSeries { Values = new ChartValues<ChartDataModel> { new ChartDataModel() } });
            ((Series)Series[0]).Fill = new SolidColorBrush(Color.FromArgb(255, (byte)153, (byte)180, (byte)211));
            Series[0].Values.Clear();

            Series.Add(new ColumnSeries { Values = new ChartValues<ChartDataModel> { new ChartDataModel() } });
            ((Series)Series[1]).Fill = new SolidColorBrush(Color.FromArgb(255, (byte)245, (byte)22, (byte)22));
            Series[1].Values.Clear();

            Formatter = value => DateTime.Now.ToString("d/M/yyyy HH:mm:ss");

            Button_Stop.IsEnabled = false;
            Button_Start.IsEnabled = true;

            ItemListToXml = new IpListToXml();
            IpList = ItemListToXml.Load(ref selectedIp);
            SelectedIp = selectedIp;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PingToolUC()
        {
            ItemListToXml.Save(SelectedIp, IpList);
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_Start_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_dispatcherTimer != null)
                return;
            // Init timer
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            _dispatcherTimer.Start();
            Button_Stop.IsEnabled = true;
            Button_Start.IsEnabled = false;
        }

        /// <summary>
        /// Button_Stop_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_dispatcherTimer == null)
                return;
            // Init timer
            _dispatcherTimer.Stop();
            _dispatcherTimer.Tick -= new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer = null;
            Button_Stop.IsEnabled = false;
            Button_Start.IsEnabled = true;
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

        /// <summary>
        /// DispatcherTimer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            double pingResult;

            _lastValue = pingResult = LocalPing.Ping(SelectedIp);
            ++_counter;

            Series[0].Values.Add(new ChartDataModel { DateTime = System.DateTime.Now.AddHours(_counter), Value = pingResult });
            Debug.WriteLine(String.Format("{0} {1}", ((ChartDataModel)Series[0].Values[Series[0].Values.Count - 1]).DateTime, ((ChartDataModel)Series[0].Values[Series[0].Values.Count - 1]).Value));
            if (Series[0].Values.Count > 30)
                Series[0].Values.RemoveAt(0);

            Series[1].Values.Add(new ChartDataModel { DateTime = System.DateTime.Now.AddHours(_counter), Value = _lastValue });
            Debug.WriteLine(String.Format("{0} {1}", ((ChartDataModel)Series[1].Values[Series[1].Values.Count - 1]).DateTime, ((ChartDataModel)Series[1].Values[Series[1].Values.Count - 1]).Value));
            if (Series[1].Values.Count > 30)
                Series[1].Values.RemoveAt(0);
        }

        /// <summary>
        /// ComboBox_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                string newItemValue = ((System.Windows.Controls.TextBox)e.OriginalSource).Text;
                var item = IpList.SingleOrDefault(x => x == newItemValue);
                if (item == null)
                    IpList.Insert(0, newItemValue);
            }
        }

        /// <summary>
        /// UserControl_Unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemListToXml.Save(SelectedIp, IpList);

            if (_dispatcherTimer == null) return;
            _dispatcherTimer.Tick -= new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Stop();
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// GetParent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T GetParent<T>(System.Windows.DependencyObject child) where T : System.Windows.DependencyObject
        {

            System.Windows.DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);

            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;
                if (parent != null)
                    return parent;
                else
                    return GetParent<T>(dependencyObject);
            }
            else
                return null;
        }

        /// <summary>
        /// SetField
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
