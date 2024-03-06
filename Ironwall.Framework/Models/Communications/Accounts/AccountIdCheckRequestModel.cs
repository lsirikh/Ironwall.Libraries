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
    public class AccountIdCheckRequestModel
        : BaseMessageModel, IAccountIdCheckRequestModel
    {
        public AccountIdCheckRequestModel()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_REQUEST;
        }
        public AccountIdCheckRequestModel(string idUser)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_REQUEST;
            IdUser = idUser;
        }

        [JsonProperty("user_id", Order = 1)]
        public string IdUser { get; set; }
    }
}
