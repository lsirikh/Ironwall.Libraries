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
    public sealed class EnumCameraStatusToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraStatus.NONE;

            if (targetType != typeof(object))
                return null;


            var inputData = (int)value;

            if (inputData == (int)EnumCameraStatus.NONE)
                return EnumCameraStatus.NONE;
            else if (inputData == (int)EnumCameraStatus.ACTIVE)
                return EnumCameraStatus.ACTIVE;
            else if (inputData == (int)EnumCameraStatus.INACTIVE)
                return EnumCameraStatus.INACTIVE;
            else if (inputData == (int)EnumCameraStatus.NO_RESPONSE)
                return EnumCameraStatus.NO_RESPONSE;
            else
                return EnumCameraStatus.NONE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraStatus.NONE;

            if (targetType != typeof(int))
                return 0;

            var inputData = (EnumCameraStatus)value;

            if (inputData == EnumCameraStatus.NONE)
                return (int)EnumCameraStatus.NONE;
            else if (inputData == EnumCameraStatus.ACTIVE)
                return (int)EnumCameraStatus.ACTIVE;
            else if (inputData == EnumCameraStatus.INACTIVE)
                return (int)EnumCameraStatus.INACTIVE;
            else if (inputData == EnumCameraStatus.NO_RESPONSE)
                return (int)EnumCameraStatus.NO_RESPONSE;
            else
                return (int)EnumCameraStatus.NONE;
        }
    }
}
