using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Account.Common.Providers.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Providers
{
    //class SessionProvider
    //     : EntityCollectionProvider<LoginSessionModel>
    //{
    //}

    public class SessionProvider : AccountBaseProvider
    {
        public SessionProvider()
        {
            ClassName = nameof(SessionProvider);
        }
    }
}
