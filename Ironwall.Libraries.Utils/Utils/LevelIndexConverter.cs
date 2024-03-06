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
    public sealed class LevelIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ///ComboBox Categories
            ///ADMIN = 0
            ///USER = 1
            ///UNDEFINED = 2

            if (value != null)
            {

                switch ((EnumLevel)value)
                {
                    case EnumLevel.UNDEFINED:
                        return 0;
                    case EnumLevel.ADMIN:
                        return 1;
                    case EnumLevel.USER:
                        return 2;
                    default:
                        return 0;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value != null)
			{
				switch (value)
				{
                    case (int)EnumLevel.ADMIN:
                        return EnumLevel.ADMIN;

                    case (int)EnumLevel.USER:
                        return EnumLevel.USER;

                    case (int)EnumLevel.UNDEFINED:
                        return EnumLevel.UNDEFINED;

                    default:
                        return EnumLevel.UNDEFINED;
                }
			}
			else
			{
                return EnumLevel.UNDEFINED;
            }
        }
    }
}
