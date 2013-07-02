using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace S7_DMCToolbox
{
    [ValueConversion(typeof(BlockSimilarityType), typeof(SolidColorBrush))]
    public class SimilarityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value is BlockSimilarityType)
            {
                switch ((BlockSimilarityType)value)
                {
                    case BlockSimilarityType.Different:
                        return  new SolidColorBrush(Colors.Red);
                        break;
                    case BlockSimilarityType.Identical:
                        return new SolidColorBrush(Colors.Black);
                        break;
                    case BlockSimilarityType.Orphan:
                        return new SolidColorBrush(Colors.Blue);
                        break;
                    default:
                        return Colors.Black;
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
