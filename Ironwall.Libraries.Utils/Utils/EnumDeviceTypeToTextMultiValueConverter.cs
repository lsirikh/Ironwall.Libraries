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
                        return $"[{values[0].ToString()}]{EnumDeviceType.Controller.ToString()}";
                    case "FENCE_SENSOR":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Fence.ToString()}";
                    case "MULTI_SNESOR":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Multi.ToString()}";
                    case "PIR_SENSOR":
                        return $"[{values[0].ToString()}]{EnumDeviceType.PIR.ToString()}";
                    case "UNDERGROUND_SENSOR":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Underground.ToString()}";
                    case "CONTACT_SWITCH":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Contact.ToString()}";
                    case "IO_CONTROLLER":
                        return $"[{values[0].ToString()}]{EnumDeviceType.IoController.ToString()}";
                    case "LASER_SENSOR":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Laser.ToString()}";
                    case "CABLE":
                        return $"[{values[0].ToString()}]{EnumDeviceType.Cable.ToString()}";
                    case "FIXED_CAMERA":
                        return $"[{values[0].ToString()}]{EnumDeviceType.IpCamera.ToString()}";
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
