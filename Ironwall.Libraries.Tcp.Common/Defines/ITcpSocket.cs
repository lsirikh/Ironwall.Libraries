using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Defines
{
    public interface ITcpSocket
    {
        int Mode { get; set; }
        Socket Socket { get; }

        void CloseSocket();
        void InitSocket();
        Task SendRequest(string msg, IPEndPoint selectedIp = null);
        Task SendFileRequest(string file, IPEndPoint selectedIp = null);
        Task SendMapDataRequest(string file, IPEndPoint selectedIp = null);
        Task SendProfileDataRequest(string file, IPEndPoint selectedIp = null);
        Task SendVideoDataRequest(string file, IPEndPoint selectedIp = null);
    }
}