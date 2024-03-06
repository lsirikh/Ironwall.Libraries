using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ActionRequestModel
        : BaseEventMessageModel, IActionRequestModel
    {
        public ActionRequestModel()
        {
            Command = (int)EnumCmdType.EVENT_ACTION_REQUEST;
        }

        public ActionRequestModel(string content, string user, IDetectionEventModel model) 
            : base(model)
        {
            Id = IdCodeGenerator.GenIdCode();
            Command = (int)EnumCmdType.EVENT_ACTION_REQUEST;
            EventId = model.Id;
            Content = content;
            User = user;
            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public ActionRequestModel(string content, string user, IMalfunctionEventModel model) 
            : base(model)
        {
            Id = IdCodeGenerator.GenIdCode();
            Command = (int)EnumCmdType.EVENT_ACTION_REQUEST;
            EventId = model.Id;
            Content = content;
            User = user;
            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public ActionRequestModel(IActionEventModel model)
            : base(model.FromEvent)
        {
            Id = model.Id;
            Command = (int)EnumCmdType.EVENT_ACTION_REQUEST;
            EventId = model.FromEvent.Id;
            Content = model.Content;
            User = model.User;
        }

        [JsonProperty("event_id", Order = 6)]
        public string EventId { get; set; }
        [JsonProperty("content", Order = 7)]
        public string Content { get; set; }
        [JsonProperty("user", Order = 8)]
        public string User { get; set; }

    }
}
