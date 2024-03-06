﻿using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountAllRequestModel
        : BaseMessageModel, IAccountAllRequestModel
    {
        public AccountAllRequestModel()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ALL_REQUEST;
        }

        public AccountAllRequestModel(string idUser, string pass)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ALL_REQUEST;
            IdUser = idUser;
            Password = pass;
        }

        [JsonProperty("user_id", Order = 1)]
        public string IdUser { get; set; }

        [JsonProperty("user_pass", Order = 2)]
        public string Password { get; set; }
    }
}
