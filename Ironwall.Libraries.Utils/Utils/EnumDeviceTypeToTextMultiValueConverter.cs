using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    public sealed class EnumDeviceTypeToTextMultiValueConverter
        : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null || values[1] == null)
                return null;

            if (!(values.Count() > 0))
                return null;

            if (targetType.Name != "String")
                return null;

            try
            {
                var type = values[1].ToString();

                switch (type)
                {
                    case "CONTROLLER":
                        return $"[{values[0]}]{EnumDeviceType.Controller}";
                    case "FENCE_SENSOR":
                        return $"[{values[0]}]{EnumDeviceType.Fence}";
                    case "MULTI_SNESOR":
                        return $"[{values[0]}]{EnumDeviceType.Multi}";
                    case "PIR_SENSOR":
                        return $"[{values[0]}]{EnumDeviceType.PIR}";
                    case "UNDERGROUND_SENSOR":
                        return $"[{values[0]}]{EnumDeviceType.Underground}";
                    case "CONTACT_SWITCH":
                        return $"[{values[0]}]{EnumDeviceType.Contact}";
                    case "IO_CONTROLLER":
                        return $"[{values[0]}]{EnumDeviceType.IoController}";
                    case "LASER_SENSOR":
                        return $"[{values[0]}]{EnumDeviceType.Laser}";
                    case "CABLE":
                        return $"[{values[0]}]{EnumDeviceType.Cable}";
                    case "FIXED_CAMERA":
                        return $"[{values[0]}]{EnumDeviceType.IpCamera}";
                    default:
                        return values[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                return null;
            }

            
        }

        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
