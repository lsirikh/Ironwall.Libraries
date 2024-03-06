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
    public class ActionRequestMalfunctionModel
        : ActionRequestModel, IActionRequestMalfunctionModel
    {
        public ActionRequestMalfunctionModel()
        {
            Command = (int)EnumCmdType.EVENT_MALFUNCTION_REQUEST;
        }

        [JsonProperty("detail", Order = 9)]
        MalfunctionDetailModel Detail { get; set; }

        //public void Insert(string id, string group, int controller, int sensor, int uType, string eventId, string content, string user, MalfunctionDetailModel detail, string dateTime)
        //{
        //    base.Insert(id, group, controller, sensor, uType, eventId, content, user, dateTime);
        //    Detail = detail;
        //}

        //public void Insert(ActionRequestMalfunctionModel model)
        //{
        //    Insert(model.Id, model.GroupId, model.CONTROLLER, model.Sensor, model.UnitType, model.EventId, model.Content, model.User, model.Detail, model.DateTime);
        //}
    }
}
