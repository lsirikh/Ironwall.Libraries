using Newtonsoft.Json;

namespace Ironwall.Libraries.Api.Common.Models
{
    public class LoginResultModel
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }
        [JsonProperty("client_id")]
        public int ClientId { get; set; }
        [JsonProperty("user_level")]
        public int UserLevel { get; set; }
        [JsonProperty("details")]
        public string Details { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}
