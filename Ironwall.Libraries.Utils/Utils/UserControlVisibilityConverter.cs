using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    public class UserControlVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            /// CONTROLLER
            /// SENSOR
            /// IPCAMERA
            /// 

            Visibility returnValue = Visibility.Hidden;

            switch ((EnumDeviceType)value)
            {
                case EnumDeviceType.NONE:
                    break;
                case EnumDeviceType.Controller:
                case EnumDeviceType.Cable:
                    {
                        if((string)parameter =="CONTROLLER")
                            returnValue = Visibility.Visible;
                        else
                            returnValue = Visibility.Hidden;
                    }
                    break;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                    {
                        if ((string)parameter == "SENSOR")
                            returnValue = Visibility.Visible;
                        else
                            returnValue = Visibility.Hidden;
                    }
                    break;
                case EnumDeviceType.IpCamera:
                    {
                        if ((string)parameter == "IPCAMERA")
                            returnValue = Visibility.Visible;
                        else
                            returnValue = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
