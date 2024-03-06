
using Newtonsoft.Json;

namespace Ironwall.Libraries.Api.Common.Models
{
    public class LoginRequestModel
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("user_pass")]
        public string UserPass { get; set; }
        [JsonProperty("force_login")]
        public bool IsForceLogin { get; set; }
    }
}
