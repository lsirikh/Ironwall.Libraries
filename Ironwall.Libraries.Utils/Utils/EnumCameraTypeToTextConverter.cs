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
    public sealed class EnumCameraTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraType.NONE.ToString();


            var inputData = (int)value;

            if (inputData == (int)EnumCameraType.NONE)
                return EnumCameraType.NONE.ToString();
            else if (inputData == (int)EnumCameraType.FIXED)
                return EnumCameraType.FIXED.ToString();
            else if (inputData == (int)EnumCameraType.PTZ)
                return EnumCameraType.PTZ.ToString();
            else if (inputData == (int)EnumCameraType.SPEED_DOM)
                return EnumCameraType.SPEED_DOM.ToString();
            else
                return EnumCameraType.NONE.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EnumCameraType.NONE.ToString();


            var inputData = value as string;

            if (inputData == EnumCameraType.NONE.ToString())
                return (int)EnumCameraType.NONE;
            else if (inputData == EnumCameraType.FIXED.ToString())
                return (int)EnumCameraType.FIXED;
            else if (inputData == EnumCameraType.PTZ.ToString())
                return (int)EnumCameraType.PTZ;
            else if (inputData == EnumCameraType.SPEED_DOM.ToString())
                return (int)EnumCameraType.SPEED_DOM;
            else
                return (int)EnumCameraType.NONE;
        }
    }
}
