using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountIdCheckRequest
        : BaseMessageModel, IAccountIdCheckRequest
    {
        public AccountIdCheckRequest()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_REQUEST;
        }
        public AccountIdCheckRequest(string idUser)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_REQUEST;
            IdUser = idUser;
        }

        [JsonProperty("user_id", Order = 1)]
        public string IdUser { get; set; }
    }
}
