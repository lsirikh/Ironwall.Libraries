using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Account.Client.Services;
using Ironwall.Libraries.Account.Common.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Utils.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Communications.Symbols;
using Ironwall.Libraries.Map.Common.Services;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using System.Windows;
using Ironwall.Libraries.Map.Common.Helpers;
using Ironwall.Libraries.Common.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Ironwall.Framework.Models.Communications.VmsApis;
using Ironwall.Libraries.Base.Services;
using System.Threading;

namespace Ironwall.Libraries.Client.Services
{
    public class ClientService : AccountClientService
    {
        #region - Ctors -
        public ClientService(
            ILogService log
            , TcpSetupModel tcpSetupModel
            , AccountDbService accountDbService
            //, EventDbService eventDbService
            , DetectionEventProvider detectionEventProvider
            , ActionEventProvider actionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider

            , DeviceProvider deviceProvider
            //, DeviceDbService deviceDbService
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider

            , CameraOptionProvider cameraOptionProvider
            , CameraProfileProvider cameraProfileProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraMappingProvider cameraMappingProvider

            , EventProvider eventProvider
            , PreEventProvider preEventProvider

            , MapDbService mapDbService

            , MapProvider mapProvider
            , SymbolProvider symbolProvider
            , PointProvider pointProvider

            , SymbolInfoModel symbolInfoModel

            , VmsApiProvider vmsApiProvider
            , VmsEventProvider vmsEventProvider
            , VmsMappingProvider vmsMappingProvider
            , VmsSensorProvider vmsSensorProvider
            , LogProvider logProvider
            ) : base(log, tcpSetupModel)
        {
            _detectionEventProvider = detectionEventProvider;
            _actionEventProvider = actionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _eventProvider = eventProvider;

            _preEventProvider = preEventProvider;

            _deviceProvider = deviceProvider;
            _controllerDeviceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeivceProvider = cameraDeviceProvider;

            _cameraOptionProvider = cameraOptionProvider;
            _cameraProfileProvider = cameraProfileProvider;
            _cameraPresetProvider = cameraPresetProvider;
            _cameraMappingProvider = cameraMappingProvider;

            _mapDbService = mapDbService;
            _mapProvider = mapProvider;
            _symbolProvider = symbolProvider;
            _pointProvider = pointProvider;

            _symbolInfoModel = symbolInfoModel;


            _vmsApiProvider = vmsApiProvider;
            _vmsEventProvider = vmsEventProvider;
            _vmsMappingProvider = vmsMappingProvider;
            _vmsSensorProvider = vmsSensorProvider;

            _logProvider = logProvider;

           
        }
        #endregion
        #region - Implementation of Interface -
        public Task Connection(ITcpServerModel model, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                SetServerIPEndPoint(model);
                InitSocket();
            }, token);
        }

        public Task Disconnection()
        {
            return Task.Run(() =>
            {
                CloseSocket();
            });
        }

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
                    _log.Error($"Raised Exception in {nameof(ConnectionRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    _log.Error($"Raised Exception in {nameof(ConnectionResponse)} of {nameof(ClientService)} : {ex.Message}");
                    var model = ResponseFactory.Build<ConnectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ConnectionResponse)} of {nameof(ClientService)} : {ex.Message}");
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

                    var eventModel = new DetectionEventModel(request.Body);
                    _eventProvider.Add(eventModel);

                    await DetectionResponse(true, $"{((EnumEventType)eventModel.MessageType).ToString()} Event({eventModel.Id}) was successfully received!", request);

                    return eventModel;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(DetectionRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    _log.Error($"Raised Exception in {nameof(DetectionResponse)} of {nameof(ClientService)} : {ex.Message}");
                    var model = ResponseFactory.Build<DetectionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(DetectionResponse)} of {nameof(ClientService)} : {ex.Message}");
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


                    if (device == null)
                    {
                        var controller = _controllerDeviceProvider.CollectionEntity.Count();
                        var sensor = _sensorDeviceProvider.CollectionEntity.Count();
                        var message = $"일치하는 장비의 정보가 없습니다.(제어기 : {controller}, 센서 : {sensor}, 기타장비 : {0}";
                        throw new DeviceMatchFailException(message:message, controller:controller, sensor:sensor);
                    }
                    var eventModel = ModelFactory.Build<MalfunctionEventModel>(request, device);
                    _eventProvider.Add(eventModel);

                    await MalfunctionResponse(true, $"{((EnumEventType)eventModel.MessageType).ToString()} Event({eventModel.Id}) was successfully received!", request);
                    return eventModel;
                }
                catch(DeviceMatchFailException ex)
                {
                    _log.Error($"Raised Exception in {nameof(MalfunctionRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(MalfunctionRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (TcpResponseFailException ex)
                {
                    _log.Error($"Raised Exception in {nameof(MalfunctionResponse)} of {nameof(ClientService)} : {ex.Message}");
                    var model = ResponseFactory.Build<MalfunctionResponseModel>(success, msg, request);
                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(MalfunctionResponse)} of {nameof(ClientService)} : {ex.Message}");
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

                    var json = JsonConvert.SerializeObject(model, _settings);
                    await SendRequest(json, endPoint);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ActionRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ActionRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ActionResponse(IActionResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!response.Success)
                        return false;


                    IMetaEventModel eventModel = _eventProvider.Where(e => e.Id == response.Body.FromEvent.Id).FirstOrDefault();
                    var actionModel = new ActionEventModel(response.Body);
                    _actionEventProvider.Add(actionModel);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ActionResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchEventRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchEventRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchEventRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(DeviceDataRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeviceDataResponse(IDeviceDataResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _logProvider.AddInfo($"The Server responsed to DeviceDataRequest with Data(Controller : {response.Sensors.Count}, Sensor : {response.Controllers.Count}, Body : {response.Cameras.Count}).");

                    _deviceProvider.Clear();

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
                    

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(DeviceDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ControllerDataRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ControllerDataResponse(IControllerDataResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var list = _deviceProvider.Where(t => t.DeviceType == EnumDeviceType.Controller).ToList();

                    //foreach (var item in list)
                    //{
                    //    await _deviceProvider.DeletedItem(item);
                    //}

                    List<ControllerDeviceModel> controllers = response.Body;

                    foreach (var item in controllers)
                    {
                        _deviceProvider.Add(item);
                    }

                    //bool finishedResult = await _deviceProvider.Finished();
                    //if (finishedResult)
                    //{
                    //    await _deviceDbService.SaveControllers();
                    //}
                    

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ControllerDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SensorDataRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> SensorDataResponse(ISensorDataResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {

                    if (!response.Success)
                        return false;

                    var list = _deviceProvider.OfType<ISensorDeviceModel>().ToList();

                    //foreach (var item in list)
                    //{
                    //    await _deviceProvider.DeletedItem(item);
                    //}

                    //Body<SensorDeviceModel> sensors = response.Body;

                    //foreach (var item in sensors)
                    //{
                    //    _deviceProvider.Add(item);
                    //}

                    //bool finishedResult = await _deviceProvider.Finished();

                    //if (finishedResult)
                    //{
                    //    await _deviceDbService.SaveSensors();
                    //}

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SensorDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataResponse(ICameraDataResponseModel response)
        {
            return Task.Run(() =>
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

                    List<CameraDeviceModel> cameras = response.Body;

                    foreach (var item in cameras)
                    {
                        _deviceProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataSaveRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraDataSaveResponse(ICameraDataSaveResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success) return false;

                    foreach (var item in _deviceProvider.OfType<CameraDeviceModel>().ToList())
                    {
                        _deviceProvider.Remove(item);
                    }

                    foreach (var item in response.Body)
                    {
                        _deviceProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataSaveResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraMappingResponse(ICameraMappingResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success) return false;
                    //var mappings = new Body<CameraMappingModel>(response.Body);
                    _cameraMappingProvider.Clear();
                    foreach (var item in response.Body)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingSaveRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }
        
        

        public Task<bool> CameraMappingSaveResponse(ICameraMappingSaveResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _cameraMappingProvider.Clear();
                    foreach (var item in response.Body)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataResponse)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraOptionRequest(ICameraOptionRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraOptionRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraOptionResponse(ICameraOptionResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    _cameraOptionProvider.Clear();
                    foreach (CameraPresetModel item in response?.Presets)
                    {
                        _cameraOptionProvider.Add(item);
                    }

                    foreach (CameraProfileModel item in response?.Profiles)
                    {
                        _cameraOptionProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetResponse(ICameraPresetResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    //_cameraPresetProvider.Clear();
                    //foreach (var item in response.Body)
                    //{
                    //    _cameraPresetProvider.Add(item);
                    //}

                    foreach (var item in _cameraOptionProvider.OfType<CameraPresetModel>().ToList())
                    {
                        _cameraOptionProvider.Remove(item);
                    }

                    foreach (var item in response.Body)
                    {
                        _cameraOptionProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingSaveRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> CameraPresetSaveResponse(ICameraPresetSaveResponseModel response)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success) return false;

                    foreach (var item in _cameraOptionProvider.OfType<CameraPresetModel>().ToList())
                    {
                        _cameraOptionProvider.Remove(item);
                    }

                    foreach (var item in response.Body)
                    {
                        _cameraOptionProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SymbolInfoRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(SymbolInfoResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SymbolDataLoadRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(SymbolDataLoadResponse)} of {nameof(ClientService)} : {ex.Message}");
                   
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SymbolDataSaveRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(SymbolDataSaveResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(MapFileLoadRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(MapFileLoadResponse)} of {nameof(ClientService)} : {ex.Message}");
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
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(MapFileSaveRequest)} of {nameof(ClientService)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(MapFileSaveResponse)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ApiEventListRequest(IVmsApiGetEventListRequestModel request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(request, _settings);
                    await SendRequest(json);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ApiEventListRequest)} of {nameof(ClientService)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> ApiEventListResponse(IVmsApiGetEventListResponseModel response, IPEndPoint endPoint = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var list = response.Body;

                    _vmsEventProvider.Clear();
                    foreach (var item in _vmsEventProvider)
                    {
                        _vmsEventProvider.Add(item);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ApiEventListResponse)} of {nameof(ClientService)} : {ex.Message}");
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
        #endregion
        #region - Attributes -
        private DetectionEventProvider _detectionEventProvider;
        private ActionEventProvider _actionEventProvider;
        private MalfunctionEventProvider _malfunctionEventProvider;
        private EventProvider _eventProvider;
        private PreEventProvider _preEventProvider;
        private DeviceProvider _deviceProvider;
        private ControllerDeviceProvider _controllerDeviceProvider;
        private SensorDeviceProvider _sensorDeviceProvider;
        private CameraDeviceProvider _cameraDeivceProvider;
        private CameraOptionProvider _cameraOptionProvider;
        private CameraProfileProvider _cameraProfileProvider;
        private CameraPresetProvider _cameraPresetProvider;
        private CameraMappingProvider _cameraMappingProvider;
        private MapDbService _mapDbService;
        private MapProvider _mapProvider;
        private SymbolProvider _symbolProvider;
        private PointProvider _pointProvider;
        private SymbolInfoModel _symbolInfoModel;
        private VmsApiProvider _vmsApiProvider;
        private VmsEventProvider _vmsEventProvider;
        private VmsMappingProvider _vmsMappingProvider;
        private VmsSensorProvider _vmsSensorProvider;
        private LogProvider _logProvider;

        #endregion
    }
}
