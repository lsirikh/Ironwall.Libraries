using Ironwall.Framework.Events;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System.Net;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Accounts.Services
{
    public interface IAccountClientService
    {
        LoginSessionModel LoginSessionModel { get; set; }
        int SessionTimeOut { get; }
        UserModel UserModel { get; set; }

        event EventHelper.EventDelegate CallRefresh;

        Task Connection(TcpServerModel model);
        Task<bool> DeleteResponse(AccountDeleteResponseModel response, IPEndPoint endPoint);
        Task Disconnection();
        Task<bool> EditResponse(AccountInfoResponseModel response, IPEndPoint endPoint);
        Task KeepAliveResponse(KeepAliveResponseModel response, IPEndPoint endPoint);
        Task<bool> LoginResponse(LoginResponseModel response, IPEndPoint endPoint);
        Task<bool> LogoutResponse(LogoutResponseModel response, IPEndPoint endPoint);
        Task<bool> RegisterResponse(AccountRegisterResponseModel response, IPEndPoint endPoint);
    }
}