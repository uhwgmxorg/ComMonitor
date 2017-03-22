using System.Windows;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für EditMessages.xaml
    /// </summary>
    public partial class EditMessages : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EditMessages()
        {
            InitializeComponent();
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
