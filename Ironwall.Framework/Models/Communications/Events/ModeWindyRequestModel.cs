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
    public class ModeWindyRequestModel 
        : BaseMessageModel
        , IModeWindyRequestModel
    {
        public ModeWindyRequestModel()
        {
            Command = (int)EnumCmdType.MODE_WINDY_REQUEST;
        }

        public ModeWindyRequestModel(IModeWindyEventModel model)
        {
            Command = (int)EnumCmdType.MODE_WINDY_REQUEST;
        }


        [JsonProperty("mode_windy", Order = 1)]
        public int ModeWindy { get; set; }
    }
}
