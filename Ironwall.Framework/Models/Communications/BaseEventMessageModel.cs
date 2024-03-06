using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Ironwall.Redis.Message.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications
{
    public abstract class BaseEventMessageModel
        : BaseMessageModel, IBaseEventMessageModel
    {

        public BaseEventMessageModel()
        {

        }

        public BaseEventMessageModel(IMetaEventModel model)
        {
            Id = model.Id != null ? model.Id : IdCodeGenerator.GenIdCode();

            Group = model.EventGroup;
            switch ((EnumDeviceType)model.Device.DeviceType)
            {
                case EnumDeviceType.NONE:
                    break;
                case EnumDeviceType.Controller:
                    {
                        Controller = (model.Device as IControllerDeviceModel).DeviceNumber;
                    }
                    break;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                    {
                        Controller = (model.Device as ISensorDeviceModel).Controller.DeviceNumber;
                        Sensor = (model.Device as ISensorDeviceModel).DeviceNumber;
                    }
                    break;
                case EnumDeviceType.Cable:
                    break;
                case EnumDeviceType.IpCamera:
                    break;
                default:
                    break;
            }
            UnitType = model.Device.DeviceType;
            Status = model.Status;
            DateTime = model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }
        public BaseEventMessageModel(BrkDectection brk)
        {
            Id = IdCodeGenerator.GenIdCode();

            Group = brk.IdGroup.ToString();
            Controller = brk.IdController;
            Sensor = brk.IdSensor;
            UnitType = brk.TypeDevice;

            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public BaseEventMessageModel(BrkMalfunction brk)
        {
            Id = IdCodeGenerator.GenIdCode();

            Group = brk.IdGroup.ToString();
            Controller = brk.IdController;
            Sensor = brk.IdSensor;
            UnitType = brk.TypeDevice;

            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public BaseEventMessageModel(BrkConnection brk)
        {
            Id = IdCodeGenerator.GenIdCode();

            Group = brk.IdGroup.ToString();
            Controller = brk.IdController;
            Sensor = brk.IdSensor;
            UnitType = brk.TypeDevice;

            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public BaseEventMessageModel(BrkContactOut brk)
        {
            Id = IdCodeGenerator.GenIdCode();

            
            Group = brk.IdGroup.ToString();
            Controller = brk.IdController;
            Sensor = brk.IdSensor;
            UnitType = (int)EnumDeviceType.Contact;

            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        [JsonProperty("group", Order = 2)]
        public string Group { get; set; }

        [JsonProperty("controller", Order = 3)]
        public int Controller { get; set; }

        [JsonProperty("sensor", Order = 4)]
        public int Sensor { get; set; }

        [JsonProperty("unit_type", Order = 5)]
        public int UnitType { get; set; }

        [JsonProperty("status", Order = 6)]
        public int Status { get; set; }

        [JsonProperty("date_time", Order = 99)]
        public string DateTime { get; set; }

       
    }
}
