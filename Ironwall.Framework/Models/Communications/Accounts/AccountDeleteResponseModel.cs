using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountDeleteResponseModel
        : ResponseModel, IAccountDeleteResponseModel
    {
        public AccountDeleteResponseModel()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_DELETE_RESPONSE;
        }

        public AccountDeleteResponseModel(bool success, string msg)
            : base(success, msg)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_DELETE_RESPONSE;
        }

        //public void Insert(bool success, string msg)
        //{
        //    Success = success;
        //    Message = msg;
        //}
    }
}
