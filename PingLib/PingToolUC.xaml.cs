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

namespace PingLib
{
    /// <summary>
    /// Interaktionslogik für PingToolUC.xaml
    /// </summary>
    public partial class PingToolUC : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer _dispatcherTimer = null;
        int _counter = 0;

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

        private string selectedIp;
        public string SelectedIp
        {
            get { return selectedIp; }
            set { SetField(ref this.selectedIp, value, nameof(SelectedIp)); }
        }
        private ObservableCollection<string> ipList;
        public ObservableCollection<string> IpList
        {
            get { return ipList; }
            set { SetField(ref this.ipList, value, nameof(IpList)); }
        }
        public string NewItem
        {
            set
            {
                if (SelectedIp != null)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(value))
                {
                    ipList.Add(value);
                    SelectedIp = value;
                }
            }
        }

        #endregion

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
            Series = new SeriesCollection(config) { new ColumnSeries { Values = new ChartValues<ChartDataModel> { new ChartDataModel() } } };
            Series[0].Values.Clear();
            ((Series)Series[0]).Fill = new SolidColorBrush(Color.FromArgb(255, (byte)153, (byte)180, (byte)211));
            Formatter = value => DateTime.Now.ToString("d/M/yyyy HH:mm:ss");

            Button_Stop.IsEnabled = false;
            Button_Start.IsEnabled = true;

            IpList = new ObservableCollection<string>();
            IpList.Add("127.0.0.1");
            SelectedIp = IpList[0];
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
            Series[0].Values.Add(new ChartDataModel { DateTime = System.DateTime.Now.AddHours(++_counter), Value = LocalPing.Ping(SelectedIp) });
            Debug.WriteLine(String.Format("{0} {1}",((ChartDataModel)Series[0].Values[Series[0].Values.Count-1]).DateTime, ((ChartDataModel)Series[0].Values[Series[0].Values.Count - 1]).Value));
            if (Series[0].Values.Count > 30)
                Series[0].Values.RemoveAt(0);
        }

        /// <summary>
        /// UserControl_Unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
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
