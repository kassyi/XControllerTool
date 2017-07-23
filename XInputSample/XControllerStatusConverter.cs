using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XControllerTool
{
    class XControllerStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int && values[1] is bool)
            {
                var controllerNo = (int)values[0] + 1;
                var isConnected = (bool)values[1] ? "Connected" : "Disconnected";
                return $"Controller{controllerNo}: {isConnected}";
            }
            else
                return "nodata";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
