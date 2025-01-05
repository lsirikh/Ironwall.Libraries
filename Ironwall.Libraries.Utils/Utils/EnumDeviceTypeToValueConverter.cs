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
    public sealed class EnumDeviceTypeToValueConverter
        : IValueConverter
    {
        /// <summary>
        /// MVVM 기준으로 ViewModel에서 넘어온 데이터가 Xaml로 표기가 되는 과정에서
        /// 일어나게 된다. 정확한 프로세스는 ViewModel의 property에서 해당 타입의 데이터가
        /// 넘어오고, 넘어온 데이터는 Xaml에 표기를 위해서 설정된 선택 풀에 해당하는 옵션으로
        /// 매칭시켜주는 작업이 필요하다.
        /// Pool - EnumDeviceType
        /// Selectable options - CONTROLLER
        ///                    - MULTI_SNESOR
        ///                    - FENCE_SENSOR
        ///                    - UNDERGROUND_SENSOR
        ///                    - CONTACT_SWITCH
        ///                    - PIR_SENSOR
        ///                    - IO_CONTROLLER
        ///                    - LASER_SENSOR
        ///                    - CABLE
        /// </summary>
        /// <param name="value">ViewModel의 Property가 활용된다.</param>
        /// <param name="targetType">Xaml로 넘어가는 데이터 변환 타입</param>
        /// <param name="parameter">Xaml에서 컨버터로 넘어올때 옵션으로 사용하는 파라미터</param>
        /// <param name="culture"></param>
        /// <returns>object type value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(object))
                return null;

            //return (EnumDeviceType)value;
            switch ((int)value)
            {
                case (int)EnumDeviceType.NONE:
                    return EnumDeviceType.NONE;
                case (int)EnumDeviceType.Controller:
                    return EnumDeviceType.Controller;
                case (int)EnumDeviceType.Multi:
                    return EnumDeviceType.Multi;
                case (int)EnumDeviceType.Fence:
                    return EnumDeviceType.Fence;
                case (int)EnumDeviceType.Underground:
                    return EnumDeviceType.Underground;
                case (int)EnumDeviceType.Contact:
                    return EnumDeviceType.Contact;
                case (int)EnumDeviceType.PIR:
                    return EnumDeviceType.PIR;
                case (int)EnumDeviceType.IoController:
                    return EnumDeviceType.IoController;
                case (int)EnumDeviceType.Laser:
                    return EnumDeviceType.Laser;
                case (int)EnumDeviceType.Cable:
                    return EnumDeviceType.Cable;
                case (int)EnumDeviceType.IpCamera:
                    return EnumDeviceType.IpCamera;
                default:
                    return EnumDeviceType.Controller;
            }
        }

        /// <summary>
        /// Xaml에서 ViewModel로 넘겨주는 과정에 활용되는 Method 보통 컨버터를 사용하면,
        /// Xaml표기타입과 ViewModel의 Property의 데이터 타입이 다르기 때문에 사용하는데,
        /// Xaml쪽에서 변경이 발생하면, 해당 과정으로 통해 ViewModel의 Property로 데이터가 넘어간다.
        /// </summary>
        /// <param name="value">Xaml에서 보내는 데이터</param>
        /// <param name="targetType">ViewModel의 Property로 보내는 데이터</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>int type value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int)) return null;

            if (value.ToString() == EnumDeviceType.NONE.ToString())
                return (int)EnumDeviceType.NONE;
            else if(value.ToString() == EnumDeviceType.Controller.ToString())
                return (int)EnumDeviceType.Controller;
            else if (value.ToString() == EnumDeviceType.Multi.ToString())
                return (int)EnumDeviceType.Multi;
            else if (value.ToString() == EnumDeviceType.Fence.ToString())
                return (int)EnumDeviceType.Fence;
            else if (value.ToString() == EnumDeviceType.Underground.ToString())
                return (int)EnumDeviceType.Underground;
            else if (value.ToString() == EnumDeviceType.Contact.ToString())
                return (int)EnumDeviceType.Contact;
            else if (value.ToString() == EnumDeviceType.PIR.ToString())
                return (int)EnumDeviceType.PIR;
            else if (value.ToString() == EnumDeviceType.IoController.ToString())
                return (int)EnumDeviceType.IoController;
            else if (value.ToString() == EnumDeviceType.Laser.ToString())
                return (int)EnumDeviceType.Laser;
            else if (value.ToString() == EnumDeviceType.Cable.ToString())
                return (int)EnumDeviceType.Cable;
            else if (value.ToString() == EnumDeviceType.IpCamera.ToString())
                return (int)EnumDeviceType.IpCamera;
            else
                return null;
        }
    }
}
