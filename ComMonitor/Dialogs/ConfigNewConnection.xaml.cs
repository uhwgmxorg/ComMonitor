using System.Windows;

namespace ComMonitor.Dialogs
{
    /// <summary>
    /// Interaktionslogik für ConfigNewConnection.xaml
    /// </summary>
    public partial class ConfigNewConnection : Window
    {
        public ConfigNewConnection()
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
