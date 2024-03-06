using Ironwall.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Defines
{
    public interface ITcpServerControlService
    {
        Task TcpServerOn();
        Task TcpServerOff();

        void JsonParser(string data, IPEndPoint endPoint);

    }
}
