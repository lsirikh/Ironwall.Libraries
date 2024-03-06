
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountEditResponseModel
        : AccountInfoResponseModel, IAccountEditResponseModel
    {
        public AccountEditResponseModel()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_EDIT_RESPONSE;
        }

        public AccountEditResponseModel(bool success, string msg, IUserModel detail)
            : base(success, msg, detail)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_EDIT_RESPONSE;
        }
    }
}
