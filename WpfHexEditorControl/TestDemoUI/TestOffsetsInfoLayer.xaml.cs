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

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestOffsetsInfoLayer.xaml
    /// </summary>
    public partial class TestOffsetsInfoLayer : UserControl {
        public TestOffsetsInfoLayer() {
            InitializeComponent();
            infoHoriLayer.StartStepIndex = 0;
            infoHoriLayer.StepsCount = 16;
            infoVerLayer.StartStepIndex = 44546419846419;
            infoVerLayer.StepsCount = 22;
        }
    }
}
