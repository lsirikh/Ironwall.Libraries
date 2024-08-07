﻿using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
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
            MessageType = (int)model.MessageType;
            Device = model.Device.Id;
            Status = EnumHelper.GetStatusType(model.Status);
        }

        public string EventGroup { get; set; }
        public int Device { get; set; }
        public bool Status { get; set; }
    }
}
