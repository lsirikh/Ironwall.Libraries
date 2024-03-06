using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Communications.Settings;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Account.Server.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Libraries.Events.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Server.Services
{
    public interface IServerService : IAccountServerService
    {
       
        DetectionEventProvider DetectionEventProvider { get; }
        MalfunctionEventProvider MalfunctionEventProvider { get; }
        ActionEventProvider ActionEventProvider { get; }

        EventDbService EventDbService { get; }
        EventProvider EventProvider { get; }

        Task<bool> ActionEventRequest(IActionRequestModel model, IPEndPoint endPoint = null);
        Task ActionEventResponse(bool success, string msg, IActionRequestModel model = null, IPEndPoint endPoint = null);
        Task CameraDataRequest(ICameraDataRequestModel model, IPEndPoint endPoint);
        Task CameraDataResponse(bool success, string msg, List<CameraDeviceModel> list, IPEndPoint endPoint = null);
        Task ConnectionEventRequest(IConnectionRequestModel model, IPEndPoint endPoint = null);
        Task ControllerDataRequest(IControllerDataRequestModel model, IPEndPoint endPoint);
        Task ControllerDataResponse(bool success, string msg, List<ControllerDeviceModel> list, IPEndPoint endPoint = null);
        Task DetectionEventRequest(IDetectionRequestModel model, IPEndPoint endPoint = null);
        Task DeviceDataRequest(IDeviceDataRequestModel model, IPEndPoint endPoint);
        Task DeviceDataResponse(bool success, string msg, List<ControllerDeviceModel> contrllerList, List<SensorDeviceModel> sensorList, List<CameraDeviceModel> cameraList, IPEndPoint endPoint = null);
        Task DeviceInfoRequest(IDeviceInfoRequestModel model, IPEndPoint endPoint);
        Task DeviceInfoResponse(bool success, string msg, IDeviceDetailModel model, IPEndPoint endPoint = null);
        Task MalfunctionEventRequest(IMalfunctionRequestModel model, IPEndPoint endPoint = null);
        Task SensorDataRequest(ISensorDataRequestModel model, IPEndPoint endPoint);
        Task SensorDataResponse(bool success, string msg, List<SensorDeviceModel> list, IPEndPoint endPoint = null);
        Task UpdateHeartBeat(IHeartBeatRequestModel model, IPEndPoint endPoint);
    }
}