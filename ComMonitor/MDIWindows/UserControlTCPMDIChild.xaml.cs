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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComMonitor.MDIWindows
{
    /// <summary>
    /// Interaktionslogik für UserControlTCPMDIChild.xaml
    /// </summary>
    public partial class UserControlTCPMDIChild : UserControl
    {
        public UserControlTCPMDIChild()
        {
            InitializeComponent();
            HexEdit.FileName = "ChangeLog.txt";
        }
    }
}
