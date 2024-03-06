using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Account.Common.Providers.Models;

namespace Ironwall.Libraries.Account.Common.Providers
{
    public class LoginUserProvider
        : UserProvider
    {
        public LoginUserProvider()
        {
            ClassName = nameof(UserProvider);
        }
    }
}
