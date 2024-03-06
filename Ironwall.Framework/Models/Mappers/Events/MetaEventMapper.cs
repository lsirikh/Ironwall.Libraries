using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class MetaEventMapper
        : EventMapperBase, IMetaEventMapper
    {

        public MetaEventMapper()
        {

        }
        public MetaEventMapper(IMetaEventModel model) : base (model)
        {
            EventGroup = model.EventGroup;
            MessageType = model.MessageType;
            Device = model.Device.Id;
            Status = model.Status;
        }

        public MetaEventMapper(IBaseEventMessageModel model, IBaseDeviceModel device) : base(model)
        {
            EventGroup = model.Group;
            MessageType = model.Command;
            Status = model.Status;
            Device = device.Id;
        }

        public string EventGroup { get; set; }
        public int MessageType { get; set; }
        public int Status { get; set; }
        public string Device { get; set; }
    }
}
