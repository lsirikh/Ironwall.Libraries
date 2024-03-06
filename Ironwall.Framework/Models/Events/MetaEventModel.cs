using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class MetaEventModel
        : BaseEventModel, IMetaEventModel
    {

        public MetaEventModel()
        {

        }

        public MetaEventModel(IMetaEventMapper model, IBaseDeviceModel device) : base(model)
        {
            EventGroup = model.EventGroup;
            MessageType = model.MessageType;
            Device = device;
            Status = model.Status;
        }

        public MetaEventModel(IBaseEventMessageModel model, IBaseDeviceModel device) : base(model)
        {
            EventGroup = model.Group;
            MessageType = EnumHelper.GetEventType(model.Command);
            Device = device;
            Status = model.Status;
        }

        public MetaEventModel(IMetaEventViewModel model) : base(model)
        {
            EventGroup = model.EventGroup;
            MessageType = model.MessageType;
            Status = model.Status;
        }

        public string EventGroup { get; set; }
        public int MessageType { get; set; }
        public int Status { get; set; }
        public IBaseDeviceModel Device { get; set; }
    }
}
