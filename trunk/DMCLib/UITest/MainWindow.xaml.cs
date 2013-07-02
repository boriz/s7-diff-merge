using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DMC.UI;
using DMC.UI.Auth;
using DMCBase;
using NLog;

namespace UITest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private System.Threading.Timer timer;
        private Random rand;
        

        public MainWindow()
        {
            InitializeComponent();

            this.rand = new Random();
            this.timer = new System.Threading.Timer(TmrCallback, null, 0, 1000);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TmrCallback(object state)
        {
            LogLevel level;

            switch (this.rand.Next(5))
            {
                case 0:
                    level = LogLevel.Debug;
                    break;
                case 1:
                    level = LogLevel.Error;
                    break;
                case 2:
                    level = LogLevel.Fatal;
                    break;
                case 3:
                    level = LogLevel.Info;
                    break;
                case 4:
                    level = LogLevel.Trace;
                    break;
                default:
                    level = LogLevel.Warn;
                    break;
            }

            logger.Log(level, "Test");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            bool? result = new UserLoginDialog().ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            //this.CMonster.FireEvent("About Window Requested");
        }

        private void Progress_Click(object sender, RoutedEventArgs e)
        {
            //this.ProgressPanel.Visibility = System.Windows.Visibility.Visible;
            //this.IsBusy = true;
        }

        private void ProgressPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.ProgressPanel.Visibility = System.Windows.Visibility.Hidden;
            //this.IsBusy = false;
        }
    }
}
