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
using Ironwall.Libraries.Base.Services;
using System.Diagnostics.Eventing;
using Newtonsoft.Json.Converters;
using Ironwall.Libraries.VMS.Common.Services;
using Ironwall.Framework.Models.Communications.VmsApis;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Sensorway.Events.Base.Models;
using IActionEventModel = Ironwall.Framework.Models.Events.IActionEventModel;

namespace Ironwall.Libraries.Server.Services
{
    public class ServerService : AccountServerService, IServerService
    {
        #region - Ctors -
        public ServerService(
            ILogService log
            , AccountSetupModel accountSetupModel
            , TcpSetupModel tcpSetupModel
            , TcpServerSetupModel tcpServerSetupModel
            , SessionProvider sessionProvider
            , TcpClientProvider tcpClientProvider
            , LoginProvider loginProvider
            , UserProvider userProvider
            , LoginUserProvider loginUserProvider
            , AccountDbService accountDbService
            , IEventDbService eventDbService

            , Events.Providers.EventProvider eventProvider
            , DetectionEventProvider detectionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider

            , ActionEventProvider actionEventProvider
            
            , DeviceProvider deviceProvider
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraProfileProvider cameraProfileProvider
            , CameraOptionProvider cameraOptionProvider
            , CameraMappingProvider cameraMappingProvider
            , IDeviceDbService deviceDbService

            , SymbolProvider symbolProvider
            , MapProvider mapProvider
            , MapDbService mapDbService
            , PointProvider pointProvider

            , TcpUserProvider tcpUserProvider
            , LogProvider logProvdier

            , IVmsApiService vmsApiService
            , IVmsDbService vmsDbService
            , VmsApiProvider vmsApiProvider
            , VmsEventProvider vmsEventProvider
            , VmsMappingProvider vmsMappingProvider
            , VmsSensorProvider vmsSensorProvider
            
            , SymbolInfoModel symbolInfoModel
            )
            : base
            ( log
            , accountSetupModel
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

            _eventDbService = eventDbService;
            _eventProvider = eventProvider;
            _detectionEventProvider = detectionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _actionEventProvider = actionEventProvider;

            _deviceDbService = deviceDbService;
            _deviceProvider = deviceProvider;
            _controllerDeviceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeviceProvider = cameraDeviceProvider;

            _cameraMappingProvider = cameraMappingProvider;

            _cameraOptionProvider = cameraOptionProvider;
            _cameraPresetProvider = cameraPresetProvider;
            _cameraProfileProvider = cameraProfileProvider;


            _mapProvider = mapProvider;
            _symbolProvider = symbolProvider;
            _mapDbService = mapDbService;
            _pointProvider = pointProvider;

            _symbolInfoModel = symbolInfoModel;

            _vmsApiService = vmsApiService;
            _vmsDbService = vmsDbService;
            _vmsApiProvider = vmsApiProvider;
            _vmsEventProvider = vmsEventProvider;
            _vmsMappingProvider = vmsMappingProvider;
            _vmsSensorProvider = vmsSensorProvider;

            settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.ff"
            };
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
                    
