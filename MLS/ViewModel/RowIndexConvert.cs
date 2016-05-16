using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MLS.ViewModel
{
    public class RowIndexConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var lvb = value as ListViewItem;
            int ordinal = 0;
            var listView = ItemsControl.ItemsControlFromItemContainer(lvb) as ListView;
            ordinal = listView.ItemContainerGenerator.IndexFromContainer(lvb) + 1;
            return ordinal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
