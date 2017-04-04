using ComMonitor.LocalTools;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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

        private List<byte[]> messagesToEdit;
        public List<byte[]> MessagesToEdit
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
                SelectedTabItemsIndex = messagesToEdit.FindIndex(b => b == FocusMessage);
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
                    MessagesToEdit[i++] = t.HexEditor.Stream.ToArray();
                }
                FocusMessage = MessagesToEdit[tabHexaEditors.SelectedIndex];
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
        private TabItem AddTabItem(byte[] v)
        {
            int count = TabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();

            tab.Header = String.Format("Message {0}", count + 1);
            tab.HexEditor = new HexaEditor();
            tab.HexEditor.Width = Double.NaN;
            tab.HexEditor.Height = Double.NaN;
            tab.HexEditor.Stream = new System.IO.MemoryStream(v);

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
}
