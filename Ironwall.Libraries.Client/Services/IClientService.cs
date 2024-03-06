using Ironwall.Framework.Events;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Libraries.Tcp.Common.Models;
using System.Net;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Client.Services
{
    public interface IClientService
    {
        int CameraCount { get; set; }
        int ControllerCount { get; set; }
        LoginSessionModel LoginSessionModel { get; set; }
        int SensorCount { get; set; }
        int SessionTimeOut { get; }
        UserModel UserModel { get; set; }

        event EventHelper.EventDelegate CallRefresh;

        Task ActionRequest(IActionRequestModel request, IPEndPoint endPoint);
        Task<bool> CameraDataRequest(ICameraDataRequestModel request);
        Task<bool> CameraDataResponse(ICameraDataResponseModel response);
        Task Connection(ITcpServerModel model);
        Task<bool> ControllerDataRequest(IControllerDataRequestModel request);
        Task<bool> ControllerDataResponse(IControllerDataResponseModel response);
        Task DeleteRequest(IAccountDeleteRequestModel requestModel);
        Task<bool> DeleteResponse(AccountDeleteResponseModel response, IPEndPoint endPoint);
        Task<bool> DetectionRequest(IDetectionRequestModel request, IPEndPoint endPoint);
        Task<bool> DeviceInfoRequest(IDeviceInfoRequestModel requestDeviceInfo);
        Task<bool> DeviceInfoResponse(IDeviceInfoResponseModel response);
        Task Disconnection();
        Task EditRequest(IAccountEditRequestModel requestModel);
        Task<bool> EditResponse(AccountInfoResponseModel response, IPEndPoint endPoint);
        Task KeepAliveRequest(IKeepAliveRequestModel requestModel);
        Task<bool> KeepAliveResponse(IKeepAliveResponseModel response, IPEndPoint endPoint);
        Task LoginRequest(ILoginRequestModel requestModel);
        Task<bool> LoginResponse(ILoginResponseModel response, IPEndPoint endPoint);
        Task LogoutRequest(ILogoutRequestModel requestModel);
        Task<bool> LogoutResponse(ILogoutResponseModel response, IPEndPoint endPoint);
        Task MalfunctionRequest(IMalfunctionRequestModel request, IPEndPoint endPoint);
        Task RegisterRequest(IAccountRegisterRequestModel requestModel);
        Task<bool> RegisterResponse(AccountRegisterResponseModel response, IPEndPoint endPoint);
        bool SendReqeust(IBaseMessageModel request);
        Task<bool> SensorDataRequest(ISensorDataRequestModel request);
        Task<bool> SensorDataResponse(ISensorDataResponseModel response);
    }
}