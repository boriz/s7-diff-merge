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
using System.Windows.Shapes;

namespace S7_DMCToolbox.TrendSelector
{
    /// <summary>
    /// Interaction logic for TrendSelector.xaml
    /// </summary>
    public partial class TrendSelector : Window
    {
        private S7_ViewModel VM;

        public TrendSelector()
        {

            InitializeComponent();
            VM = TryFindResource("VM") as S7_ViewModel;


            //Set title of window to current assembly version number
            Version myVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = this.Title + " v" + myVersion.Major + "." + myVersion.Minor + "." + myVersion.Build;
        }
    }
}
