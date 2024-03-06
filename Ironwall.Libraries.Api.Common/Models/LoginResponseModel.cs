using Newtonsoft.Json;

namespace Ironwall.Libraries.Api.Common.Models
{
    public class LoginResponseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("results")]
        public LoginResultModel Results { get; set; }
    }
}
