using System.Windows;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für EditMessages.xaml
    /// </summary>
    public partial class EditMessages : Window
    {
        public EditMessages()
        {
            InitializeComponent();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
