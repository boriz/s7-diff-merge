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
    public class BlockToMergeEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value is Block)
            {
                Block blk = (Block)value;

                // Allowing to merge all source blocks and standard blocks except special types and blocks compiled from source
                if (blk.Type == PLCBlockType.SourceBlock ||
                    ((  blk.Type == PLCBlockType.DB ||
                        blk.Type == PLCBlockType.FB ||
                        blk.Type == PLCBlockType.FC ||
                        blk.Type == PLCBlockType.OB ||
                        blk.Type == PLCBlockType.UDT
                    ) && 
                        blk.Language != PLCLanguage.SCL &&
                        blk.Language != PLCLanguage.F_CALL &&
                        blk.Language != PLCLanguage.GRAPH &&
                        blk.Language != PLCLanguage.HMI &&
                        blk.Language != PLCLanguage.SYM))
                {
                    switch (blk.Similarity)
                    {
                        case BlockSimilarityType.Different:
                            return Visibility.Visible;

                        case BlockSimilarityType.Identical:
                            return Visibility.Hidden;

                        case BlockSimilarityType.Orphan:
                            return Visibility.Hidden;

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
