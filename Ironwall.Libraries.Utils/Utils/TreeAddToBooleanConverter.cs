using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    public class TreeAddToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EnumTreeType inputData))
                return null;

            var param = parameter.ToString();

            if (param.Contains(value.ToString()))
                return true;
            else
                return false;

            /*if (inputData == EnumTreeType.ROOT
                || inputData == EnumTreeType.BRANCH)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
