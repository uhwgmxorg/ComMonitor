using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHexEditor.Sample.MVVM {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            DispatcherUnhandledException += (sender, e) => {

            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {

            };
        }
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            //new BootStrapper().Run()
        }
    }
}
