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
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace PingLib
{
    /// <summary>
    /// Interaktionslogik für PingToolUC.xaml
    /// </summary>
    public partial class PingToolUC : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler IpChange;

        private DispatcherTimer _dispatcherTimer = null;
        private int _counter = 0;
        private double _lastValue = 1.0;
        private double _minTime = Double.MaxValue;
        private double _totalOfTime = 0.0;
        private double _sumVar = 0.0;

        #region INotifyPropertyChanged Properties
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }

        private ZoomingOptions zoomingMode;
        public ZoomingOptions ZoomingMode
        {
            get { return zoomingMode; }
            set { SetField(ref this.zoomingMode, value, nameof(ZoomingMode)); }
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
                    NewPingTarget(SelectedIp);
                }
            }
        }

        private int numberOfPings;
        public int NumberOfPings
        {
            get { return numberOfPings; }
            set
            {
                SetField(ref numberOfPings, value, nameof(NumberOfPings));
                int toDelete = Series[0].Values.Count - numberOfPings;
                if (numberOfPings > 0)
                {
                    for(int i=1; i<=toDelete; i++)
                    {
                        Series[0].Values.RemoveAt(0);
                        Series[1].Values.RemoveAt(0);
                        Counts = Series[0].Values.Count;
                    }
                }
            }
        }
        private int counts;
        public int Counts
        {
            get { return counts; }
            set { SetField(ref counts, value, nameof(Counts)); }
        }
        private double success;
        public double Success
        {
            get { return success; }
            set { SetField(ref success, value, nameof(Success)); }
        }
        private int fail;
        public int Fail
        {
            get { return fail; }
            set { SetField(ref fail, value, nameof(Fail)); }
        }
        private int total;
        public int Total
        {
            get { return total; }
            set { SetField(ref total, value, nameof(Total)); }
        }
        private double maxTime;
        public double MaxTime
        {
            get { return maxTime; }
            set { SetField(ref maxTime, value, nameof(MaxTime)); }
        }
        private double minTime;
        public double MinTime
        {
            get { return minTime; }
            set { SetField(ref minTime, value, nameof(MinTime)); }
        }
        private double currentTime;
        public double CurrentTime
        {
            get { return currentTime; }
            set { SetField(ref currentTime, value, nameof(CurrentTime)); }
        }
        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set { SetField(ref startTime, value, nameof(StartTime)); }
        }
        private DateTime stopTime;
        public DateTime StopTime
        {
            get { return stopTime; }
            set { SetField(ref stopTime, value, nameof(StopTime)); }
        }
        private TimeSpan duration;
        public TimeSpan Duration
        {
            get { return duration; }
            set { SetField(ref duration, value, nameof(Duration)); }
        }
        private string startTimeStr;
        public string StartTimeStr
        {
            get { return startTimeStr; }
            set { SetField(ref startTimeStr, value, nameof(StartTimeStr)); }
        }
        private string stopTimeStr;
        public string StopTimeStr
        {
            get { return stopTimeStr; }
            set { SetField(ref stopTimeStr, value, nameof(StopTimeStr)); }
        }
        private string durationStr;
        public string DurationStr
        {
            get { return durationStr; }
            set { SetField(ref durationStr, value, nameof(DurationStr)); }
        }
        private double average;
        public double Average
        {
            get { return average; }
            set { SetField(ref average, value, nameof(Average)); }
        }
        private double variance;
        public double Variance
        {
            get { return variance; }
            set { SetField(ref variance, value, nameof(Variance)); }
        }
        private double standardDeviation;
        public double StandardDeviation
        {
            get { return standardDeviation; }
            set { SetField(ref standardDeviation, value, nameof(StandardDeviation)); }
        }
        private string averageStr;
        public string AverageStr
        {
            get { return averageStr; }
            set { SetField(ref averageStr, value, nameof(AverageStr)); }
        }
        private string varianceStr;
        public string VarianceStr
        {
            get { return varianceStr; }
            set { SetField(ref varianceStr, value, nameof(VarianceStr)); }
        }
        private string standardDeviationStr;
        public string StandardDeviationStr
        {
            get { return standardDeviationStr; }
            set { SetField(ref standardDeviationStr, value, nameof(StandardDeviationStr)); }
        }
        #endregion

        public IpListToXml ItemListToXml { get; set; }

        public Func<double, string> Formatter { get; set; }

        public String PingTarget
        {
            get { return (String)GetValue(PingTargetProperty); }
            set { SetValue(PingTargetProperty, value); }
        }
        public static readonly DependencyProperty PingTargetProperty =
            DependencyProperty.Register("PingTarget", typeof(string), typeof(PingToolUC), new PropertyMetadata(""));

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

            ZoomingMode = ZoomingOptions.X;

            Button_Stop.IsEnabled = false;
            Button_Start.IsEnabled = true;

            ItemListToXml = new IpListToXml();
            IpList = ItemListToXml.Load(ref selectedIp);
            SelectedIp = selectedIp;
            NewPingTarget(SelectedIp);

            NumberOfPings = 30;

            Button_Clear_Click(null,null);
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

            SetStartTime();
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

            SetStopTime();
        }

        /// <summary>
        /// Button_Clear_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Counts = 0;
            Series[0].Values.Clear();
            Series[1].Values.Clear();
            Success = 0;
            Fail = 0;
            Total = 0;
            MaxTime = 0;
            _minTime = Double.MaxValue;
            MinTime = 0;

            _totalOfTime = 0.0;
            _sumVar = 0.0;

            Variance = 0.0;
            Average = 0.0;
            StandardDeviation = 0.0;
            VarianceStr = String.Format("Variance: -");
            AverageStr = String.Format("Average: -");
            StandardDeviationStr = String.Format("Standard Deviation: -");

            SetStartTime();
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
        /// UserControl_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NewPingTarget(SelectedIp);
        }

        /// <summary>
        /// Chart_MouseDoubleClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Reset zoom and pan
            chart.AxisX[0].MinValue = double.NaN;
            chart.AxisX[0].MaxValue = double.NaN;
            chart.AxisY[0].MinValue = double.NaN;
            chart.AxisY[0].MaxValue = double.NaN;
        }

        /// <summary>
        /// DispatcherTimer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            double pingResult = 0.0;
            double blueValue;
            double redValue;

            pingResult = await LocalPing.PingAsync(SelectedIp);
            ++_counter;
            if(pingResult > 0)
                Total++;
            Success = Total - Fail;

            if (pingResult > 0)
            {
                _lastValue = pingResult;
                blueValue = pingResult;
                redValue = 0.0;
            }
            else
            {
                blueValue = 0.0;
                redValue = _lastValue;
                Fail++;
                Success = Total - Fail;
            }

            CurrentTime = pingResult;
            if (pingResult > MaxTime) MaxTime = pingResult;
            if (pingResult > 0)
            {
                if (pingResult < _minTime) _minTime = pingResult;
                if (_minTime != Double.MaxValue) MinTime = _minTime;
            }
            Duration = DateTime.Now - StopTime;
            DurationStr = String.Format("Duration: {0:dd\\.hh\\:mm\\:ss}", Duration);
            if (pingResult > 0)
            {
                _totalOfTime = _totalOfTime + pingResult;
                Average = _totalOfTime / Total;
                _sumVar += Math.Pow(pingResult - Average, 2);
                Variance = (1.0 / (double)Total) * _sumVar;
                StandardDeviation = Math.Sqrt(Variance);
            }
            AverageStr = String.Format("Average: {0:0.00} ms", Average);
            VarianceStr = String.Format("Variance: {0:0.00}", Variance);
            StandardDeviationStr = String.Format("Standard Deviation: {0:0.00}", StandardDeviation);

            Series[0].Values.Add(new ChartDataModel { DateTime = System.DateTime.Now.AddHours(_counter), Value = blueValue });
            //Debug.WriteLine(String.Format("{0} {1}", ((ChartDataModel)Series[0].Values[Series[0].Values.Count - 1]).DateTime, ((ChartDataModel)Series[0].Values[Series[0].Values.Count - 1]).Value));
            if (Series[0].Values.Count > NumberOfPings && numberOfPings > 0)
                Series[0].Values.RemoveAt(0);

            Series[1].Values.Add(new ChartDataModel { DateTime = System.DateTime.Now.AddHours(_counter), Value = redValue });
            //Debug.WriteLine(String.Format("{0} {1}", ((ChartDataModel)Series[1].Values[Series[1].Values.Count - 1]).DateTime, ((ChartDataModel)Series[1].Values[Series[1].Values.Count - 1]).Value));
            if (Series[1].Values.Count > NumberOfPings && numberOfPings > 0)
                Series[1].Values.RemoveAt(0);

            //Debug.WriteLine(String.Format("Interval = {0}", Interval));

            Counts = Series[0].Values.Count;
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
        /// SetStartTime
        /// </summary>
        private void SetStartTime()
        {
            StartTime = DateTime.Now;
            StopTime = DateTime.Now;
            Duration = StopTime - StartTime;
            StartTimeStr = String.Format("Start Time: {0}", StartTime);
            if (StartTime == StopTime)
                StopTimeStr = String.Format("Stop Time: -");
            else
                StopTimeStr = String.Format("Stop Time: {0}", StopTime);
            DurationStr = String.Format("Duration: {0}", Duration);

            Variance = 0.0;
            Average = 0.0;
            StandardDeviation = 0.0;
            VarianceStr = String.Format("Variance: -");
            AverageStr = String.Format("Average: -");
            StandardDeviationStr = String.Format("Standard Deviation: -");
        }

        /// <summary>
        /// SetStartStopTime
        /// </summary>
        private void SetStopTime()
        {
            StopTime = DateTime.Now;
            Duration = StopTime - StartTime;
            StartTimeStr = String.Format("Start Time: {0}", StartTime);
            if(StartTime == StopTime)
                StopTimeStr = String.Format("Stop Time: -");
            else
                StopTimeStr = String.Format("Stop Time: {0}", StopTime);
            DurationStr = String.Format("Duration: {0}", Duration);
        }

        /// <summary>
        /// NewPingTarget
        /// </summary>
        /// <param name="conState"></param>
        private void NewPingTarget(string selectedIp)
        {
            PingTarget = String.Format("Ping to {0}", selectedIp);
            IpChange?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// GetParent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public T GetParent<T>(System.Windows.DependencyObject child) where T : System.Windows.DependencyObject
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

    #region Converter Helper Class

    public class ZoomingModeCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ZoomingOptions)value)
            {
                case ZoomingOptions.None:
                    return "None";
                case ZoomingOptions.X:
                    return "X";
                case ZoomingOptions.Y:
                    return "Y";
                case ZoomingOptions.Xy:
                    return "XY";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
