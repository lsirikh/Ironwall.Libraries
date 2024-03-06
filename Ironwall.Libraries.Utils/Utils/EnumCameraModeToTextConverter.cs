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
    public sealed class EnumCameraModeToTextConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                return null;

            switch (value)
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
            throw new NotImplementedException();
        }
    }
}
