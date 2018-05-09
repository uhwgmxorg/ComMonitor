using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHexEditor.Sample.MVVM.Helpers
{
    public static class AppHelper
    {
        public static string FindResourceString(string keyName) {
            try {
                var res = Application.Current.TryFindResource(keyName);
                return res as string;
            }
            catch {
                return null;
            }
            
        }
    }
}
