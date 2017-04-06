using ComMonitor.LocalTools;
using ComMonitor.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WPFHexaEditor.Control;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für EditMessages.xaml
    /// </summary>
    public partial class EditMessages : Window
    {

        private Logger _logger;

        public int SelectedTabItemsIndex { get; set; }
        private ObservableCollection<TabItem> tabItems;
        public ObservableCollection<TabItem> TabItems
        {
            get
            {
                return tabItems;
            }
            set
            {
                tabItems = value;
            }
        }
        private TabItem _tabAdd;

        private List<Message> messagesToEdit;
        public List<Message> MessagesToEdit
        {
            get
            {
                return messagesToEdit;
            }
            set
            {
                messagesToEdit = value;
                for (int i = 0; i < messagesToEdit.Count; i++)
                    _tabAdd = AddTabItem(messagesToEdit[i]);
                SelectedTabItemsIndex = messagesToEdit.FindIndex(b => b.Content == FocusMessage);
            }
        }

        public byte[] FocusMessage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EditMessages()
        {
            _logger = LogManager.GetCurrentClassLogger();

            InitializeComponent();

            tabItems = new ObservableCollection<TabItem>();
            SelectedTabItemsIndex = 0;

            DataContext = this;
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
            int i = 0;
            DialogResult = true;

            try
            {
                foreach (var t in TabItems)
                {
                    t.HexEditor.SubmitChanges();
                    MessagesToEdit[i].MessageName = t.Header;
                    MessagesToEdit[i].Content = t.HexEditor.Stream.ToArray();
                    i++;
                }
                FocusMessage = MessagesToEdit[tabHexaEditors.SelectedIndex].Content;
            }
            catch (Exception ex)
            {
                _logger.Error(String.Format("Exception in {0} {1}", LST.GetCurrentMethod(), ex.Message));
            }

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
        /// AddTabItem
        /// </summary>
        /// <param name="v"></param>
        private TabItem AddTabItem(Message m)
        {
            int count = TabItems.Count;

            TabItem tab = new TabItem();
            tab.Header = m.MessageName;
            tab.HexEditor = new HexaEditor();
            tab.HexEditor.Width = Double.NaN;
            tab.HexEditor.Height = Double.NaN;
            tab.HexEditor.Stream = new System.IO.MemoryStream(m.Content);
            TabItems.Add(tab);

            _logger.Trace(String.Format("AddTabItem in {0}", LST.GetCurrentMethod()));

            return tab;
        }

        #endregion
    }

    public sealed class TabItem
    {
        public string Header { get; set; }
        public HexaEditor HexEditor { get; set; }
    }

    [TemplatePart(Name = "PART_TabHeader", Type = typeof(TextBox))]
    public class EditableTabHeaderControl : ContentControl
    {
        private delegate void FocusTextBox();

        /// <summary>
        /// Dependency property to bind EditMode with XAML Trigger
        /// Gets or sets a value indicating whether this instance is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)this.GetValue(IsInEditModeProperty);
            }
            set
            {
                if (string.IsNullOrEmpty(this._textBox.Text))
                {
                    this._textBox.Text = this._oldText;
                }

                this._oldText = this._textBox.Text;
                this.SetValue(IsInEditModeProperty, value);
            }
        }
        private static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTabHeaderControl));

        private TextBox _textBox;
        private string _oldText;
        private DispatcherTimer _timer;


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._textBox = this.Template.FindName("PART_TabHeader", this) as TextBox;
            if (this._textBox != null)
            {
                this._timer = new DispatcherTimer();
                this._timer.Tick += TimerTick;
                this._timer.Interval = TimeSpan.FromMilliseconds(1);
                this.LostFocus += TextBoxLostFocus;
                this._textBox.KeyDown += TextBoxKeyDown;
                this.MouseDoubleClick += EditableTabHeaderControlMouseDoubleClick;
            }
        }

        /// <summary>
        /// Sets the IsInEdit mode.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetEditMode(bool value)
        {
            this.IsInEditMode = value;
            this._timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this._timer.Stop();
            this.MoveTextBoxInFocus();
        }

        private void MoveTextBoxInFocus()
        {
            if (this._textBox.CheckAccess())
            {
                if (!string.IsNullOrEmpty(this._textBox.Text))
                {
                    this._textBox.CaretIndex = 0;
                    this._textBox.Focus();
                }
            }
            else
            {
                this._textBox.Dispatcher.BeginInvoke(DispatcherPriority.Render, new FocusTextBox(this.MoveTextBoxInFocus));
            }
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this._textBox.Text = _oldText;
                this.IsInEditMode = false;
            }
            else if (e.Key == Key.Enter)
            {
                this.IsInEditMode = false;
            }
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
        }

        private void EditableTabHeaderControlMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetEditMode(true);
            }
        }
    }
                 
}
