using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Communications.Helpers
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/1/2024 2:30:36 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class DeviceModelConverter : JsonConverter<BaseDeviceModel>
    {
        public override BaseDeviceModel ReadJson(JsonReader reader, Type objectType, BaseDeviceModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // DeviceType에 따라 적절한 클래스 인스턴스를 생성
            EnumDeviceType deviceType = jo["device_type"].ToObject<EnumDeviceType>();
            BaseDeviceModel device = null;

            switch (deviceType)
            {
                case EnumDeviceType.NONE:
                    break;
                case EnumDeviceType.Controller:
                    device = jo.ToObject<ControllerDeviceModel>();
                    break;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Radar:
                case EnumDeviceType.OpticalCable:
                    device = jo.ToObject<SensorDeviceModel>();
                    break;
                case EnumDeviceType.Cable:
                    break;
                case EnumDeviceType.IpCamera:
                    device = jo.ToObject<CameraDeviceModel>();
                    break;
                case EnumDeviceType.IpSpeaker:
                    break;
                case EnumDeviceType.Fence_Line:
                    break;
                default:
                    throw new Exception($"Unknown device type: {deviceType}");
            }

            serializer.Populate(jo.CreateReader(), device);
            return device;
        }

        public override void WriteJson(JsonWriter writer, BaseDeviceModel value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}