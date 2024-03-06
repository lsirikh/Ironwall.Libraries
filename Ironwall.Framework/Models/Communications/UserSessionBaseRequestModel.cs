using Ironwall.Framework.Models.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications
{
    public abstract class UserSessionBaseRequestModel
        : BaseMessageModel, IUserSessionBaseRequestModel
    {
        public UserSessionBaseRequestModel()
        {

        }

        public UserSessionBaseRequestModel(ILoginSessionModel model)
        {
            UserId = model.UserId;
            Token = model.Token;
            TimeCreated = model.TimeCreated;
            TimeExpired = model.TimeExpired;
        }

        [JsonProperty("user_id", Order = 1)]
        public string UserId { get; private set; }
        [JsonIgnore]
        public string UserPass { get; private set; }
        [JsonProperty("token", Order = 2)]
        public string Token { get; private set; }
        [JsonProperty("time_created", Order = 3)]
        public string TimeCreated { get; private set; }
        [JsonProperty("time_expired", Order = 4)]
        public string TimeExpired { get; private set; }
    }

}
