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
    public sealed class EnumCameraStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraStatus.NONE.ToString();


            var inputData = (int)value;

            if (inputData == (int)EnumCameraStatus.NONE)
                return EnumCameraStatus.NONE.ToString();
            else if (inputData == (int)EnumCameraStatus.ACTIVE)
                return EnumCameraStatus.ACTIVE.ToString();
            else if (inputData == (int)EnumCameraStatus.INACTIVE)
                return EnumCameraStatus.INACTIVE.ToString();
            else if (inputData == (int)EnumCameraStatus.NO_RESPONSE)
                return EnumCameraStatus.NO_RESPONSE.ToString();
            else
                return EnumCameraStatus.NONE.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraStatus.NONE.ToString();


            var inputData = value as string;

            if (inputData == EnumCameraStatus.NONE.ToString())
                return (int)EnumCameraStatus.NONE;
            else if (inputData == EnumCameraStatus.ACTIVE.ToString())
                return (int)EnumCameraStatus.ACTIVE;
            else if (inputData == EnumCameraStatus.INACTIVE.ToString())
                return (int)EnumCameraStatus.INACTIVE;
            else if (inputData == EnumCameraStatus.NO_RESPONSE.ToString())
                return (int)EnumCameraStatus.NO_RESPONSE;
            else
                return (int)EnumCameraStatus.NONE;
        }
    }
}
