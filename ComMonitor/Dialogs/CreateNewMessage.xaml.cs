using ComMonitor.LocalTools;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für CreateNewMessage.xaml
    /// </summary>
    public partial class CreateNewMessage : Window
    {

        private Logger _logger;

        public byte[] FocusMessage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CreateNewMessage()
        {
            _logger = LogManager.GetCurrentClassLogger();
            InitializeComponent();

            textBox_MessageSize.Text = "32";
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_Click_Create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            int size = 0;

            try
            {
                size = Int32.Parse(textBox_MessageSize.Text);
            }
            catch (Exception)
            {
                Console.Beep();
                return;
            }

            byte[] v = new byte[size];
            HexEdit.Stream = new System.IO.MemoryStream(v);
        }

        /// <summary>
        /// Button_Click_Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            HexEdit.Stream = new System.IO.MemoryStream(0);
        }

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
            try
            {
                DialogResult = true;
                FocusMessage = new byte[HexEdit.Stream.Length];
                HexEdit.SubmitChanges();
                FocusMessage = HexEdit.Stream.ToArray();
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

        #endregion
    }
}
