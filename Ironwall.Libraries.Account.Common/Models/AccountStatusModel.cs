using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Models
{
    public class AccountStatusModel : IAccountStatusModel
    {
        #region - Ctors -
        public AccountStatusModel()
        {

        }

        public AccountStatusModel(bool isLogin, int level, string status, ILoginSessionModel sessionModel = null, IUserModel userModel = null)
        {
            IsLogin = isLogin;
            Level = level;
            Status = status;
            SessionModel = sessionModel;
            UserModel = userModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsLogin { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public ILoginSessionModel SessionModel { get; set; }
        public IUserModel UserModel { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
