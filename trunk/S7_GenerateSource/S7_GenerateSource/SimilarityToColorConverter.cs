using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace S7_GenerateSource
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
                        
                    case BlockSimilarityType.Identical:
                        return new SolidColorBrush(Colors.Black);
                        
                    case BlockSimilarityType.Orphan:
                        return new SolidColorBrush(Colors.Blue);
                        
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
