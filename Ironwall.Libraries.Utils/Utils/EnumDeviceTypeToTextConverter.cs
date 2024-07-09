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
    public class EnumDeviceTypeToTextConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                return null;

            //return (EnumDeviceType)value;
            switch (value)
            {
                case (int)EnumDeviceType.NONE:
                    return EnumDeviceType.NONE.ToString();
                case (int)EnumDeviceType.Controller:
                    return EnumDeviceType.Controller.ToString();
                case (int)EnumDeviceType.Multi:
                    return EnumDeviceType.Multi.ToString();
                case (int)EnumDeviceType.Fence:
                    return EnumDeviceType.Fence.ToString();
                case (int)EnumDeviceType.Underground:
                    return EnumDeviceType.Underground.ToString();
                case (int)EnumDeviceType.Contact:
                    return EnumDeviceType.Contact.ToString();
                case (int)EnumDeviceType.PIR:
                    return EnumDeviceType.PIR.ToString();
                case (int)EnumDeviceType.IoController:
                    return EnumDeviceType.IoController.ToString();
                case (int)EnumDeviceType.Laser:
                    return EnumDeviceType.Laser.ToString();
                case (int)EnumDeviceType.Cable:
                    return EnumDeviceType.Cable.ToString();
                case (int)EnumDeviceType.IpCamera:
                    return EnumDeviceType.IpCamera.ToString();
                case (int)EnumDeviceType.IpSpeaker:
                    return EnumDeviceType.IpSpeaker.ToString();
                case (int)EnumDeviceType.Radar:
                    return EnumDeviceType.Radar.ToString();
                case (int)EnumDeviceType.OpticalCable:
                    return EnumDeviceType.OpticalCable.ToString();
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
