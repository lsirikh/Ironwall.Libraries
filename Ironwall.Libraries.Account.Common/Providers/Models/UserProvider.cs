using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Account.Common.Providers.Models;

namespace Ironwall.Libraries.Account.Common.Providers
{
    public class UserProvider
        : AccountBaseProvider
    {
        public UserProvider()
        {
            ClassName = nameof(UserProvider);
        }
    }
}
