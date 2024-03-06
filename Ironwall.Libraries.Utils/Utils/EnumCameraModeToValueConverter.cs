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
    public sealed class EnumCameraModeToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(object))
                return null;

            switch ((int)value)
            {
                case (int)EnumCameraMode.NONE:
                    return EnumCameraMode.NONE.ToString();
                case (int)EnumCameraMode.ONVIF:
                    return EnumCameraMode.ONVIF.ToString();
                case (int)EnumCameraMode.INNODEP_API:
                    return EnumCameraMode.INNODEP_API.ToString();
                case (int)EnumCameraMode.ETC:
                    return EnumCameraMode.ETC.ToString();
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
                return null;

            if (value.ToString() == EnumCameraMode.NONE.ToString())
                return (int)EnumCameraMode.NONE;
            else if (value.ToString() == EnumCameraMode.ONVIF.ToString())
                return (int)EnumCameraMode.ONVIF;
            else if (value.ToString() == EnumCameraMode.INNODEP_API.ToString())
                return (int)EnumCameraMode.INNODEP_API;
            else if (value.ToString() == EnumCameraMode.ETC.ToString())
                return (int)EnumCameraMode.ETC;
            else
                return null;
        }
    }
}
