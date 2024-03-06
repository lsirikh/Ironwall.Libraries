using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Account.Common.Providers.Models;

namespace Ironwall.Libraries.Account.Common.Providers
{
    public class LoginProvider
        : AccountBaseProvider
    {
        public LoginProvider()
        {
            ClassName = nameof(LoginProvider);
        }
    }
}
