using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Providers.ViewModels
{
    public class LoginUserViewModelProvider
        : UserViewModelProvider
    {
        public LoginUserViewModelProvider(UserProvider provider)
            : base(provider)
        {
            ClassName = nameof(LoginUserViewModelProvider);
        }

    }

}
