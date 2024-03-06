using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class LogoutRequestModel : BaseMessageModel, ILogoutRequestModel
    {
        public LogoutRequestModel()
        {
            Command = (int)EnumCmdType.LOGOUT_REQUEST;
        }

        public LogoutRequestModel(string userId, string token)
        {
            Command = (int)EnumCmdType.LOGOUT_REQUEST;
            UserId = userId;
            Token = token;
        }

        public LogoutRequestModel(int cmd, string userId, string token)
            : base(cmd)
        {
            UserId = userId;
            Token = token;
        }

        [JsonProperty("user_id", Order = 1)]
        public string UserId { get; set; }
        [JsonProperty("auth_token", Order = 2)]
        public string Token { get; set; }

        public void Insert(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }

        public void Insert(int cmd, string userId, string token)
        {
            Command = cmd;
            UserId = userId;
            Token = token;
        }
    }
}
