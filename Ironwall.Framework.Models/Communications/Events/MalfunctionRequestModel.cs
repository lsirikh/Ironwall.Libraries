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
    : BaseMessageModel, IMalfunctionRequestModel
    {
        public MalfunctionRequestModel()
        {
            Command = EnumCmdType.EVENT_MALFUNCTION_REQUEST;
        }

        public MalfunctionRequestModel(IMalfunctionEventModel model) 
            : base(EnumCmdType.EVENT_MALFUNCTION_REQUEST)
        {
            Body = model as MalfunctionEventModel;
        }

        [JsonProperty("detail", Order = 6)]
        public MalfunctionEventModel Body { get; set; }

    }
}
