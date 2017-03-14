using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HexMessageViewerControl
{
    /// <summary>
    /// Interaktionslogik für HexMessageContainerUC.xaml
    /// </summary>
    public partial class HexMessageContainerUC : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Dependencie Propertys
        public HexMessageContainerUCAction MyUserControlActionObj
        {
            get
            {
                return (HexMessageContainerUCAction)GetValue(MyUserControlActionProperty);
            }
            set { SetValue(MyUserControlActionProperty, value); }
        }
        public static readonly DependencyProperty MyUserControlActionProperty =
            DependencyProperty.Register("MyUserControlActionObj", typeof(HexMessageContainerUCAction), typeof(HexMessageContainerUC), new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// Constructur
        /// </summary>
        public HexMessageContainerUC()
        {
            InitializeComponent();
            DataContext = this;

            MyUserControlActionObj = new HexMessageContainerUCAction();

            MyUserControlActionObj.AddMessageEvent += AddMessageHandler;
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        /// <summary>
        /// MenuItem_Click_SelectAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_SelectAll(object sender, RoutedEventArgs e)
        {
            foreach (var c in LineStackPanel.Children)
                ((HexMessageUC)c).IsSecected = true;
        }

        /// <summary>
        /// MenuItem_Click_DeselectAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_DeselectAll(object sender, RoutedEventArgs e)
        {
            foreach (var c in LineStackPanel.Children)
                ((HexMessageUC)c).IsSecected = false;
        }

        /// <summary>
        /// MenuItem_Click_Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Clear(object sender, RoutedEventArgs e)
        {
            LineStackPanel.Children.Clear();
        }

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// AddMessageHandler
        /// </summary>
        /// <param name="message"></param>
        /// <param name="direction"></param>
        private void AddMessageHandler(byte[] message, Direction direction)
        {
            AddMessage(message,direction);
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// AddMessage
        /// </summary>
        /// <param name="message"></param>
        /// <param name="direction"></param>
        public void AddMessage(byte[] message, Direction direction)
        {
            LineStackPanel.Children.Add(new HexMessageUC { HexContentByte = message, MessageDirection = direction });
            ScrollViewer1.ScrollToBottom();
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

    /// <summary>
    /// class HexMessageContainerUCAction
    /// for take action on the UC i.e. 
    /// add a message
    /// </summary>
    public class HexMessageContainerUCAction
    {

        public delegate void MessageEventHandler(byte[] message,Direction direction);
        public event MessageEventHandler AddMessageEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public HexMessageContainerUCAction()
        {
        }

        /// <summary>
        /// AddMessage
        /// </summary>
        /// <param name="message"></param>
        /// <param name="direction"></param>
        public void AddMessage(byte[] message,Direction direction)
        {
            if (AddMessageEvent != null) AddMessageEvent(message,direction);
        }
    }

    /// <summary>
    /// class AutoScrollBehavior
    /// for change window size
    /// </summary>
    public static class AutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, AutoScrollPropertyChanged));


        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer != null && (bool)args.NewValue)
            {
                scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
                scrollViewer.ScrollToEnd();
            }
            else
            {
                scrollViewer.LayoutUpdated -= ScrollViewer_SizeChanged;
            }
        }

        private static void ScrollViewer_SizeChanged(object sender, EventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if(scrollViewer != null)
                scrollViewer.ScrollToEnd();
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
    }
}
