using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;


namespace S7_GenerateSource
{
    [ValueConversion(typeof(Block), typeof(Visibility))]
    public class BlockToCopyEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value is Block)
            {
                Block blk = (Block)value;

                // Allow to copy all source blocks and all other blocks except safety call, symbols and HMIs
                if (blk.Type == PLCBlockType.SourceBlock ||
                        (blk.Language != PLCLanguage.F_CALL && 
                        blk.Language != PLCLanguage.SYM &&
                        blk.Language != PLCLanguage.HMI &&
                        blk.Language != PLCLanguage.HWC))
                {
                    switch (blk.Similarity)
                    {
                        case BlockSimilarityType.Different:
                            return Visibility.Visible;

                        case BlockSimilarityType.Identical:
                            return Visibility.Hidden;

                        case BlockSimilarityType.Orphan:
                            return Visibility.Visible;

                        default:
                            return Visibility.Hidden;
                    }
                }
            }
            return Visibility.Hidden;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
