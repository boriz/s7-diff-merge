using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Windows_Display_App
{
    public class EnumComboItem<T> : ComboBoxItem where T : struct, IComparable, IFormattable, IConvertible
    {
        public EnumComboItem()
        {
            this.SetResourceReference(StyleProperty, typeof(ComboBoxItem));
        }
        public T Value { get; set; }
    }
}
