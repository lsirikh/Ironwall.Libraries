using Ironwall.Framework.Models.Accounts;
using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class LoginResultModel 
        : ILoginResultModel
    {
        public LoginResultModel()
        {

        }

        public LoginResultModel(
            string userId,
            string token,
            int clientId,
            int userLevel,
            int sessionTimeOut,
            IUserModel details,
            string createdTime,
            string expiredTime)
        {
            UserId = userId;
            Token = token;
            ClientId = clientId;
            UserLevel = userLevel;
            SessionTimeOut = sessionTimeOut;
            Details = details as AccountDetailModel;
            TimeCreated = createdTime;
            TimeExpired = expiredTime;
        }

        [JsonProperty("user_id", Order = 1)]
        public string UserId { get; set; }

        [JsonProperty("auth_token", Order = 2)]
        public string Token { get; set; }

        [JsonProperty("client_id", Order = 3)]
        public int ClientId { get; set; }

        [JsonProperty("user_level", Order = 4)]
        public int UserLevel { get; set; }

        [JsonProperty("session_timeout", Order = 5)]
        public int SessionTimeOut { get; set; }

        [JsonProperty("details", Order = 6)]
        public AccountDetailModel Details { get; set; }

        [JsonProperty("created_time", Order = 7)]
        public string TimeCreated { get; set; }

        [JsonProperty("expired_time", Order = 8)]
        public string TimeExpired { get; set; }

        public void Insert(string userId,
            string token,
            int clientId,
            int userLevel,
            int sessionTimeOut,
            AccountDetailModel details,
            string createdTime,
            string expiredTime
            )
        {
            UserId = userId;
            Token = token;
            ClientId = clientId;
            UserLevel = userLevel;
            SessionTimeOut = sessionTimeOut;
            Details = details;
            TimeCreated = createdTime;
            TimeExpired = expiredTime;
        }
    }
}
