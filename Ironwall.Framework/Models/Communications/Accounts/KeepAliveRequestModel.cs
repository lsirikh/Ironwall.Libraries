using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class KeepAliveRequestModel : BaseMessageModel, IKeepAliveRequestModel
    {
        public KeepAliveRequestModel()
        {
            Command = (int)EnumCmdType.SESSION_REFRESH_REQUEST;
        }

        public KeepAliveRequestModel(string token)
        {
            Command = (int)EnumCmdType.SESSION_REFRESH_REQUEST;
            Token = token;
        }

        [JsonProperty("auth_token", Order = 1)]
        public string Token { get; set; }
   }
}
