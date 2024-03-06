using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class KeepAliveResponseModel
        : ResponseModel, IKeepAliveResponseModel
    {
        public KeepAliveResponseModel()
        {
            Command = (int)EnumCmdType.SESSION_REFRESH_RESPONSE;
        }

        public KeepAliveResponseModel(bool success, string msg, string expiredTime)
            : base(success, msg)
        {
            Command = (int)EnumCmdType.SESSION_REFRESH_RESPONSE;
            TimeExpired = expiredTime;
        }

        [JsonProperty("expired_time", Order = 3)]
        public string TimeExpired { get; set; }

        public void Insert(bool success, string msg, string expiredTime)
        {
            Success = success;
            Message = msg;
            TimeExpired = expiredTime;
        }
    }
}
