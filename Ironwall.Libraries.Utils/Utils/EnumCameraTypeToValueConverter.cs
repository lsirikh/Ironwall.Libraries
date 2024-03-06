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
    public sealed class EnumCameraTypeToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraType.NONE;

            if (targetType != typeof(object))
                return null;


            var inputData = (int)value;

            if (inputData == (int)EnumCameraType.NONE)
                return EnumCameraType.NONE;
            else if (inputData == (int)EnumCameraType.FIXED)
                return EnumCameraType.FIXED;
            else if (inputData == (int)EnumCameraType.PTZ)
                return EnumCameraType.PTZ;
            else if (inputData == (int)EnumCameraType.SPEED_DOM)
                return EnumCameraType.SPEED_DOM;
            else
                return EnumCameraType.NONE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraType.NONE;

            if (targetType != typeof(int))
                return 0;

            var inputData = (EnumCameraType)value;

            if (inputData == EnumCameraType.NONE)
                return (int)EnumCameraType.NONE;
            else if (inputData == EnumCameraType.FIXED)
                return (int)EnumCameraType.FIXED;
            else if (inputData == EnumCameraType.PTZ)
                return (int)EnumCameraType.PTZ;
            else if (inputData == EnumCameraType.SPEED_DOM)
                return (int)EnumCameraType.SPEED_DOM;
            else
                return (int)EnumCameraType.NONE;
        }
    }
}
