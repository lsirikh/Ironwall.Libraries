using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Common.Proivders;
using Ironwall.Libraries.Tcp.Server.Services;
using Ironwall.Framework.DataProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.ObjectModel;
using Ironwall.Libraries.Utils.Exceptions;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Events.Services;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications;
using System.Threading;
using Ironwall.Framework.Models.Communications.Settings;
using Ironwall.Libraries.Account.Common.Providers;
using Ironwall.Libraries.Account.Common.Models;
using Ironwall.Libraries.Account.Common.Services;
using Ironwall.Libraries.Account.Server.Services;
using Ironwall.Libraries.Tcp.Common.Proivders.Models;
using System.Reflection;
using Ironwall.Libraries.Common.Providers;
using Ironwall.Libraries.Tcp.Server.Models;
using Ironwall.Framework.Models.Communications.Symbols;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Ironwall.Libraries.Map.Common.Services;
using System.Windows;
using Ironwall.Libraries.Map.Common.Helpers;
using System.Windows.Media.Media3D;
using System.IO;
using Ironwall.Libraries.Devices.Helpers;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Server.Services
{
    public class ServerService
        : AccountServerService, IServerService
    {
        #region - Ctors -
        public ServerService(
            AccountSetupModel accountSetupModel
            , TcpSetupModel tcpSetupModel
            , TcpServerSetupModel tcpServerSetupModel
            , SessionProvider sessionProvider
            , TcpClientProvider tcpClientProvider
            , LoginProvider loginProvider
            , UserProvider userProvider
            , LoginUserProvider loginUserProvider
            , AccountDbService accountDbService
            , EventDbService eventDbService

            , EventProvider eventProvider
            , DetectionEventProvider detectionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider

            , ActionEventProvider actionEventProvider
            
            , DeviceProvider deviceProvider
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraMappingProvider cameraMappingProvider
            , DeviceDbService deviceDbService

            , SymbolProvider symbolProvider
            , MapProvider mapProvider
            , MapDbService mapDbService
            , PointProvider pointProvider

            , TcpUserProvider tcpUserProvider
            , LogProvider logProvdier

            , DeviceInfoModel deviceInfoModel
            , SymbolInfoModel symbolInfoModel
            , MappingInfoModel mappingInfoModel
            )
            : base
            (accountSetupModel
            , tcpSetupModel
            , tcpServerSetupModel
            , sessionProvider
            , tcpClientProvider
            , loginProvider
            , userProvider
            , loginUserProvider
            , accountDbService
            , tcpUserProvider
            , logProvdier)
        {

            EventDbService = eventDbService;
            EventProvider = eventProvider;
            DetectionEventProvider = detectionEventProvider;
            MalfunctionEventProvider = malfunctionEventProvider;
            ActionEventProvider = actionEventProvider;

            _deviceProvider = deviceProvider;
            _controllerDeviceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeviceProvider = cameraDeviceProvider;
            _cameraMappingProvider = cameraMappingProvider;
            _cameraPresetProvider = cameraPresetProvider;

            _deviceDbService = deviceDbService;



            _mapProvider = mapProvider;
            _symbolProvider = symbolProvider;
            _mapDbService = mapDbService;
            _pointProvider = pointProvider;

            _deviceInfoModel = deviceInfoModel;
            _symbolInfoModel = symbolInfoModel;
            _mappingInfoModel = mappingInfoModel;
        }


        #endregion
        #region - Implementation of Interface -
        public Task UpdateHeartBeat(IHeartBeatRequestModel model, IPEndPoint endPoint)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //(IPEndPoint)removeCli?.Socket?.RemoteEndPoint
                    var searchedCli = ClientList.Where(t => (t.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() == model.IpAddress
                    && (t.Socket.RemoteEndPoint as IPEndPoint).Port == model.Port).FirstOrDefault();
                    if (searchedCli == null)
                    {
                        throw new NullReferenceException($"Server does not have the requested EndPoint Information in the ClientList");
                    }
                    var now = DateTimeHelper.GetCurrentTimeWithoutMS();
                    searchedCli.UpdateHeartBeat(now + TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat));

                    //Send Heartbeat Update Response!!!!!!!!!
                    var msg = $"HeartBeat Time was successfully extended";
                    var responseModel = ResponseFactory.Build<HeartBeatResponseModel>(true, msg, now.ToString("yyyy-MM-dd HH:mm:ss.ff"), searchedCli.HeartBeatExpireTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));

                    AcceptedClient_Event(msg, endPoint);
                    
                    await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in {nameof(UpdateHeartBeat)}  : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(UpdateHeartBeat)}  : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    var message = $"Raised NullReferenceException in {nameof(UpdateHeartBeat)}  : {ex.Message}";
                    Debug.WriteLine(message);
                    //Response message to the Client
                    //await KeepAliveResponse(false, "token was not matched!", null, endPoint);
                    AcceptedClient_Event(message, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}");
                    //Response message to the Client
                    //await KeepAliveResponse(false, $"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}", null, endPoint);
                    AcceptedClient_Event("token was not matched!", endPoint);
                    return false;
                }
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public Task ConnectionEventRequest(IConnectionRequestModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Connection Event with Updating Connection Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: "Connection data is empty.");

                    //02. Save Connection Event with Updating Connection Event Provider


                    //03. Bypass Event Message to All Clients
                    await SendRequest(JsonConvert.SerializeObject(model));

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"[{model.Id}]Controller({model.Controller}), Sensor({model.Sensor}) was connected");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ConnectionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(ConnectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task DetectionEventRequest(IDetectionRequestModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Detection Event with Updating Detection Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: "Detection data is empty.");

                    //02. Save Detection Event with Updating Detection Event Provider
                    var sensor = _sensorDeviceProvider
                    .Where((t) => (t as ISensorDeviceModel).DeviceNumber == model.Sensor
                    && (t as ISensorDeviceModel).Controller.DeviceNumber == model.Controller)
                    .FirstOrDefault() as SensorDeviceModel;
                    var detectionModel = ModelFactory.Build<DetectionEventModel>(model, sensor);

                    await EventDbService.SaveDetectionEvent(detectionModel);

                    //03. Bypass Event Message to All Clients
                    await SendRequest(JsonConvert.SerializeObject(model));

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"[{model.Id}]Controller({model.Controller}), Sensor({model.Sensor}) was detected");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DetectionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(DetectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task MalfunctionEventRequest(IMalfunctionRequestModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Malfunction Event with Updating Malfunction Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: "Detection data is empty.");

                    //02. Save Detection Event with Updating Detection Event Provider
                    MalfunctionEventModel eventModel = null;
                    switch (model.UnitType)
                    {
                        case EnumDeviceType.Controller:
                            {
                                var controller = _controllerDeviceProvider
                                .Where(t => t.DeviceNumber == model.Controller)
                                .FirstOrDefault() as ControllerDeviceModel;

                                eventModel = ModelFactory
                                .Build<MalfunctionEventModel>(model, controller);
                            }
                            break;
                        case EnumDeviceType.Multi:
                        case EnumDeviceType.Fence:
                        case EnumDeviceType.Contact:
                        case EnumDeviceType.PIR:
                        case EnumDeviceType.Underground:
                        case EnumDeviceType.Laser:
                            {
                                var sensor = _sensorDeviceProvider
                                .Where(s => s.DeviceNumber == model.Sensor)
                                .FirstOrDefault() as SensorDeviceModel;

                                eventModel = ModelFactory
                                .Build<MalfunctionEventModel>(model, sensor);
                            }
                            break;

                        default:
                            break;
                    }

                    await EventDbService.SaveMalfunctionEvent(eventModel);

                    //03. Bypass Event Message to All Clients
                    await SendRequest(JsonConvert.SerializeObject(model));

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"[{model.Id}]Controller({model.Controller}), Sensor({model.Sensor}) has an error");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(MalfunctionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(MalfunctionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task<bool> ActionEventRequest(IActionRequestModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Detection Event with Updating Detection Event Provider
            ///03. Send ActionResponse 
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: $"{nameof(ActionEventRequest)} data is empty.");

                    //02. Save Detection Event with Updating Detection Event Provider
                    var eventModel = EventProvider.Where(item => item.Id == model.EventId).FirstOrDefault();

                    if (eventModel.Status == 1)
                        throw new Exception(message: $"{model.EventId} has already been processed.");

                    eventModel.Status = 1;
                    ActionEventModel actionModel = null;
                    switch ((EnumEventType)eventModel.MessageType)
                    {
                        case EnumEventType.Intrusion:
                            {
                                var eModel = eventModel as DetectionEventModel;
                                actionModel = ModelFactory.Build<ActionEventModel>(model, eModel);
                                await EventDbService.UpdateDetectionEvent(eModel);
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
                                var eModel = eventModel as MalfunctionEventModel;
                                actionModel = ModelFactory.Build<ActionEventModel>(model, eModel);
                                await EventDbService.UpdateMalfunctionEvent(eModel);
                            }
                            break;
                        case EnumEventType.WindyMode:
                            break;
                        default:
                            break;
                    }


                    await EventDbService.SaveActionEvent(actionModel);


                    //03. Send ActionResponse 
                    //await ActionEventResponse(true, "Action Request was successfully applied!", actionModel);
                    await ActionEventResponse(true, "Action Request was successfully applied!", model);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"[{model.Id}]Action Report was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}");
                    await ActionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
            });
        }

        public Task<bool> SearchDetectionEventRequest(ISearchDetectionRequestModel model, IPEndPoint endPoint)
        {
            ///01. Check model
            ///02. Find events from DB
            ///03. Send SearchDetectionEventResponse 
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: $"{nameof(SearchDetectionEventRequest)} data is empty.");

                    if (!await IsLoggedInUser(model))
                        throw new Exception(message: "User token is not appropriate!");

                    //02. Find events from DB
                    await EventDbService.FetchDetectionEvent(model.StartDateTime, model.EndDateTime);

                    List<DetectionRequestModel> events = new List<DetectionRequestModel>();
                    foreach (DetectionEventModel item in EventProvider.OfType<IDetectionEventModel>().ToList())
                    {
                        var requestModel = RequestFactory.Build<DetectionRequestModel>(item);
                        events.Add(requestModel);
                    }

                    //03. Send SearchDetectionEventResponse 
                    await SearchEventResponse(true, "Search Detection Event Request was successfully applied!", events);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Detection Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in {nameof(SearchDetectionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchDetectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchDetectionEventRequest)} : {ex.Message}");
                    await ActionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SearchDetectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
            });
        }

        public Task SearchMalfunctionEventRequest(ISearchMalfunctionRequestModel model, IPEndPoint endPoint)
        {
            ///01. Check model
            ///02. Find events from DB
            ///03. Send SearchMalfunctionEventRequest 
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: $"{nameof(SearchMalfunctionEventRequest)} data is empty.");

                    if (!await IsLoggedInUser(model))
                        throw new Exception(message: "User token is not appropriate!");

                    //02. Find events from DB
                    await EventDbService.FetchMalfunctionEvent(model.StartDateTime, model.EndDateTime);

                    List<MalfunctionRequestModel> events = new List<MalfunctionRequestModel>();
                    foreach (MalfunctionEventModel item in EventProvider.OfType<IMalfunctionEventModel>().ToList())
                    {
                        var requestModel = RequestFactory.Build<MalfunctionRequestModel>(item);
                        events.Add(requestModel);
                    }

                    //03. Send SearchDetectionEventResponse 
                    await SearchEventResponse(true, "Search Malfunction Event Request was successfully applied!", events);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Malfunction Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}");
                    await ActionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
            });
        }

        public Task SearchActionEventRequest(ISearchActionRequestModel model, IPEndPoint endPoint)
        {
            ///01. Check model
            ///02. Find events from DB
            ///03. Send SearchActionEventRequest 
            ///04. Send Message to UI Log

            return Task.Run(async () =>
            {
                try
                {
                    //01. Check model
                    if (model == null)
                        throw new Exception(message: $"{nameof(SearchActionEventRequest)} data is empty.");

                    if (!await IsLoggedInUser(model))
                        throw new Exception(message: "User token is not appropriate!");

                    //02. Find events from DB
                    await EventDbService.FetchEvent(model.StartDateTime, model.EndDateTime);

                    List<DetectionRequestModel> detectionEvents = new List<DetectionRequestModel>();
                    List<MalfunctionRequestModel> malfunctionEvents = new List<MalfunctionRequestModel>();
                    List<ActionRequestModel> actionEvents = new List<ActionRequestModel>();
                    foreach (var item in EventProvider.ToList())
                    {
                        if(item is IDetectionEventModel detectionModel)
                        {
                            var requestModel = RequestFactory.Build<DetectionRequestModel>(detectionModel);
                            detectionEvents.Add(requestModel);
                        }else if(item is IMalfunctionEventModel malfunctionModel)
                        {
                            var requestModel = RequestFactory.Build<MalfunctionRequestModel>(malfunctionModel);
                            malfunctionEvents.Add(requestModel);
                        }
                        else
                        {
                        }
                    }

                    foreach (IActionEventModel item in ActionEventProvider.ToList())
                    {
                        var requestModel = RequestFactory.Build<ActionRequestModel>(item);
                        actionEvents.Add(requestModel);
                    }

                    //03. Send SearchActionEventResponse 
                    await SearchEventResponse(true, "Search Action Event Request was successfully applied!", detectionEvents, malfunctionEvents, actionEvents);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Action Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in {nameof(SearchActionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SearchActionEventRequest)} : {ex.Message}");
                    await ActionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SearchActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
            });
        }

       

        public Task DeviceInfoRequest(IDeviceInfoRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Device Info to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    if (!await IsLoggedInUser(model))
                        throw new Exception(message: "User token is not appropriate!");

                    //Debug.WriteLine($"_deviceInfoModel : {_deviceInfoModel.GetHashCode()}");

                    //01. Create Model with Parameters
                    var detail = ResponseFactory.Build<DeviceDetailModel>(_deviceInfoModel.Controller, _deviceInfoModel.Sensor, _deviceInfoModel.Camera, _deviceInfoModel.UpdateTime);

                    //02. Send Message to the designated Clients
                    await DeviceInfoResponse(true, aim, detail, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(DeviceInfoRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(DeviceInfoRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task DeviceDataRequest(IDeviceDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Device Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var contrllerList = new List<ControllerDeviceModel>();
                    var sensorList = new List<SensorDeviceModel>();
                    var cameraList = new List<CameraDeviceModel>();

                    foreach (var item in _deviceProvider)
                    {
                        if (!DeviceHelper.IsControllerCategory(item.DeviceType)) continue;

                        contrllerList.Add(item as ControllerDeviceModel);
                    }

                    foreach (var item in _deviceProvider)
                    {
                        if (!DeviceHelper.IsSensorCategory(item.DeviceType)) continue;

                        sensorList.Add(item as SensorDeviceModel);
                    }

                    foreach (var item in _deviceProvider)
                    {
                        if (!DeviceHelper.IsCameraCategory(item.DeviceType)) continue;

                        cameraList.Add(item as CameraDeviceModel);
                    }


                    //02. Send Message to the designated Clients
                    await DeviceDataResponse(true, aim, contrllerList, sensorList, cameraList, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task ControllerDataRequest(IControllerDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Controller Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var contrllerList = new List<ControllerDeviceModel>();
                    foreach (var item in _controllerDeviceProvider)
                    {
                        contrllerList.Add(item as ControllerDeviceModel);
                    }

                    //02. Send Message to the designated Clients
                    await ControllerDataResponse(true, aim, contrllerList, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task SensorDataRequest(ISensorDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Sensor Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var sensorList = new List<SensorDeviceModel>();

                    //For Data size, needs to send the partially grouped data
                    foreach (SensorDeviceModel sensor in _sensorDeviceProvider)
                    {
                        sensorList.Add(sensor);
                    }

                    //02. Send Message to the designated Clients
                    await SensorDataResponse(true, aim, sensorList, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SensorDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(SensorDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task CameraDataRequest(ICameraDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    List<CameraDeviceModel> camreaList = _deviceProvider.OfType<CameraDeviceModel>().ToList();

                    //02. Send Message to the designated Clients
                    await CameraDataResponse(true, aim, camreaList, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraDataSaveRequest(ICameraDataSaveRequestModel model, IPEndPoint endPoint)
        {
            ///01. Remove All Data from Provider
            ///02. Insert All Received Data from RequestModel
            ///03. Send Message to UI Log
            var aim = $"Send Camera Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    ///01. Remove All Data from Provider
                    foreach (var item in _deviceProvider.OfType<ICameraDeviceModel>().ToList())
                    {
                        _deviceProvider.Remove(item);
                    }

                    //For Data size, needs to send the partially grouped data
                    foreach (var item in model.Cameras)
                    {
                        _deviceProvider.Add(item);
                    }

                    await _deviceDbService.SaveCameras();

                    var deviceDetail = DeviceInfoUpdate();
                    await _deviceDbService.UpdateDeviceInfo(deviceDetail);

                    //02. Send Message to the designated Clients
                    await CameraDataSaveResponse(true, aim, deviceDetail, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraPresetRequest(ICameraPresetRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Preset Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    //For Data size, needs to send the partially grouped data
                    var list = _cameraPresetProvider.OfType<CameraPresetModel>().ToList();

                    //02. Send Message to the designated Clients
                    await CameraPresetResponse(true, aim, list, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraPresetSaveRequest(ICameraPresetSaveRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Preset Save Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    //For Data size, needs to send the partially grouped data
                    
                    _cameraPresetProvider.Clear();
                    foreach (var preset in model.Presets) 
                    {
                        _cameraPresetProvider.Add(preset);
                    }

                    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                    await _deviceDbService.SaveCameraPresets(tcs:tcs);

                    //02. Send Message to the designated Clients
                    await CameraPresetSaveResponse(tcs.Task.Result, aim, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraMappingRequest(ICameraMappingRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Mapping Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var list = new List<CameraMappingModel>();

                    //For Data size, needs to send the partially grouped data
                    foreach (CameraMappingModel item in _cameraMappingProvider)
                    {
                        list.Add(item);
                    }

                    //02. Send Message to the designated Clients
                    await CameraMappingResponse(true, aim, list, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraMappingSaveRequest(ICameraMappingSaveRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Mapping Save Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    _cameraMappingProvider.Clear();
                    foreach (var item in model.Mappings)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    //MetaData UPDATE ===> MappingInfoModel Update!!
                    var mappingInfo = ModelFactory.Build<MappingInfoModel>(_cameraMappingProvider.Count, DateTimeHelper.GetCurrentTimeWithoutMS());
                    //_mappingInfoModel.Mapping =_cameraMappingProvider.Count;
                    //_mappingInfoModel.UpdateTime = DateTimeHelper.GetCurrentTimeWithoutMS();

                    await _deviceDbService.SaveCameraMappings(isFinished:true);
                    await _deviceDbService.UpdateMappingInfo(mappingInfo);

                    //02. Send Message to the designated Clients
                    await CameraMappingSaveResponse(true, aim, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraMappingInfoRequest(ICameraMappingInfoRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Mapping Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters

                    //02. Send Message to the designated Clients
                    await CameraMappingInfoResponse(true, aim, _mappingInfoModel, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraMappingInfoRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraMappingInfoRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        

        public Task SymbolInfoDataRequest(ISymbolInfoRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send SymbolInfo Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var isTokenTrue = SessionProvider.Where(t => (t as ILoginSessionModel).Token == model.Token).Count();
                    if (!(isTokenTrue > 0)) return;

                    //_symbolInfoModel.
                    var detail = ResponseFactory.Build<SymbolDetailModel>(_symbolInfoModel.Map, _symbolInfoModel.Symbol
                        , _symbolInfoModel.ShapeSymbol, _symbolInfoModel.ObjectShape, _symbolInfoModel.UpdateTime);
                    //02. Send Message to the designated Clients
                    await SymbolInfoResponse(true, aim, detail, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }


        public Task SymbolDataLoadRequest(ISymbolDataLoadRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Symbol Data Load Response to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    var maps = new List<MapModel>();
                    foreach (var item in _mapProvider.ToList())
                    {
                        maps.Add(item as MapModel);
                    }

                    var points = new List<PointClass>();
                    foreach (var item in _pointProvider.ToList())
                    {
                        points.Add(item as PointClass);
                    }

                    //01. Create Model with Parameters
                    var symbols = new List<SymbolModel>();
                    foreach (var item in _symbolProvider)
                    {
                        if (SymbolHelper.IsSymbolCategory(item.TypeShape))
                            symbols.Add(item as SymbolModel);
                    }

                    var shapes = new List<ShapeSymbolModel>();
                    foreach (var item in _symbolProvider)
                    {
                        if(SymbolHelper.IsShapeCategory(item.TypeShape))
                            shapes.Add(item as ShapeSymbolModel);
                    }

                    var objects = new List<ObjectShapeModel>();
                    foreach (var item in _symbolProvider)
                    {
                        if (SymbolHelper.IsObjectCategory(item.TypeShape))
                            objects.Add(item as ObjectShapeModel);
                    }
                    

                    //02. Send Message to the designated Clients
                    await SymbolDataLoadResponse(true, aim, maps, points, symbols, shapes, objects, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolDataLoadRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolDataLoadRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }


        public Task SymbolDataSaveRequest(ISymbolDataSaveRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Symbol Data Save Response to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //List<MapModel> maps = model.Maps;
                    List<PointClass> points = model.Points;
                    List<SymbolModel> symbols = model.Symbols;
                    List<ShapeSymbolModel> shapes = model.Shapes;
                    List<ObjectShapeModel> objects = model.Objects;

                    //await _mapProvider.ClearData();

                    //foreach (var item in maps)
                    //{
                    //    _mapProvider.Add(item, 0);
                    //}
                    
                    _pointProvider.Clear();
                    foreach (var item in points)
                    {
                        _pointProvider.Add(item);
                    }

                    bool mapFinishedResult = await _mapProvider.Finished();
                    if (mapFinishedResult)
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
                        if (!SymbolHelper.IsShapeCategory(item.TypeShape)) continue;

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
                    
                    await _mapDbService.SaveAllSymbols();

                    var detail = ResponseFactory.Build<SymbolDetailModel>(_mapProvider.Count(), symbols.Count(), shapes.Count(), objects.Count(), DateTimeHelper.GetCurrentTimeWithoutMS());
                    await _mapDbService.UpdateSymbolInfo(detail);

                    var moreDetail = ResponseFactory.Build<SymbolMoreDetailModel>(_mapProvider.Count(), symbols.Count(), _pointProvider.Count(), shapes.Count(), objects.Count(), _symbolInfoModel.UpdateTime);
                    await SymbolDataSaveResponse(true, aim, moreDetail, endPoint);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolDataSaveRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolDataSaveRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task MapFileLoadRequest(IMapFileLoadRequestModel model, IPEndPoint endPoint)
        {
            var aim = $"Send Map File Process to {model.UserId} Successfully excuted!";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var isTokenTrue = SessionProvider.Where(t => (t as ILoginSessionModel).Token == model.Token).Count();
                    if (!(isTokenTrue > 0)) return;
                    
                    var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    List<MapModel> maps = new List<MapModel>(); 
                    foreach (var map in _mapProvider)
                    {
                        if (MapHelper.IsMapFileExist(map.Url))
                        {
                            //await SendMapDataRequest(MapHelper.CreateFullUrl(model.MapModel.Url));
                            maps.Add(map as MapModel);
                        }
                    }
                    await MapFileLoadResonse(true, aim, maps, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task MapFileSaveRequest(IMapFileSaveRequestModel model, IPEndPoint endPoint)
        {
            var aim = $"Send Map File Process to {model.UserId} Successfully excuted!";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var isTokenTrue = SessionProvider.Where(t => (t as ILoginSessionModel).Token == model.Token).Count();
                    if (!(isTokenTrue > 0)) return;

                    await _mapProvider.ClearData();

                    foreach (var item in model.Maps)
                    {
                        if (MapHelper.IsMapFileExist(item.Url))
                        {
                            _mapProvider.Add(item);
                        }
                    }

                    await _mapDbService.SaveMaps();

                    var detail = new MapDetailModel(model.Maps, DateTimeHelper.GetCurrentTimeWithoutMS());

                    var symbolDetailModel = ResponseFactory.Build<SymbolDetailModel>(detail.Maps.Count(), _symbolInfoModel.Symbol, _symbolInfoModel.ShapeSymbol, _symbolInfoModel.ObjectShape, detail.UpdateTime);
                    await _mapDbService.UpdateSymbolInfo(symbolDetailModel);

                    await MapFileSaveResonse(true, aim, detail, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region TaskResponse Process
        public async Task ActionEventResponse(bool success, string msg, IActionRequestModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<ActionResponseModel>(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }


        private async Task SearchEventResponse(bool success, string msg, List<DetectionRequestModel> events, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SearchDetectionResponseModel>(success, msg, events);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SearchEventResponse(bool success, string msg, List<MalfunctionRequestModel> events, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SearchMalfunctionResponseModel>(success, msg, events);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SearchEventResponse(bool success, string msg
            , List<DetectionRequestModel> detectionEvents
            , List<MalfunctionRequestModel> malfunctionEvents
            , List<ActionRequestModel> actionEvents, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SearchActionResponseModel>(success, msg, detectionEvents, malfunctionEvents, actionEvents);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task DeviceInfoResponse(bool success, string msg, IDeviceDetailModel model, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<DeviceInfoResponseModel>(success, msg, model);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(DeviceInfoResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task DeviceDataResponse(bool success, string msg,
            List<ControllerDeviceModel> contrllerList,
            List<SensorDeviceModel> sensorList,
            List<CameraDeviceModel> cameraList,
            IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<DeviceDataResponseModel>(success, msg, contrllerList, sensorList, cameraList);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(DeviceDataResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task ControllerDataResponse(bool success, string msg, List<ControllerDeviceModel> list, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<ControllerDataResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ControllerDataResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        public async Task SensorDataResponse(bool success, string msg, List<SensorDeviceModel> list, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SensorDataResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SensorDataResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        public async Task CameraDataResponse(bool success, string msg, List<CameraDeviceModel> list, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraDataResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraDataResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        
        public async Task CameraDataSaveResponse(bool success, string msg, IDeviceDetailModel deviceDetail, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraDataSaveResponseModel>(success, msg, deviceDetail);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraDataSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraPresetResponse(bool success, string msg, List<CameraPresetModel> list, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraPresetResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraPresetResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        
        private async Task CameraPresetSaveResponse(bool success, string msg, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraPresetSaveResponseModel>(success, msg);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraPresetSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraMappingResponse(bool success, string msg, List<CameraMappingModel> list, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraMappingResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraMappingResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        
        

        private async Task CameraMappingSaveResponse(bool success, string msg, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraMappingSaveResponseModel>(success, msg, _mappingInfoModel);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraMappingSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraMappingInfoResponse(bool success, string msg, MappingInfoModel mappingInfoModel, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraMappingInfoResponseModel>(success, msg, mappingInfoModel);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraMappingResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task SymbolInfoResponse(bool success, string msg, SymbolDetailModel model, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SymbolInfoResponseModel>(success, msg, model);
                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SymbolInfoResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SymbolDataLoadResponse(bool success, string msg, List<MapModel> maps, List<PointClass> points, List<SymbolModel> symbols, List<ShapeSymbolModel> shapes, List<ObjectShapeModel> objects, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SymbolDataLoadResponseModel>(success, msg, maps, points, symbols, shapes, objects);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SymbolDataSaveResponse(bool success, string msg, SymbolMoreDetailModel detail, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SymbolDataSaveResponseModel>(success, msg, detail);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task MapFileLoadResonse(bool success, string msg, List<MapModel> model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<MapFileLoadResponseModel>(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        
        public async Task MapFileSaveResonse(bool success, string msg, MapDetailModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = ResponseFactory.Build<MapFileSaveResponseModel>(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private IDeviceDetailModel DeviceInfoUpdate()
        {
            var controller = _deviceProvider.OfType<IControllerDeviceModel>().Count();
            var sensor = _deviceProvider.OfType<ISensorDeviceModel>().Count();
            var camera = _deviceProvider.OfType<ICameraDeviceModel>().Count();

            return ModelFactory.Build<DeviceDetailModel>(controller, sensor, camera, DateTimeHelper.GetCurrentTimeWithoutMS());
            
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public EventDbService EventDbService { get; }
        public EventProvider EventProvider { get; }
        public DetectionEventProvider DetectionEventProvider { get; }
        public ActionEventProvider ActionEventProvider { get; }
        public MalfunctionEventProvider MalfunctionEventProvider { get; }
        #endregion
        #region - Attributes -
        public DeviceInfoModel _deviceInfoModel;
        private SymbolInfoModel _symbolInfoModel;
        private MappingInfoModel _mappingInfoModel;
        private DeviceProvider _deviceProvider;
        public ControllerDeviceProvider _controllerDeviceProvider;
        public SensorDeviceProvider _sensorDeviceProvider;
        public CameraDeviceProvider _cameraDeviceProvider;
        private CameraMappingProvider _cameraMappingProvider;
        private CameraPresetProvider _cameraPresetProvider;
        private DeviceDbService _deviceDbService;
        private MapDbService _mapDbService;

        private MapProvider _mapProvider;
        private SymbolProvider _symbolProvider;
        private PointProvider _pointProvider;
        #endregion
    }
}
