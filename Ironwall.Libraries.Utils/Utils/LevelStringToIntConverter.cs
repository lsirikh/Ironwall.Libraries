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
    public sealed class LevelStringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((EnumLevel)value)
            {
                case EnumLevel.UNDEFINED:
                    return "UNDEFINED";
                case EnumLevel.ADMIN:
                    return "ADMIN";
                case EnumLevel.USER:
                    return "USER";
                default:
                    return "UNDEFINED";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == EnumLevel.USER.ToString())
            {
                return (int)EnumLevel.USER;
            }
            else if (value.ToString() == EnumLevel.ADMIN.ToString())
            {
                return (int)EnumLevel.ADMIN;
            }
            else if (value.ToString() == EnumLevel.UNDEFINED.ToString())
            {
                return (int)EnumLevel.UNDEFINED;
            }
            else
            {
                return (int)EnumLevel.UNDEFINED;
            }
        }
    }
}
