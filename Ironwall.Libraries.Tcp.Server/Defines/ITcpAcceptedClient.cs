using Ironwall.Libraries.Tcp.Common.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Server.Defines
{
    public interface ITcpAcceptedClient : ITcpCommon
    {
        event TcpCliAccept_dele AcceptedClientConnected;
        event TcpEvent_dele AcceptedClientEvent;
        event TcpCliDiscon_dele AcceptedClientDisconnected;
    }
}
