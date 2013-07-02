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

namespace S7_DMCToolbox
{
    /// <summary>
    /// Interaction logic for AlarmWorxEntry.xaml
    /// </summary>
    public partial class AlarmWorxEntry : Window
    {
        public AlarmWorxEntry()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
