using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    public class LoginInfoMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is string type)
            {
                switch (type)
                {
                    case "Login":
                        {
                            return (int)values[0];
                        }
                    case "Logout":
                        {
                            return (int)values[1] - (int)values[0];
                        }
                    default:
                        return 0;
                }
            }
            else
            {
                return 0;
            }
         
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
