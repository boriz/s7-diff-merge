using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using S7_DMCToolbox.Base.UserInterface;

namespace S7_DMCToolbox.ValueConverters
{
    [ValueConversion(typeof(IViewModel), typeof(UserControl))]
    public class ViewModelToViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ScreenManager.Screens != null
                       ? ScreenManager.Screens.FirstOrDefault(s => s.DataContext == value)
                       : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}