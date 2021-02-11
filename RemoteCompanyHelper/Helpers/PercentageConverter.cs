using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteCompanyHelper.Helpers
{
    public class PercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            Console.WriteLine(System.Convert.ToDouble(values[0]));
            Console.WriteLine(System.Convert.ToDouble(values[1]));
            return System.Convert.ToDouble(values[0]) *
                   System.Convert.ToDouble(values[1]);
        }

        public object[] ConvertBack(object value,
            Type[] targetTypes,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
