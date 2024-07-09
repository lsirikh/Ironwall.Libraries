using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Ironwall.Redis.Message.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications
{
    public abstract class BaseEventMessageModel : BaseMessageModel, IBaseEventMessageModel
    {

        //public BaseEventMessageModel()
        //{
        //}

        //public BaseEventMessageModel(EnumCmdType cmd, IMetaEventModel model) : base(cmd)
        //{
        //    Id = model.Id;
        //    Group = model.EventGroup;
        //    switch (model.Device.DeviceType)
        //    {
        //        case EnumDeviceType.NONE:
        //            break;
        //        case EnumDeviceType.Controller:
        //            {
        //                Controller = (model.Device as IControllerDeviceModel).DeviceNumber;
        //            }
        //            break;
        //        case EnumDeviceType.Multi:
        //        case EnumDeviceType.Fence:
        //        case EnumDeviceType.Underground:
        //        case EnumDeviceType.Contact:
        //        case EnumDeviceType.PIR:
        //        case EnumDeviceType.IoController:
        //        case EnumDeviceType.Laser:
        //            {
        //                Controller = (model.Device as ISensorDeviceModel).Controller.DeviceNumber;
        //                Sensor = (model.Device as ISensorDeviceModel).DeviceNumber;
        //            }
        //            break;
        //        case EnumDeviceType.Cable:
        //            break;
        //        case EnumDeviceType.IpCamera:
        //            break;
        //        default:
        //            break;
        //    }
        //    UnitType = model.Device.DeviceType;
        //    Status = model.Status;
        //    DateTime = model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
        //}
        //public BaseEventMessageModel(EnumCmdType cmd, BrkDectection brk) : base(cmd)
        //{
        //    //Id = IdCodeGenerator.GenIdCode();

        //    Group = brk.IdGroup.ToString();
        //    Controller = brk.IdController;
        //    Sensor = brk.IdSensor;
        //    UnitType = (EnumDeviceType)brk.TypeDevice;

        //    DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        //}

        //public BaseEventMessageModel(EnumCmdType cmd, BrkMalfunction brk) : base(cmd)
        //{
        //    //Id = IdCodeGenerator.GenIdCode();

        //    Group = brk.IdGroup.ToString();
        //    Controller = brk.IdController;
        //    Sensor = brk.IdSensor;
        //    UnitType = (EnumDeviceType)brk.TypeDevice;

        //    DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        //}

        //public BaseEventMessageModel(EnumCmdType cmd, BrkConnection brk) : base(cmd)
        //{
        //    //Id = IdCodeGenerator.GenIdCode();

        //    Group = brk.IdGroup.ToString();
        //    Controller = brk.IdController;
        //    Sensor = brk.IdSensor;
        //    UnitType = (EnumDeviceType)brk.TypeDevice;

        //    DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        //}

        //public BaseEventMessageModel(EnumCmdType cmd, BrkContactOut brk) : base(cmd)
        //{
        //    //Id = IdCodeGenerator.GenIdCode();

        //    Group = brk.IdGroup.ToString();
        //    Controller = brk.IdController;
        //    Sensor = brk.IdSensor;
        //    UnitType = EnumDeviceType.Contact;

        //    DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        //}
        protected BaseEventMessageModel()
        {
            
        }
        protected BaseEventMessageModel(EnumCmdType cmd) : base(cmd)
        {
            
        }
        protected BaseEventMessageModel(int id, DateTime dateTime, EnumCmdType cmd) : base(cmd)
        {
            Id = id;
            DateTime = dateTime;
        }
        protected BaseEventMessageModel(IBaseEventMessageModel model):base(model)
        {
            Id = model.Id;
            DateTime = model.DateTime;
        }

        [JsonProperty("id", Order = 1)]
        public int Id { get; set; }

        [JsonProperty("date_time", Order = 99)]
        public DateTime DateTime { get; set; } = DateTime.Now;

       
    }
}
