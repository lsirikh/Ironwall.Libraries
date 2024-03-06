using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class AccountIdCheckResponseModel 
        : ResponseModel
    {
        public AccountIdCheckResponseModel()
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_RESPONSE;
        }

        public AccountIdCheckResponseModel(bool success, string message)
            : base(success, message)
        {
            Command = (int)EnumCmdType.USER_ACCOUNT_ID_CHECK_RESPONSE;
        }
    }
}
