using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System.Net;
using System.Threading.Tasks;
using static Ironwall.Libraries.Tcp.Common.Defines.ITcpCommon;

namespace Ironwall.Libraries.Accounts.Services
{
    public interface IAccountServerService
    {
        /// <summary>
        /// Login is the process to get the required token for communication with a server and clients 
        /// Command : 100
        /// </summary>
        /// <param name="model">LoginRequestModel</param>
        /// <param name="endPoint">IPEndPoint</param>
        /// <returns>bool</returns>
        public Task<bool> Login(LoginRequestModel model, IPEndPoint endPoint = null);
        /// <summary>
        /// Logout is the process to remove session information from SessionProvider and Db 
        /// Command : 200
        /// </summary>
        /// <param name="model"></param>
        /// <param name="endPoint"></param>
        /// <returns>bool</returns>
        public Task<bool> Logout(LogoutRequestModel model = null, IPEndPoint endPoint = null);
        /// <summary>
        /// KeepAlive is the process to extend the expiration time as Session
        /// Command : 300
        /// </summary>
        /// <param name="model">KeepAliveRequestModel</param>
        /// <param name="endPoint">IPEndPoint</param>
        /// <returns>bool</returns>
        public Task<bool> KeepAlive(KeepAliveRequestModel model, IPEndPoint endPoint = null);
        public Task<string> CreateToken();

        event TcpLogin_dele ClientLogin;
        event TcpLogout_dele ClientLogout;
    }
}