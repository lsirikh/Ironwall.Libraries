﻿using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountAllResponseModel
        : ResponseModel, IAccountAllResponseModel
    {
        public AccountAllResponseModel()
        {
            Command = EnumCmdType.USER_ACCOUNT_ALL_RESPONSE;
        }

        public AccountAllResponseModel(bool success, string msg, List<AccountDetailModel> detail)
            : base(success, msg)
        {
            Command = EnumCmdType.USER_ACCOUNT_ALL_RESPONSE;
            Details = detail;
        }

        [JsonProperty("details", Order = 4)]
        public List<AccountDetailModel> Details { get; set; }
    }
}
