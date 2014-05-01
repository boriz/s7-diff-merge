using System.Windows;
using System.Windows.Controls;

namespace Windows_Trend_View
{
    public class MenuBase : UserControl
    {
     
        protected void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected void Test_Action_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
