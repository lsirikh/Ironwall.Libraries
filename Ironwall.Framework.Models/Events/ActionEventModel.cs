using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Communications.Helpers;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ActionEventModel
        : BaseEventModel, IActionEventModel
    {

        public ActionEventModel()
        {
            MessageType = EnumEventType.Action;
        }

        public ActionEventModel(IActionEventMapper model, IMetaEventModel fromEvent) : base(model)
        {
            FromEvent = fromEvent as MetaEventModel;
            Content = model.Content;
            User = model.User;
            MessageType = EnumEventType.Action;
        }

        public ActionEventModel(IActionRequestMalfunctionModel model) : base(model)
        {
            FromEvent = model.Event;
            Content = model.Content;
            User = model.User;
            MessageType = EnumEventType.Action;
        }

        public ActionEventModel(IActionRequestDetectionModel model) : base(model)
        {
            FromEvent = model.Event;
            Content = model.Content;
            User = model.User;
            MessageType = EnumEventType.Action;
        }

        public ActionEventModel(IActionRequestModel model) : base(model)
        {
            FromEvent = model.Body.FromEvent;
            Content = model.Body.Content;
            User = model.Body.User;
            MessageType = EnumEventType.Action;
        }

        public ActionEventModel(IActionEventModel model) : base(model)
        {
            FromEvent = model.FromEvent;
            Content = model.Content;
            User = model.User;
            MessageType = EnumEventType.Action;
        }

        [JsonProperty("from_event", Order = 2)]
        [JsonConverter(typeof(EventModelConverter))] // JsonConverter 추가
        public MetaEventModel FromEvent { get; set; }
        [JsonProperty("content", Order = 3)]
        public string Content { get; set; }
        [JsonProperty("user", Order = 4)]
        public string User { get; set; }

    }
}
