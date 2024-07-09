using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Models
{
    static class AccountModelFactory
    {
        //static T Build<T>(IAccountStatusModel model) where T : AccountStatusModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
        //    return instance;
        //}

        //static T Build<T>(bool isLogin, string status) where T : AccountStatusModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { isLogin, status });
        //    return instance;
        //}

        static T Build<T>(bool isLogin = false, int level = 0, string status = null, ILoginSessionModel sessionModel = null, IUserModel userModel = null) where T 
            : AccountStatusModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { isLogin, level, status, sessionModel, userModel });
            return instance;
        }
    }
}
