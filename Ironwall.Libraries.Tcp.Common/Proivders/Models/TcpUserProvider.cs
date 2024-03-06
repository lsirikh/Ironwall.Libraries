using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Proivders.Models
{
    public class TcpUserProvider
        : TcpUserBaseProvider
    {
        public TcpUserProvider()
        {
            ClassName = nameof(TcpUserProvider);
        }
    }
}
