using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Communications.Settings;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Account.Client.Services;
using Ironwall.Libraries.Account.Common.Models;
using Ironwall.Libraries.Account.Common.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Components;
using Ironwall.Libraries.Event.UI.ViewModels;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Libraries.Events.Services;
using Ironwall.Libraries.Tcp.Client.Services;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Common.Proivders;
using Ironwall.Libraries.Utils.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Framework.Models.Communications.Symbols;
using Ironwall.Libraries.Map.Common.Services;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using System.Windows;
using Ironwall.Libraries.Map.Common.Helpers;
using System.Windows.Interop;
using Ironwall.Libraries.Common.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Client.Services
{
    public class ClientService : AccountClientService
    {
        #region - Ctors -
        public ClientService(
            TcpSetupModel tcpSetupModel
            , AccountDbService accountDbService
            , EventDbService eventDbService
            , DetectionEventProvider detectionEventProvider
            , ActionEventProvider actionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider

            , DeviceProvider deviceProvider
            , DeviceDbService deviceDbService
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraMappingProvider cameraMappingProvider

            , EventProvider eventProvider
            , PreEventProvider preEventProvider

            , MapDbService mapDbService

            , MapProvider mapProvider
            , SymbolProvider symbolProvider
            , PointProvider pointProvider

            , DeviceInfoModel deviceInfoModel
            , SymbolInfoModel symbolInfoModel
            , MappingInfoModel mappingInfoModel
            , LogProvider logProvider
            ) : base(tcpSetupModel)
        {
            _eventDbService = eventDbService;
            _detectionEventProvider = detectionEventProvider;
            _actionEventProvider = actionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _eventProvider = eventProvider;

            _preEventProvider = preEventProvider;

            _deviceProvider = deviceProvider;
            _deviceDbService = deviceDbService;
            _controllerDeviceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeivceProvider = cameraDeviceProvider;
            _cameraPresetProvider = cameraPresetProvider;
            _cameraMappingProvider = cameraMappingProvider;

            _mapDbService = mapDbService;
            _mapProvider = mapProvider;
            _symbolProvider = symbolProvider;
            _pointProvider = pointProvider;

            _deviceInfoModel = deviceInfoModel;
            _symbolInfoModel = symbolInfoModel;
            _mappingInfoModel = mappingInfoModel;

            _logProvider = logProvider;
        }
        #endregion
        #region - Implementation of Interface -
        public Task Connection(ITcpServerModel model)
        {
            return Task.Run(() =>
            {
                SetServerIPEndPoint(model);
                InitSocket();
            });
        }

        public Task Disconnection()
        {
            return Task.Run(() =>
            {
                CloseSocket();
            });
        }

        #region Deprecated Code
        //protected override async void ConnectionTick(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]ClientService Tick...");
        //        bool isHeartBeatSent = false;
        //        //bool isLoginSessionProcess = false;
        //        if (DateTime.Now > HeartBeatExpireTime - TimeSpan.FromSeconds(10.5))
        //        {
        //            isHeartBeatSent = await SendHeartBeat();
        //            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]SendHeartBeat result : {isHeartBeatSent}");
        //        }

        //        if(LoginSessionModel != null)
        //            await LoginSessionProcess();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Raised Exception in {nameof(ConnectionTick)} : {ex.Message}");
        //    }
        //}

        //private Task<bool> SendHeartBeat()
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (!Socket.Connected)
        //            {
        //                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Socket is connected :{Socket.Connected} in {nameof(SendHeartBeat)}");
        //                return false;
        //            }

        //            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Heartbeat...");

        //            var iPEndPoint  = Socket.LocalEndPoint as IPEndPoint;
        //            //var requestModel = RequestFactory.Build<HeartBeatRequestModel>(SetupModel.ClientIp, SetupModel.ClientPort);
        //            var requestModel = RequestFactory.Build<HeartBeatRequestModel>(iPEndPoint.Address.ToString(), iPEndPoint.Port);
        //            var msg = JsonConvert.SerializeObject(requestModel);
        //            SendRequest(msg);
        //            HeartBeatExpireTime += TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat);
        //            return true;

        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(SendHeartBeat)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //private Task LoginSessionProcess()
        //{
        //    return Task.Run(async () =>
        //    {
        //        try
        //        {

        //            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Session Tick...{(int)(DateTime.Parse(LoginSessionModel?.TimeExpired) - DateTime.Now).TotalSeconds}");


        //            if (DateTime.Parse(LoginSessionModel?.TimeExpired) - DateTime.Now < TimeSpan.FromSeconds(10))
        //            {
        //                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Session was expired!");

        //                var requestModel = RequestFactory.Build<KeepAliveRequestModel>(LoginSessionModel.Token);
        //                await KeepAliveRequest(requestModel);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(LoginSessionProcess)} : {ex.Message}");
        //        }
        //    });
        //}


        //public Task<bool> LoginResponse(ILoginResponseModel response, IPEndPoint endPoint)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (!response.Success)
        //                return false;

        //            UserModel = ModelFactory.Build<UserModel>(response?.Results?.Details);
        //            LoginSessionModel = ModelFactory.Build<LoginSessionModel>(response?.Results);
        //            SessionTimeOut = (int)response?.Results?.SessionTimeOut;

        //            CallRefresh?.Invoke();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(LoginResponse)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}


        //public Task LogoutRequest(ILogoutRequestModel requestModel)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            var msg = JsonConvert.SerializeObject(requestModel);
        //            SendRequest(msg);

        //            CallRefresh?.Invoke();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(LogoutRequest)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}


        //public Task RegisterRequest(IAccountRegisterRequestModel requestModel)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            var msg = JsonConvert.SerializeObject(requestModel);
        //            SendRequest(msg);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(RegisterRequest)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //public Task<bool> RegisterResponse(AccountRegisterResponseModel response, IPEndPoint endPoint)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            if (!response.Success)
        //                return false;

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(RegisterResponse)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //public Task EditRequest(IAccountEditRequestModel requestModel)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            var msg = JsonConvert.SerializeObject(requestModel);
        //            SendRequest(msg);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(RegisterRequest)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //public Task<bool> EditResponse(AccountInfoResponseModel response, IPEndPoint endPoint)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            if (!response.Success)
        //                return false;

        //            var userModel = InstanceFactory.Build<UserModel>();
        //            userModel.Insert(response?.Details);

        //            UserModel = userModel;
        //            CallRefresh?.Invoke();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(EditResponse)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //public Task DeleteRequest(IAccountDeleteRequestModel requestModel)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            var msg = JsonConvert.SerializeObject(requestModel);
        //            SendRequest(msg);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(RegisterRequest)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}

        //public Task<bool> DeleteResponse(AccountDeleteResponseModel response, IPEndPoint endPoint)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (LoginSessionModel == null)
        //                return false;

        //            if (!response.Success)
        //                return false;

        //            UserModel = null;
        //            LoginSessionModel = null;

        //            SocketReceived($"{response.Message}", endPoint);
        //            CallRefresh?.Invoke();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(DeleteResponse)} : {ex.Message}");
        //            return false;
        //        }
        //    });
        //}
        #endregion

        public Task<bool> ConnectionRequest(IConnectionRequestModel request, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (LoginSessionModel == null) return false;

                    //IBaseDeviceModel device = _sensorDeviceProvider
                    //.Where(sensor => sensor.DeviceNumber == request.Sensor
                    //&& (sensor as ISensorDeviceModel).CONTROLLER.DeviceNumber == request.CONTROLLER)
                    //.FirstOrDefault();

                    //var eventModel = ModelFactory.Build<ConnectionEventModel>(request, device);
                    //_eventProvider.Add(eventModel);
                    //_eventProvider.Finished();

                    //await ConnectionResponse(true, $"{((EnumEventType)eventModel.MessageType).ToString()} Event({eventModel.Id}) was successfully received!", request);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DetectionRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ConnectionResponse(bool success, string msg, IConnectionRequestModel request, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!success)
                        throw new TcpResponseFailException(success, msg);

                    var model = ResponseFactory.Build<ConnectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    Debug.WriteLine($"Request Message was fail in {nameof(ConnectionResponse)} : {ex.Message}");
                    var model = ResponseFactory.Build<ConnectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ConnectionResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<DetectionEventModel> DetectionRequest(IDetectionRequestModel request, IPEndPoint endPoint)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return null;

                    IBaseDeviceModel device = _sensorDeviceProvider
                    .Where(sensor => sensor.DeviceNumber == request.Sensor
                    && (sensor as ISensorDeviceModel).Controller.DeviceNumber == request.Controller)
                    .FirstOrDefault();

                    var eventModel = ModelFactory.Build<DetectionEventModel>(request, device);
                    //_eventProvider.Add(eventModel);
                    //await _eventProvider.Finished();
                    await _eventProvider.InsertedItem(eventModel);

                    await DetectionResponse(true, $"{((EnumEventType)eventModel.MessageType).ToString()} Event({eventModel.Id}) was successfully received!", request);

                    return eventModel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DetectionRequest)} : {ex.Message}");
                    return null;
                }
            });
        }

        public Task<bool> DetectionResponse(bool success, string msg, IDetectionRequestModel request, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!success)
                        throw new TcpResponseFailException(success, msg);

                    var model = ResponseFactory.Build<DetectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    Debug.WriteLine($"Request Message was fail in {nameof(DetectionResponse)} : {ex.Message}");
                    var model = ResponseFactory.Build<DetectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DetectionResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<MalfunctionEventModel> MalfunctionRequest(IMalfunctionRequestModel request, IPEndPoint endPoint)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return null;

                    IBaseDeviceModel device = null;

                    if (request.Sensor != 0)
                        device = _sensorDeviceProvider
                        .Where(sensor => sensor.DeviceNumber == request.Sensor
                        && (sensor as ISensorDeviceModel).Controller.DeviceNumber == request.Controller)
                        .FirstOrDefault();
                    else
                        device = _controllerDeviceProvider.Where(con => con.DeviceNumber == request.Controller).FirstOrDefault();

                    if (device == null)
                    {
                        var controller = _controllerDeviceProvider.CollectionEntity.Count();
                        var sensor = _sensorDeviceProvider.CollectionEntity.Count();
                        var message = $"일치하는 장비의 정보가 없습니다.(제어기 : {controller}, 센서 : {sensor}, 기타장비 : {0}";
                        throw new DeviceMatchFailException(message:message, controller:controller, sensor:sensor);
                    }


                    var eventModel = ModelFactory.Build<MalfunctionEventModel>(request, device);
                    _eventProvider.Add(eventModel);
                    await _eventProvider.Finished();

                    await MalfunctionResponse(true, $"{((EnumEventType)eventModel.MessageType).ToString()} Event({eventModel.Id}) was successfully received!", request);
                    return eventModel;
                }
                catch(DeviceMatchFailException ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MalfunctionRequest)} : {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MalfunctionRequest)} : {ex.Message}");
                    return null;
                }
            });
        }

        public Task<bool> MalfunctionResponse(bool success, string msg, IMalfunctionRequestModel request, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!success)
                        throw new TcpResponseFailException(success, msg);

                    var model = ResponseFactory.Build<MalfunctionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    Debug.WriteLine($"Request Message was fail in {nameof(MalfunctionResponse)} : {ex.Message}");
                    var model = ResponseFactory.Build<MalfunctionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MalfunctionResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task ActionRequest(IActionRequestModel model, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    //IMetaEventModel eventModel = _eventProvider.Where(e => e.Id == request.EventId).FirstOrDefault();

                    //var actionModel = ModelFactory.Build<ActionEventModel>(request, eventModel);
                    //_actionEventProvider.Add(actionModel);
                    //await _actionEventProvider.Finished();
                    var json = JsonConvert.SerializeObject(model);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ActionRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task ActionRequest(IMetaEventModel metaEvent, string content, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    ActionRequestModel requestModel = null;

                    switch ((EnumEventType)metaEvent?.MessageType)
                    {
                        case EnumEventType.Intrusion:
                            {
                                var eventModel = metaEvent as IDetectionEventModel;
                                requestModel = RequestFactory.Build<ActionRequestModel>(content, UserModel.IdUser, eventModel);
                            }
                            break;
                        case EnumEventType.ContactOn:
                            break;
                        case EnumEventType.ContactOff:
                            break;
                        case EnumEventType.Connection:
                            break;
                        case EnumEventType.Action:
                            break;
                        case EnumEventType.Fault:
                            {
                                var eventModel = ModelFactory.Build<MalfunctionEventModel>(metaEvent as IMalfunctionEventViewModel);
                                requestModel = RequestFactory.Build<ActionRequestModel>(content, UserModel.IdUser, eventModel);
                            }
                            break;
                        case EnumEventType.WindyMode:
                            break;
                        default:
                            break;
                    }


                    //var actionModel = ModelFactory.Build<ActionEventModel>(requestModel, metaEvent);
                    //_actionEventProvider.Add(actionModel);
                    //await _actionEventProvider.Finished();

                    //await ActionRequest(requestModel, endPoint);
                    var json = JsonConvert.SerializeObject(requestModel);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ActionRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ActionResponse(IActionResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!response.Success)
                        return false;


                    IMetaEventModel eventModel = _eventProvider.Where(e => e.Id == response.RequestModel.EventId).FirstOrDefault();

                    var actionModel = ModelFactory.Build<ActionEventModel>(response.RequestModel, eventModel);
                    await _actionEventProvider.InsertedItem(actionModel);
                    //await _actionEventProvider.Finished();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ActionResponse)} : {ex.Message}");
                    return false;
                }
            });
        }


        public Task SearchEventRequest(ISearchDetectionRequestModel requestModel, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(requestModel);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchEventRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task SearchEventRequest(ISearchMalfunctionRequestModel requestModel, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(requestModel);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchEventRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task SearchEventRequest(ISearchActionRequestModel requestModel, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(requestModel);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchEventRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeviceInfoRequest(IDeviceInfoRequestModel requestDeviceInfo)
        {
            return Task.Run(async() =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(requestDeviceInfo);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DeviceInfoRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeviceInfoResponse(IDeviceInfoResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var updateTime = response.Detail.UpdateTime;
                    
                    Debug.WriteLine($"==>클라이언트 DB의 DeviceInfo 정보({_deviceInfoModel.UpdateTime})");
                    if (updateTime > _deviceInfoModel.UpdateTime)
                    {
                        Debug.WriteLine($"==>서버에 최신화된 DeviceInfo 정보({response.Detail.UpdateTime})");
                        await _deviceDbService.UpdateDeviceInfo(response.Detail);
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ControllerDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeviceDataRequest(IDeviceDataRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DeviceDataRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeviceDataResponse(IDeviceDataResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _logProvider.AddInfo($"The Server responsed to DeviceDataRequest with Data(Controller : {response.Sensors.Count}, Sensor : {response.Controllers.Count}, Camera : {response.Cameras.Count}).");

                    await _deviceProvider.ClearData();

                    foreach (var item in response.Controllers)
                    {
                        _deviceProvider.Add(item);
                    }

                    foreach (var item in response.Sensors)
                    {
                        _deviceProvider.Add(item);
                    }

                    foreach (var item in response.Cameras)
                    {
                        _deviceProvider.Add(item);
                    }
                    
                    bool finishedResult = await _deviceProvider.Finished();
                    if (finishedResult)
                    {
                        await _deviceDbService.SaveControllers();
                        await _deviceDbService.SaveSensors();
                        await _deviceDbService.SaveCameras();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    var msg = $"Raised Exception in {nameof(DeviceDataResponse)} : {ex.Message}";
                    _logProvider.AddError(msg);
                    Debug.WriteLine(msg);
                    return false;
                }
            });
        }


        public Task<bool> ControllerDataRequest(IControllerDataRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ControllerDataResponse(IControllerDataResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var list = _deviceProvider.Where(t => t.DeviceType == EnumDeviceType.Controller).ToList();

                    foreach (var item in list)
                    {
                        await _deviceProvider.DeletedItem(item);
                    }

                    List<ControllerDeviceModel> controllers = response.Controllers;

                    foreach (var item in controllers)
                    {
                        _deviceProvider.Add(item);
                    }

                    bool finishedResult = await _deviceProvider.Finished();
                    if (finishedResult)
                    {
                        await _deviceDbService.SaveControllers();
                    }
                    
                    Debug.WriteLine($"DeviceProvider(Controllers) Finished Result : {finishedResult}");

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ControllerDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SensorDataRequest(ISensorDataRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SensorDataRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SensorDataResponse(ISensorDataResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {

                    if (!response.Success)
                        return false;

                    var list = _deviceProvider.OfType<ISensorDeviceModel>().ToList();

                    foreach (var item in list)
                    {
                        await _deviceProvider.DeletedItem(item);
                    }

                    List<SensorDeviceModel> sensors = response.Sensors;

                    foreach (var item in sensors)
                    {
                        _deviceProvider.Add(item);
                    }

                    bool finishedResult = await _deviceProvider.Finished();

                    if (finishedResult)
                    {
                        await _deviceDbService.SaveSensors();
                    }

                    Debug.WriteLine($"DeviceProvider(Sensors) Finished Result : {finishedResult}");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SensorDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataRequest(ICameraDataRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataResponse(ICameraDataResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var list = _deviceProvider.OfType<ICameraDeviceModel>().ToList();

                    foreach (var item in list)
                    {
                        _deviceProvider.Remove(item);
                    }

                    List<CameraDeviceModel> cameras = response.Cameras;

                    foreach (var item in cameras)
                    {
                        _deviceProvider.Add(item);
                    }

                    bool finishedResult = await _deviceProvider.Finished();

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataSaveRequest(ICameraDataSaveRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataSaveRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataSaveResponse(ICameraDataSaveResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    await _deviceDbService.SaveCameras();

                    await _deviceDbService.UpdateDeviceInfo(response.Detail);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraMappingInfoRequest(ICameraMappingInfoRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingInfoRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraMappingInfoResponse(ICameraMappingInfoResponseModel response)
        {
            return Task.Run(async () => 
            {
                try
                {
                    if (!response.Success) return false;

                    var updateTime = response.Detail.UpdateTime;

                    Debug.WriteLine($"==>클라이언트 DB의 MappingInfo 정보({_mappingInfoModel.UpdateTime})");
                    if (updateTime > _mappingInfoModel.UpdateTime)
                    {
                        Debug.WriteLine($"==>서버에 최신화된 MappingInfo 정보({response.Detail.UpdateTime})");
                        //var model = new MappingInfoModel(response.Detail.Mapping, response.Detail.UpdateTime);
                        //ModelFactory.Build<MappingInfoModel>(response.Detail.Mapping, response.Detail.UpdateTime);
                        await _deviceDbService.UpdateMappingInfo(response.Detail);
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
            
        }

        public Task<bool> CameraMappingRequest(ICameraMappingRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraMappingResponse(ICameraMappingResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _cameraMappingProvider.Clear();
                    foreach (var item in response.List)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    await _cameraMappingProvider.Finished();
                    //await _deviceDbService.SaveCameraMappings(isFinished:true);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraMappingSaveRequest(ICameraMappingSaveRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingSaveRequest)} : {ex.Message}");
                    return false;
                }
            });
        }
        
        

        public Task<bool> CameraMappingSaveResponse(ICameraMappingSaveResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    await _deviceDbService.SaveCameraMappings();

                    await _deviceDbService.UpdateMappingInfo(response.Detail);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetRequest(ICameraPresetRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetResponse(ICameraPresetResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _cameraPresetProvider.Clear();
                    foreach (var item in response.List)
                    {
                        _cameraPresetProvider.Add(item);
                    }

                    await _cameraPresetProvider.Finished();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetSaveRequest(ICameraPresetSaveRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingSaveRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetSaveResponse(ICameraPresetSaveResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    //await _deviceDbService.UpdateMappingInfo(response.Detail);
                    await _deviceDbService.SaveCameraPresets();

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SymbolInfoRequest(ISymbolInfoRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolInfoRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SymbolInfoResponse(ISymbolInfoResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var updateTime = response.Detail.UpdateTime;

                    Debug.WriteLine($"==>클라이언트 DB의 SymbolInfo 정보({_symbolInfoModel.UpdateTime})");
                    if (updateTime > _symbolInfoModel.UpdateTime)
                    {
                        Debug.WriteLine($"==>서버에 최신화된 SymbolInfo 정보({response.Detail.UpdateTime})");
                        await _mapDbService.UpdateSymbolInfo(response.Detail);
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolInfoResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SymbolDataLoadRequest(ISymbolDataLoadRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolDataLoadRequest)} : {ex.Message}");
                    return false;
                }
            });
        }
        public Task<bool> SymbolDataLoadResponse(ISymbolDataLoadResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success) return false;

                    List<MapModel> maps = response.Maps;
                    List<PointClass> points = response.Points;
                    List<SymbolModel> symbols = response.Symbols;
                    List<ShapeSymbolModel> shapes = response.Shapes;
                    List<ObjectShapeModel> objects = response.Objects;


                    _logProvider.AddInfo($"SymbolDataLoadResponse");

                    _pointProvider.Clear();
                    foreach (var item in points)
                    {
                        _pointProvider.Add(item);
                    }

                    

                    bool mapFinishedResult = await _mapProvider.Finished();
                    if(mapFinishedResult)
                    {
                        await _mapDbService.SaveMaps();
                    }

                    await _symbolProvider.ClearData();

                    ///Symbol Data Insert
                    foreach (var item in symbols)
                    {
                        if (!SymbolHelper.IsSymbolCategory(item.TypeShape)) continue;
                        
                        var model = item as ISymbolModel;
                        _symbolProvider.Add(model);
                    }

                    ///Shape Data Insert
                    foreach (var item in shapes)
                    {
                        if(!SymbolHelper.IsShapeCategory(item.TypeShape)) continue;

                        var model = item as IShapeSymbolModel;
                        if (model.TypeShape == (int)EnumShapeType.POLYLINE)
                        {
                            var pointElements = _pointProvider
                                                .Where(p => p.PointGroup == model.Id)
                                                .ToList().OrderBy(p => p.Sequence);
                            var pointList = new List<Point>();
                            pointList.AddRange(pointElements.Select(p => new Point(p.X, p.Y)));
                            model.Points = new System.Windows.Media.PointCollection(pointList);
                            model.Points.Freeze();
                        }
                        _symbolProvider.Add(model);
                    }

                    ///Object Data Insert
                    foreach (var item in objects)
                    {
                        if (!SymbolHelper.IsObjectCategory(item.TypeShape)) continue;
                        
                        var model = item as IObjectShapeModel;
                        if (model.TypeShape == (int)EnumShapeType.FENCE)
                        {
                            var pointElements = _pointProvider
                                                .Where(p => p.PointGroup == model.Id)
                                                .ToList().OrderBy(p => p.Sequence);
                            var pointList = new List<Point>();
                            pointList.AddRange(pointElements.Select(p => new Point(p.X, p.Y)));
                            model.Points = new System.Windows.Media.PointCollection(pointList);
                            model.Points.Freeze();
                        }
                        _symbolProvider.Add(model);
                    }

                    bool symbolFinishedResult = await _symbolProvider.Finished();
                    if (symbolFinishedResult)
                    {
                        await _mapDbService.SaveAllSymbols();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    var msg = $"Raised Exception in {nameof(SymbolDataLoadResponse)} : {ex.Message}";
                    _logProvider.AddError(msg);
                    Debug.WriteLine(msg);
                    return false;
                }
            });
        }

        public Task<bool> SymbolDataSaveRequest(ISymbolDataSaveRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolDataSaveRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SymbolDataSaveResponse(ISymbolDataSaveResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success) return false;

                    var model = ModelFactory.Build<SymbolDetailModel>(response.Detail.Map, response.Detail.Symbol, response.Detail.ShapeSymbol, response.Detail.ObjectShape, response.Detail.UpdateTime);

                    await _mapDbService.UpdateSymbolInfo(model);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolDataSaveResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> MapFileLoadRequest(IMapFileLoadRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MapFileLoadRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> MapFileLoadResponse(IMapFileLoadResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    await _mapProvider.ClearData();

                    foreach (var item in response.Maps)
                    {
                        _mapProvider.Add(item);

                        _logProvider.AddInfo($"Map file was loaed : {item.MapName}");

                        if (!MapHelper.IsMapFileExist(item.Url))
                        {
                            _logProvider.AddInfo($"{item.FileName} was not exist! then reqeust {item.FileName} from server!");
                            //var requestModel = RequestFactory.MapFileRequestBuild<MapFileRequestModel>(LoginSessionModel, item);
                            //var requestModel = RequestFactory.Build<MapFileLoadRequestModel>(LoginSessionModel, item);
                            //await MapFileLoadRequest(requestModel);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MapFileLoadResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> MapFileSaveRequest(IMapFileSaveRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MapFileSaveRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> MapFileSaveResponse(IMapFileSaveResponseModel response)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!response.Success) return false;

                    foreach (var item in response.Detail.Maps)
                    {
                        if (!_mapProvider.Any(entity => entity.IsEqual(item)))
                        {
                            _logProvider.AddInfo($"Map file was not saved : {item.MapName}");
                            continue;
                        }

                        _logProvider.AddInfo($"Map file was saved : {item.MapName}");
                    }
                    var map = response.Detail.Maps.Count();
                    var updateTime = response.Detail.UpdateTime;
                    var symbolDetailModel = ResponseFactory.Build<SymbolDetailModel>(map, _symbolInfoModel.Symbol, _symbolInfoModel.ShapeSymbol, _symbolInfoModel.ObjectShape, updateTime);

                    await _mapDbService.UpdateSymbolInfo(symbolDetailModel);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MapFileSaveResponse)} : {ex.Message}");
                    return false;
                }
            });
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void AccountInfoClear()
        {
            try
            {
                SessionTimeOut = 0;
                UserModel = null;
                LoginSessionModel = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int ControllerCount { get; set; }
        public int SensorCount { get; set; }
        public int CameraCount { get; set; }
        public DateTime DeviceUpdateTime { get; private set; }

        //public AccountStatusModel AccountStatus { get; set; }

        #endregion
        #region - Attributes -
        private EventDbService _eventDbService;
        private DetectionEventProvider _detectionEventProvider;
        private ActionEventProvider _actionEventProvider;
        private MalfunctionEventProvider _malfunctionEventProvider;
        private EventProvider _eventProvider;
        private PreEventProvider _preEventProvider;
        private DeviceProvider _deviceProvider;
        private DeviceDbService _deviceDbService;
        private ControllerDeviceProvider _controllerDeviceProvider;
        private SensorDeviceProvider _sensorDeviceProvider;
        private CameraDeviceProvider _cameraDeivceProvider;
        private CameraPresetProvider _cameraPresetProvider;
        private CameraMappingProvider _cameraMappingProvider;
        private MapDbService _mapDbService;
        private MapProvider _mapProvider;
        private SymbolProvider _symbolProvider;
        private PointProvider _pointProvider;
        private DeviceInfoModel _deviceInfoModel;
        private SymbolInfoModel _symbolInfoModel;
        private MappingInfoModel _mappingInfoModel;
        private LogProvider _logProvider;

        #endregion

    }
}
