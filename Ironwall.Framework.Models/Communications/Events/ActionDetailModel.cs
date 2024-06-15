﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ActionDetailModel : IActionDetailModel
    {
        public ActionDetailModel()
        {

        }

        public ActionDetailModel(string id, string idUser, string content, string fromEventId)
        {
            Id = id;
            IdUser = idUser;
            Content = content;
            FromEventId = fromEventId;
        }

        [JsonProperty("id", Order = 4)]
        public string Id { get; set; }
        [JsonProperty("idUser", Order = 5)]
        public string IdUser { get; set; }
        [JsonProperty("content", Order = 6)]
        public string Content { get; set; }
        [JsonProperty("from_event_id", Order = 7)]
        public string FromEventId { get; set; }
    }
}
