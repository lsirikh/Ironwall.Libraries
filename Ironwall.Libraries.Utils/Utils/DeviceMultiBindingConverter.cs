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
    public class DeviceMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string convertValue = null;

            try
            {
                if ((string)parameter == "SENSOR")
                {
                    int deviceType = (int)values[0];
                    int controller = (int)values[1];
                    int sensor = (int)values[2];

                    convertValue = CreateText(deviceType, controller, sensor);
                }
                else if ((string)parameter == "CONTROLLER")
                {
                    int deviceType = (int)values[0];
                    int controller = (int)values[1];

                    convertValue = CreateText(deviceType, controller);
                }
                return convertValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Convert)} of {nameof(DeviceMultiBindingConverter)} : {ex.Message}");
                return convertValue;
            }
            
        }

        private string CreateText(int deviceType, int controller, int sensor = 0)
        {
            try
            {
                var retValue = "";

                switch ((EnumDeviceType)deviceType)
                {
                    case EnumDeviceType.NONE:
                        break;
                    case EnumDeviceType.Controller:
                        {
                            retValue = controller + "-" + 0;
                        }
                        break;
                    case EnumDeviceType.Multi:
                    case EnumDeviceType.Fence:
                    case EnumDeviceType.Underground:
                    case EnumDeviceType.Contact:
                    case EnumDeviceType.PIR:
                    case EnumDeviceType.IoController:
                    case EnumDeviceType.Laser:
                    case EnumDeviceType.SmartSensor:
                    case EnumDeviceType.SmartSensor2:
                    case EnumDeviceType.SmartCompound:
                        {
                            retValue = controller + "-" + sensor;
                        }
                        break;
                    case EnumDeviceType.Cable:
                        break;
                    case EnumDeviceType.IpCamera:
                        break;
                    case EnumDeviceType.IpSpeaker:
                        break;
                    case EnumDeviceType.Radar:
                        break;
                    case EnumDeviceType.OpticalCable:
                        break;
                    case EnumDeviceType.Fence_Line:
                        break;
                    default:
                        break;
                }

                return retValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
