using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountRegisterResponseModel
        : ResponseModel, IAccountRegisterResponseModel
    {
        public AccountRegisterResponseModel()
        {
            Command = EnumCmdType.USER_ACCOUNT_ADD_RESPONSE;
        }

        public AccountRegisterResponseModel(bool success, string msg, IUserModel details)
            : base(success, msg)
        {
            Command = EnumCmdType.USER_ACCOUNT_ADD_RESPONSE;
            Details = details as AccountDetailModel;
        }

        [JsonProperty("details", Order = 4)]
        public AccountDetailModel Details { get; set; }

        public void Insert(bool success, string msg, AccountDetailModel detail)
        {
            Success = success;
            Message = msg;
            Details = detail;
        }

    }
}
