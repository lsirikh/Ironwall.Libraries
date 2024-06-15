using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Ironwall.Redis.Message.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class MalfunctionRequestModel
    : BaseEventMessageModel, IMalfunctionRequestModel
    {
        public MalfunctionRequestModel()
        {
            Command = EnumCmdType.EVENT_MALFUNCTION_REQUEST;
        }

        public MalfunctionRequestModel(BrkMalfunction brk) : base(brk)
        {
            Command = EnumCmdType.EVENT_MALFUNCTION_REQUEST;
            Detail = RequestFactory.Build<MalfunctionDetailModel>((EnumFaultType)brk.Reason, brk.CutFirstStart, brk.CutFirstEnd, brk.CutSecondStart, brk.CutSecondEnd);
        }

        public MalfunctionRequestModel(IMalfunctionEventModel model) : base(model)
        {
            Command = EnumCmdType.EVENT_MALFUNCTION_REQUEST;
            Detail = RequestFactory.Build<MalfunctionDetailModel>(model.Reason, model.FirstStart, model.FirstEnd, model.SecondStart, model.SecondEnd);
        }

        [JsonProperty("detail", Order = 6)]
        public MalfunctionDetailModel Detail { get; set; }

        //public void Insert(string id, string group, int controller, int sensor, int uType, MalfunctionDetailModel detail, string dateTime)
        //{
        //    Insert(id, group, controller, sensor, uType, dateTime);
        //    Detail = detail;
        //}
    }
}
