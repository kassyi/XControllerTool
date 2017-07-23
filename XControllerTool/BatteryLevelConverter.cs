using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Reactive.Bindings;

namespace XControllerTool
{
    class BatteryLevelConverter : IValueConverter
    {
        static readonly FontAwesomeIcon[] icon = new FontAwesomeIcon[] {
            FontAwesomeIcon.Battery0,
            FontAwesomeIcon.Battery1,
            FontAwesomeIcon.Battery3,
            FontAwesomeIcon.Battery4};

        public object Convert(object v, Type targetType, object parameter, CultureInfo culture)
        {
            if (v is byte level && level >= 0 && level <= 3)
                return icon[level];
            else
                return icon[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