                    await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in {nameof(UpdateHeartBeat)}  : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(UpdateHeartBeat)}  : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    var message = $"Raised NullReferenceException in {nameof(UpdateHeartBeat)}  : {ex.Message}";
                    _log.Error(message);
                    //Response message to the Client
                    //await KeepAliveResponse(false, "token was not matched!", null, endPoint);
                    AcceptedClient_Event(message, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}");
                    //Response message to the Client
                    //await KeepAliveResponse(false, $"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}", null, endPoint);
                    AcceptedClient_Event("token was not matched!", endPoint);
                    return false;
                }
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public  Task ConnectionEventRequest(IConnectionEventModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Connection Event with Updating Connection Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log

            try
            {
                //01. Check model
                if (model == null)
                    throw new Exception(message: "Connection data is empty.");

                //02. Save Connection Event with Updating Connection Event Provider


                //03. Bypass Event Message to All Clients
                //await SendRequest(JsonConvert.SerializeObject(model));

                //04. Send Message to UI Log
                AcceptedClient_Event($"[{model.Id}]{model.Device.DeviceType} Device({model.Device.Id}) was connected!");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(ConnectionEventRequest)} : {ex.Message}");
                AcceptedClient_Event($"Raised Exception in {nameof(ConnectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
            }

            return Task.CompletedTask;
        }

        public async Task DetectionEventRequest(IDetectionEventModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Detection Event with Updating Detection Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log
            try
            {
                //01. Check model
                if (model == null)
                    throw new Exception(message: "Detection data is empty.");

                var tcs = new TaskCompletionSource<bool>();
                Debug.WriteLine($"model : {model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}");
                var fetchEvent = await _eventDbService.SaveDetectionEvent(model, tcs: tcs);
                var result = await tcs.Task;
                Debug.WriteLine($"fetchmodel : {fetchEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}");
                if (!result) throw new Exception(message: $"Fail to execute {nameof(DetectionEventRequest)} in {nameof(ServerService)}");

                try
                {
                    //Test를 위해서 Static 코드를 추가
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    var vmsSensor = _vmsSensorProvider.Where(entity => entity.Device.Id == fetchEvent.Device.Id).FirstOrDefault();
                    var vmsMapping = _vmsMappingProvider.Where(entity => entity.GroupNumber == vmsSensor?.GroupNumber).FirstOrDefault();
                    var eventModel = _vmsEventProvider.Where(entity => entity.Id == vmsMapping?.EventId).FirstOrDefault();

                    var device = fetchEvent.Device as ISensorDeviceModel;

                    var actionEvent = new Sensorway.Events.Base.Models.ActionEventModel(0, eventModel.Id, $"Controller : {device.Controller.DeviceNumber}, Sensor : {device.DeviceNumber} was detected", Sensorway.Events.Base.Enums.EnumEventStatus.ON, DateTime.Now);
                    await _vmsApiService.ApiSetActionEventProcess(actionEvent);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                catch 
                {
                }
                

                var request = new DetectionRequestModel(fetchEvent);

                //03. Bypass Event Message to All Clients
                await SendRequest(JsonConvert.SerializeObject(request, settings));

                //04. Send Message to UI Log
                AcceptedClient_Event($"[{model.Id}]{model.Device.DeviceType} Device({model.Device.Id}) was detected!");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(DetectionEventRequest)} : {ex.Message}");
                AcceptedClient_Event($"Raised Exception in {nameof(DetectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
            }

        }

        public async Task MalfunctionEventRequest(IMalfunctionEventModel model, IPEndPoint endPoint = null)
        {
            ///01. Check model
            ///02. Save Malfunction Event with Updating Malfunction Event Provider
            ///03. Bypass Event Message to All Clients
            ///04. Send Message to UI Log
            try
            {
                //01. Check model
                if (model == null)
                    throw new Exception(message: "Detection data is empty.");

                var tcs = new TaskCompletionSource<bool>();
                var fetchEvent = await _eventDbService.SaveMalfunctionEvent(model, tcs: tcs);
                var result = await tcs.Task;
                if (!result) throw new Exception(message: $"Fail to execute {nameof(MalfunctionEventRequest)} in {nameof(ServerService)}");

                var request = new MalfunctionRequestModel(fetchEvent);

                //03. Bypass Event Message to All Clients
                await SendRequest(JsonConvert.SerializeObject(request, settings));

                //04. Send Message to UI Log
                AcceptedClient_Event($"[{model.Id}]{model.Device.DeviceType} Device({model.Device.Id}) was broken!");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(MalfunctionEventRequest)} : {ex.Message}");
                AcceptedClient_Event($"Raised Exception in {nameof(MalfunctionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
            }
        }


        public async Task<bool> ActionEventRequest(IActionRequestModel model, IPEndPoint endPoint = null)
        {
            //01. Check model
            //02. Fetch original event from DB
            //03. Check original event was updated from EnumTrueFalse.False to EnumTrueFalse.True
            //04.Update original event from DB
            //05.Save new action event
            //06. Send ActionResponse 
            //07. Send Message to UI Log

            await _semaphore.WaitAsync(); // 시작 시 세마포어를 기다립니다.

            try
            {
                //01. Check model
                if (model == null) throw new Exception(message: $"{nameof(ActionEventRequest)} data is empty.");

                //02. Fetch original event from DB
                var tcs = new TaskCompletionSource<bool>();
                var eventModel = model.Body;
                bool result = false;
                IMetaEventModel originEvent = null;
                switch ((EnumEventType)eventModel.FromEvent.MessageType)
                {
                    case EnumEventType.Intrusion:
                        {
                            originEvent = await _eventDbService.FetchDetectionEvent(eventModel.FromEvent as IDetectionEventModel, tcs: tcs);
                            result = await tcs.Task;
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
                            originEvent = await _eventDbService.FetchMalfunctionEvent(eventModel.FromEvent as IMalfunctionEventModel, tcs: tcs);
                            result = await tcs.Task;
                        }
                        break;
                    case EnumEventType.WindyMode:
                        break;
                    default:
                        break;
                }

                if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");

                //03. Check original event was updated from EnumTrueFalse.False to EnumTrueFalse.True
                if (originEvent.Status == EnumTrueFalse.True)
                    throw new Exception(message: $"{originEvent.Id} has already been processed.");

                //04.Save new action event
                tcs = new TaskCompletionSource<bool>();
                var savedEvent = await _eventDbService.SaveActionEvent(eventModel, tcs: tcs);
                result = await tcs.Task;
                if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");

                //05.Update original event from DB
                tcs = new TaskCompletionSource<bool>();
                originEvent.Status = EnumTrueFalse.True;

                switch ((EnumEventType)originEvent.MessageType)
                {
                    case EnumEventType.Intrusion:
                        {
                            await _eventDbService.UpdateDetectionEvent(originEvent as IDetectionEventModel, tcs: tcs);
                            result = await tcs.Task;
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
                            await _eventDbService.UpdateMalfunctionEvent(originEvent as IMalfunctionEventModel, tcs: tcs);
                            result = await tcs.Task;
                        }
                        break;
                    case EnumEventType.WindyMode:
                        break;
                    default:
                        break;
                }
                if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");


                //Static Code should be removed!!!!!!!!!!!!!!
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                try
                {
                    var vmsSensor = _vmsSensorProvider.Where(entity => entity.Device.Id == originEvent.Device.Id).FirstOrDefault();
                    var vmsMapping = _vmsMappingProvider.Where(entity => entity.GroupNumber == vmsSensor.GroupNumber).FirstOrDefault();
                    var vmsEventModel = _vmsEventProvider.Where(entity => entity.Id == vmsMapping.EventId).FirstOrDefault();

                    var device = originEvent.Device as ISensorDeviceModel;
                    var actionEvent = new Sensorway.Events.Base.Models.ActionEventModel(0, vmsEventModel.Id, $"Detection Event(Controller : {device.Controller.DeviceNumber}, Sensor : {device.DeviceNumber}) was reported", Sensorway.Events.Base.Enums.EnumEventStatus.OFF, DateTime.Now);
                    await _vmsApiService.ApiSetActionEventProcess(actionEvent);
                }
                catch 
                {
                }      
                
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //06. Send ActionResponse 
                await ActionEventDetectionResponse(true, "Action Request was successfully applied!", savedEvent);

                //07. Send Message to UI Log
                AcceptedClient_Event($"[{model.Id}]Action Report was requested!", endPoint);

                return true;
            }
            catch (SocketSendException ex)
            {
                _log.Error($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}");
                AcceptedClient_Event($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                return false;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}");
                await ActionEventResponse(false, ex.Message, null as IActionEventModel, endPoint);
                AcceptedClient_Event($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                return false;
            }
            finally
            {
                _semaphore.Release(); // 작업이 완료되면 세마포어를 해제합니다.
            }
        }

        //public Task<bool> ActionEventRequest(IActionRequestMalfunctionModel model, IPEndPoint endPoint = null)
        //{
        //    ///01. Check model
        //    ///02. Save Detection Event with Updating Detection Event Provider
        //    ///03. Send ActionResponse 
        //    ///04. Send Message to UI Log

        //    return Task.Run(async () =>
        //    {
        //        try
        //        {
        //            //01. Check model
        //            if (model == null)
        //                throw new Exception(message: $"{nameof(ActionEventRequest)} data is empty.");


        //            //01. Fetch original event from DB
        //            var tcs = new TaskCompletionSource<bool>();
        //            var eventModel = new ActionEventModel(model);
        //            var originEvent = await _eventDbService.FetchMalfunctionEvent(eventModel.FromEvent.Id, tcs: tcs);
        //            var result = await tcs.Task;
        //            if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");

        //            //02. Check original event was updated from EnumTrueFalse.False to EnumTrueFalse.True
        //            if (originEvent.Status == EnumTrueFalse.True)
        //                throw new Exception(message: $"{originEvent.Id} has already been processed.");

        //            //03.Update original event from DB
        //            tcs = new TaskCompletionSource<bool>();
        //            originEvent.Status = EnumTrueFalse.True;
        //            await _eventDbService.UpdateMalfunctionEvent(originEvent, tcs: tcs);
        //            result = await tcs.Task;
        //            if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");

        //            //04.Save new action event
        //            tcs = new TaskCompletionSource<bool>();
        //            var savedEvent = await _eventDbService.SaveActionEvent(eventModel, tcs: tcs);
        //            result = await tcs.Task;
        //            if (!result) throw new Exception(message: $"Fail to execute {nameof(ActionEventRequest)} in {nameof(ServerService)}");

        //            //05. Send ActionResponse 
        //            await ActionEventMalfunctionResponse(true, "Action Request was successfully applied!", savedEvent);

        //            //06. Send Message to UI Log
        //            AcceptedClient_Event($"[{model.Id}]Action Report was requested!", endPoint);

        //            return true;
        //        }
        //        catch (SocketSendException ex)
        //        {
        //            _log.Error($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}");
        //            AcceptedClient_Event($"Raised SocketSendException in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

        //            return false;
        //        }
        //        catch (Exception ex)
        //        {
        //            _log.Error($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}");
        //            await ActionEventResponse(false, ex.Message, null as IMalfunctionEventModel, endPoint);
        //            AcceptedClient_Event($"Raised Exception in {nameof(ActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

        //            return false;
        //        }
        //    });
        //}

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
                    var list = await _eventDbService.FetchDetectionEvents(model.StartDateTime, model.EndDateTime);

                    //03. Send SearchDetectionEventResponse 
                    await SearchEventResponse(true, "Search Detection Event Request was successfully applied!", list);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Detection Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in {nameof(SearchDetectionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchDetectionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchDetectionEventRequest)} : {ex.Message}");
                    await SearchEventResponse(false, ex.Message, null as List<IDetectionEventModel>, endPoint);
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
                    var events = await _eventDbService.FetchMalfunctionEvents(model.StartDateTime, model.EndDateTime);

                    //03. Send SearchDetectionEventResponse 
                    await SearchEventResponse(true, "Search Malfunction Event Request was successfully applied!", events);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Malfunction Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchMalfunctionEventRequest)} : {ex.Message}");
                    await SearchEventResponse(false, ex.Message, null as List<IMalfunctionEventModel>, endPoint);
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
                    //var detectionList = await _eventDbService.FetchDetectionEvents(model.StartDateTime, model.EndDateTime);
                    //var malfunctionList = await _eventDbService.FetchMalfunctionEvents(model.StartDateTime, model.EndDateTime);
                    var events = await _eventDbService.FetchActionEvents(model.StartDateTime, model.EndDateTime);

                    //03. Send SearchActionEventResponse 
                    await SearchEventResponse(true, "Search Action Event Request was successfully applied!", events);

                    //04. Send Message to UI Log
                    AcceptedClient_Event($"Search Action Event Request was requested!", endPoint);

                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in {nameof(SearchActionEventRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in {nameof(SearchActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(SearchActionEventRequest)} : {ex.Message}");
                    await SearchEventResponse(false, ex.Message, null as List<IActionEventModel>, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(SearchActionEventRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);

                    return false;
                }
            });
        }

        public Task DeviceDataRequest(IDeviceDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Device Data Reponse to {endPoint.Address} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters
                    var contrllerList = new List<ControllerDeviceModel>();
                    var sensorList = new List<SensorDeviceModel>();
                    var cameraList = new List<CameraDeviceModel>();

                    foreach (var item in _deviceProvider.OfType<ControllerDeviceModel>().ToList())
                    {
                        //if (!DeviceHelper.IsControllerCategory(item.DeviceType)) continue;
                        contrllerList.Add(item as ControllerDeviceModel);
                    }

                    foreach (var item in _deviceProvider.OfType<SensorDeviceModel>().ToList())
                    {
                        //if (!DeviceHelper.IsSensorCategory(item.DeviceType)) continue;
                        sensorList.Add(item as SensorDeviceModel);
                    }

                    foreach (var item in _deviceProvider.OfType<CameraDeviceModel>().ToList())
                    {
                        //if (!DeviceHelper.IsCameraCategory(item.DeviceType)) continue;
                        cameraList.Add(item as CameraDeviceModel);
                    }


                    //02. Send Message to the designated Clients
                    await DeviceDataResponse(true, aim, contrllerList, sensorList, cameraList, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");

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
            var aim = $"Send Controller Data Reponse to {endPoint.Address} Successfully.";
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
                    _log.Error($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");

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
            var aim = $"Send Sensor Data Reponse to {endPoint.Address} Successfully.";
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
                    _log.Error($"Raised Exception in {nameof(SensorDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(SensorDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public Task CameraDataRequest(ICameraDataRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Data Reponse to {endPoint.Address} Successfully.";
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
                    _log.Error($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraDataSaveRequest(ICameraDataSaveRequestModel model, IPEndPoint endPoint)
        {
            ///01. Remove All Data from Provider
            ///02. Insert All Received Data from Body
            ///03. Send Message to UI Log
            var aim = $"Send Camera Data Reponse to {model.UserId} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    var tcs = new TaskCompletionSource<bool>();
                    var list = model.Body.OfType<ICameraDeviceModel>().ToList();
                    await _deviceDbService.SaveCameras(list, tcs: tcs);
                    var result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to execute {nameof(CameraDataSaveRequest)} in {nameof(ServerService)}");

                    tcs = new TaskCompletionSource<bool>();
                    var fetches = await _deviceDbService.FetchCameras(tcs: tcs);
                    result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to fetch {fetches.GetType()} in {nameof(CameraDataSaveRequest)} of {nameof(ServerService)}");

                    foreach (var item in _deviceProvider.OfType<CameraDeviceModel>().ToList())
                    {
                        _deviceProvider.Remove(item);
                    }

                    foreach (var item in fetches)
                    {
                        _deviceProvider.Add(item);
                    }

                    //02. Send Message to the designated Clients
                    await CameraDataSaveResponse(true, aim, fetches, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraOptionRequest(CameraOptionRequestModel json, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Option Data Reponse to {endPoint.Address} Successfully.";
            return Task.Run(async () =>
            {
                try
                {

                    //var response = new CameraOptionResponse(_cameraOptionProvider.OfType<BaseOptionModel>().ToList(), true, aim);
                    //02. Send Message to the designated Clients
                    await CameraOptionResponse(true, aim, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        

        public Task CameraPresetRequest(ICameraPresetRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Preset Data Reponse to {endPoint.Address} Successfully.";
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
                    _log.Error($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}");
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

                    var tcs = new TaskCompletionSource<bool>();
                    var list = model.Body.OfType<ICameraPresetModel>().ToList();
                    await _deviceDbService.SavePresets(list, tcs: tcs);
                    var result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to execute {nameof(CameraPresetSaveRequest)} in {nameof(ServerService)}");

                    tcs = new TaskCompletionSource<bool>();
                    var fetches = await _deviceDbService.FetchPresets(tcs: tcs);
                    result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to fetch {fetches.GetType()} in {nameof(CameraDataSaveRequest)} of {nameof(ServerService)}");


                    foreach (var item in _cameraOptionProvider.OfType<CameraPresetModel>().ToList())
                    {
                        _cameraOptionProvider.Remove(item);
                    }

                    foreach (var item in fetches) 
                    {
                        _cameraOptionProvider.Add(item);
                    }

                    //02. Send Message to the designated Clients
                    await CameraPresetSaveResponse(tcs.Task.Result, aim, fetches, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraPresetRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task CameraMappingRequest(ICameraMappingRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send Camera Mapping Data Reponse to {endPoint.Address} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    var tcs = new TaskCompletionSource<bool>();
                    var mappings = await _deviceDbService.FetchCameraMappings(tcs: tcs);
                   
                    var result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to fetch CameraMappingModels in {nameof(CameraMappingRequest)} of {nameof(ServerService)}");

                    _cameraMappingProvider.Clear();
                    foreach (var item in mappings)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    await CameraMappingResponse(true, aim, _cameraMappingProvider.ToList(), endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
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
                    var tcs = new TaskCompletionSource<bool>();
                    
                    await _deviceDbService.SaveCameraMappings(model.Body?.OfType<ICameraMappingModel>().ToList(), tcs: tcs);
                    var result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to save CameraMappingModels in {nameof(CameraDataSaveRequest)} of {nameof(ServerService)}");


                    tcs = new TaskCompletionSource<bool>();
                    var mappings = await _deviceDbService.FetchCameraMappings(tcs: tcs);
                    result = await tcs.Task;
                    if (!result) throw new Exception(message: $"Fail to fetch CameraMappingModels in {nameof(CameraDataSaveRequest)} of {nameof(ServerService)}");


                    foreach (var item in mappings)
                    {
                        _cameraMappingProvider.Add(item);
                    }

                    //02. Send Message to the designated Clients
                    await CameraMappingSaveResponse(mappings, true, aim, endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}");
                    AcceptedClient_Event($"Raised Exception in {nameof(CameraMappingRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
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
                    _log.Error($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

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
                    _log.Error($"Raised Exception in {nameof(SymbolDataLoadRequest)} : {ex.Message}");

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
                    _log.Error($"Raised Exception in {nameof(SymbolDataSaveRequest)} : {ex.Message}");

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
                    _log.Error($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

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
                    _log.Error($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}");

                    AcceptedClient_Event($"Raised Exception in {nameof(SymbolInfoDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        //////////////////////////////////////////VMS API METHODS/////////////////////////////////////////////
        public Task VmsApiGetEventListRequest(IVmsApiGetEventListRequestModel model, IPEndPoint endPoint)
        {
            ///01. Create Model with Parameters
            ///02. Send Message to All Clients
            ///03. Send Message to UI Log
            var aim = $"Send VmsEventList Data Reponse to {endPoint.Address} Successfully.";
            return Task.Run(async () =>
            {
                try
                {
                    //01. Create Model with Parameters

                    await _vmsApiService.ApiGetEventListProcess();
                   

                    //02. Send Message to the designated Clients
                    await VmsApiGetEventListResponse(true, aim, _vmsEventProvider.OfType<Sensorway.Events.Base.Models.IEventModel>().ToList(), endPoint);

                    //03. Send Message to UI Log
                    AcceptedClient_Event($"{aim}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}");

                    //await SendDetectionEventResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"Raised Exception in {nameof(ControllerDataRequest)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }

            });
        }

        public async Task VmsApiGetEventListResponse(bool success, string msg, List<Sensorway.Events.Base.Models.IEventModel> model, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new VmsApiGetEventListResponseModel(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region TaskResponse Process
        public async Task ActionEventDetectionResponse(bool success, string msg, IActionEventModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new ActionResponseModel(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task ActionEventMalfunctionResponse(bool success, string msg, IActionEventModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new ActionResponseModel(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task ActionEventResponse(bool success, string msg, IActionEventModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new ActionResponseModel(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task ActionEventResponse(bool success, string msg, IMalfunctionEventModel model = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new ActionResponseMalfunctionModel(success, msg, model);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(ActionEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }


        private async Task SearchEventResponse(bool success, string msg, List<IDetectionEventModel> events = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new SearchDetectionResponseModel(success, msg, events);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SearchEventResponse(bool success, string msg, List<IMalfunctionEventModel> events = null, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new SearchMalfunctionResponseModel(success, msg, events);

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task SearchEventResponse(bool success, string msg
            //, List<DetectionRequestModel> detectionEvents
            //, List<MalfunctionRequestModel> malfunctionEvents
            , List<IActionEventModel> events, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new SearchActionResponseModel(success, msg, events);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(SearchEventResponse)} while sending message : {ex.Message}", endPoint);
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
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraDataResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        
        public async Task CameraDataSaveResponse(bool success, string msg, List<ICameraDeviceModel> list, IPEndPoint endPoint = null)
        {
            try
            {
                var responseModel = new CameraDataSaveResponseModel(list, success:success, content: msg);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraDataSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraOptionResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            try
            {
                var presets = _cameraOptionProvider.OfType<CameraPresetModel>().ToList();
                var profiles = _cameraOptionProvider.OfType<CameraProfileModel>().ToList();

                var responseModel = new CameraOptionResponseModel(presets, profiles, success: success, content: msg);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraOptionResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraPresetResponse(bool success, string msg, List<CameraPresetModel> list, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<CameraPresetResponseModel>(success, msg, list);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraPresetResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        


        private async Task CameraPresetSaveResponse(bool success, string msg, List<ICameraPresetModel> fetches, IPEndPoint endPoint)
        {
            try
            {
                //var responseModel = ResponseFactory.Build<CameraPresetSaveResponseModel>(success, msg);
                var responseModel = new CameraPresetSaveResponseModel(fetches, success, msg);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraPresetSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        private async Task CameraMappingResponse(bool success, string msg, List<ICameraMappingModel> list, IPEndPoint endPoint)
        {
            try
            {
                var responseModel =new CameraMappingResponseModel(success, msg, list);
                
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraMappingResponse)} while sending message : {ex.Message}", endPoint);
            }
        }
        

        private async Task CameraMappingSaveResponse(List<ICameraMappingModel> list, bool success, string msg, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = new CameraMappingSaveResponseModel(list, success: success, content: msg);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
            }
            catch (Exception ex)
            {
                throw new SocketSendException($"Raised Exception in {nameof(CameraMappingSaveResponse)} while sending message : {ex.Message}", endPoint);
            }
        }

        public async Task SymbolInfoResponse(bool success, string msg, SymbolDetailModel model, IPEndPoint endPoint)
        {
            try
            {
                var responseModel = ResponseFactory.Build<SymbolInfoResponseModel>(success, msg, model);
                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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

                await SendRequest(JsonConvert.SerializeObject(responseModel, settings), endPoint);
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
        #endregion
        #region - Attributes -
        private SymbolInfoModel _symbolInfoModel;
        private VmsControlService _vmsControlService;
        private IVmsApiService _vmsApiService;
        private IVmsDbService _vmsDbService;
        private VmsApiProvider _vmsApiProvider;
        private VmsEventProvider _vmsEventProvider;
        private VmsMappingProvider _vmsMappingProvider;
        private VmsSensorProvider _vmsSensorProvider;
        private JsonSerializerSettings settings;
        private IEventDbService _eventDbService;
        private Events.Providers.EventProvider _eventProvider;
        private DetectionEventProvider _detectionEventProvider;
        private ActionEventProvider _actionEventProvider;
        private MalfunctionEventProvider _malfunctionEventProvider;
        
        private IDeviceDbService _deviceDbService;
        private DeviceProvider _deviceProvider;
        private ControllerDeviceProvider _controllerDeviceProvider;
        private SensorDeviceProvider _sensorDeviceProvider;
        private CameraDeviceProvider _cameraDeviceProvider;

        private CameraMappingProvider _cameraMappingProvider;
        private CameraOptionProvider _cameraOptionProvider;
        private CameraPresetProvider _cameraPresetProvider;
        private CameraProfileProvider _cameraProfileProvider;
        private MapDbService _mapDbService;

        private MapProvider _mapProvider;
        private SymbolProvider _symbolProvider;
        private PointProvider _pointProvider;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        #endregion
    }
}
