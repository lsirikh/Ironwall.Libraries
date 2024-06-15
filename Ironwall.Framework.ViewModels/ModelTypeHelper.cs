using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public static class ModelTypeHelper
    {

        public static IBaseDeviceModel GetDevice(IBaseDeviceViewModel device)
        {
            IBaseDeviceModel model = null;
            try
            {
                switch ((EnumDeviceType)device.DeviceType)
                {
                    case EnumDeviceType.NONE:
                        {

                        }
                        break;
                    case EnumDeviceType.Controller:
                        {
                            model = ModelFactory
                                .Build<ControllerDeviceModel>(device as IControllerDeviceViewModel);
                        }
                        break;
                    case EnumDeviceType.Multi:
                    case EnumDeviceType.Fence:
                    case EnumDeviceType.Underground:
                    case EnumDeviceType.Contact:
                    case EnumDeviceType.PIR:
                    case EnumDeviceType.IoController:
                    case EnumDeviceType.Laser:
                    case EnumDeviceType.Cable:
                        {
                            var sensor = device as ISensorDeviceViewModel;
                            model = ModelFactory
                                .Build<SensorDeviceModel>(sensor);
                        }
                        break;
                    case EnumDeviceType.IpCamera:
                        break;
                    default:
                        break;
                }

                return model;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in GetDevice(ModelTypeHelper) : {ex.Message}");
                return null;
            }
        }


        public static IMetaEventModel GetEvent(IMetaEventViewModel fromEvent)
        {
            IMetaEventModel model = null;
            try
            {
                switch ((EnumEventType)fromEvent.MessageType)
                {
                    case EnumEventType.Intrusion:
                        model = ModelFactory.Build<DetectionEventModel>(fromEvent as IDetectionEventViewModel);
                        break;
                    case EnumEventType.ContactOn:
                        break;
                    case EnumEventType.ContactOff:
                        break;
                    case EnumEventType.Connection:
                        break;
                    case EnumEventType.Action:
                        break;
                    case EnumEventType.Fault:
                        model = ModelFactory.Build<MalfunctionEventModel>(fromEvent as IMalfunctionEventViewModel);
                        break;
                    case EnumEventType.WindyMode:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            return model;
        }
    }
}
