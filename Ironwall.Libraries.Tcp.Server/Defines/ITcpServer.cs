using Ironwall.Libraries.Tcp.Server.Models;
using Ironwall.Libraries.Tcp.Server.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Ironwall.Libraries.Tcp.Common.Defines.ITcpCommon;

namespace Ironwall.Libraries.Tcp.Server.Defines
{
    public interface ITcpServer
    {
        int ClientCount { get; set; }
        List<TcpAcceptedClient> ClientList { get; set; }

        /// <summary>
        /// This Event will be triggered in Server, When a client tried to connect.
        /// </summary>
        event TcpAccept_dele ServerAccepted;
        /// <summary>
        /// This Event will be triggered in Server, When a connected client tried to send a message.
        /// </summary>
        event TcpEvent_dele ServerEvent;
        //event TcpSend_dele ServerSend;
        /// <summary>
        /// This Event will be triggered in Server, When a client tried to disconnect.
        /// </summary>
        event TcpDisconnect_dele ServerDisconnected;

        void CloseSocket();
        void InitSocket();
        Task SendRequest(string msg, IPEndPoint endPoint = null);
        Task SendFileRequest(string msg, IPEndPoint endPoint = null);
    }
}