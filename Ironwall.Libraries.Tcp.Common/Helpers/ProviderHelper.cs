using Ironwall.Libraries.Tcp.Common.Proivders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Helpers
{
    public static class ProviderHelper
    {
        public static int GetMaxId(TcpClientProvider provider)
        {
            int count = 0;
            foreach (var item in provider.ToList())
            {
                if (item.Id > count)
                    count = item.Id;
            }
            return count;
        }
    }
}
