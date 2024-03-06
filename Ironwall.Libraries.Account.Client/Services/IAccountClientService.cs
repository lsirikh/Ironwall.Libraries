using Ironwall.Framework.Events;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Accounts;
using System.Net;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Client.Services
{
    public interface IAccountClientService
    {
        LoginSessionModel LoginSessionModel { get; set; }
        int SessionTimeOut { get; }
        UserModel UserModel { get; set; }

        event EventHelper.EventDelegate CallRefresh;

        Task CheckIdRequest(IAccountIdCheckRequestModel requestModel);
        Task CheckIdResponse(IResponseModel requestModel, IPEndPoint endPoint);
        Task DeleteRequest(IAccountDeleteRequestModel requestModel);
        Task<bool> DeleteResponse(IAccountDeleteResponseModel response, IPEndPoint endPoint);
        void DisposeSessionTimer();
        Task EditRequest(IAccountEditRequestModel requestModel);
        Task<bool> EditResponse(IAccountInfoResponseModel response, IPEndPoint endPoint);
        bool GetSessionTimerEnable();
        double GetSessionTimerInterval();
        void InitSessionTimer();
        Task KeepAliveRequest(IKeepAliveRequestModel requestModel);
        Task<bool> KeepAliveResponse(IKeepAliveResponseModel response, IPEndPoint endPoint);
        Task LoginRequest(ILoginRequestModel requestModel);
        Task<bool> LoginResponse(ILoginResponseModel response, IPEndPoint endPoint);
        Task LogoutRequest(ILogoutRequestModel requestModel);
        Task<bool> LogoutResponse(ILogoutResponseModel response, IPEndPoint endPoint);
        Task RegisterRequest(IAccountRegisterRequestModel requestModel);
        Task<bool> RegisterResponse(IAccountRegisterResponseModel response, IPEndPoint endPoint);
        void SetSessionTimerEnable(bool value);
        void SetSessionTimerInterval(int time = 1000);
        void SetSessionTimerStart();
        void SetSessionTimerStop();
    }
}