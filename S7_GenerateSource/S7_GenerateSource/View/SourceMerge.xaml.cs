using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S7_GenerateSource
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SourceMerge : Window
    {
        private S7_ViewModel VM;

        public SourceMerge()
        {

            InitializeComponent();
            VM = TryFindResource("VM") as S7_ViewModel;
            VM.InitFromCommandLineArguments(App.StartupArgs);

            Closing += VM.OnClosing;
        }
    }
}
