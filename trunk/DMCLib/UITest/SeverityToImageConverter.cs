using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UITest
{
    [ValueConversion(typeof(LogEvent.Severity), typeof(string))]
    public class SeverityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is LogEvent.Severity)
            {
                switch ((LogEvent.Severity)value)
                {
                    case LogEvent.Severity.Info:
                        return "/UITest;component/109_AllAnnotations_Info_24x24_72.png";
                    case LogEvent.Severity.Warning:
                        return "/UITest;component/109_AllAnnotations_Warning_24x24_72.png";
                    case LogEvent.Severity.Error:
                        return "/UITest;component/109_AllAnnotations_Error_24x24_72.png";
                    default:
                        return "/UITest;component/109_AllAnnotations_Info_24x24_72.png";
                }
                
            }
            return "/UITest;component/109_AllAnnotations_Info_24x24_72.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
