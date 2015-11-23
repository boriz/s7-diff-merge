using System;
using System.Globalization;
using System.Windows.Data;

namespace S7_DMCToolbox.ValueConverters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class TypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter is Type)
            {
                return !(value.GetType() == (Type) parameter);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
