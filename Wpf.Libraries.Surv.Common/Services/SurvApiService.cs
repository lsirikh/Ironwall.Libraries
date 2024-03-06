using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;
using Wpf.Libraries.Surv.Common.Models;
using Ironwall.Libraries.Base.Services;
using Wpf.Libraries.Surv.Common.Providers.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Maps;

namespace Wpf.Libraries.Surv.Common.Sdk
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/26/2023 3:57:52 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvApiService : TaskService
    {

        #region - Ctors -
        public SurvApiService(ILogService log
                            , SurvApiModelProvider survApiModelProvider
                            , SurvCameraModelProvider survCameraModelProvider
                            , SurvEventModelProvider survEventModelProvider
                            , SurvMappingModelProvider survMappingModelProvider
                            , SurvSensorModelProvider survSensorModelProvider)
        {
            _apiModelProvider = survApiModelProvider;
            _cameraModelProvider = survCameraModelProvider;
            _eventModelProvider = survEventModelProvider;
            _mappingModelProvider = survMappingModelProvider;
            _sensorModelProvider = survSensorModelProvider;

            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task RunTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void CreateLookupTable()
        {
            LookupTable = from mappingmodel in _mappingModelProvider
                          join sensormodel in _sensorModelProvider
                          on mappingmodel.GroupName equals sensormodel.GroupName into sensorGroup
                          from sensormodel in sensorGroup.DefaultIfEmpty()

                          join eventmodel in _eventModelProvider
                          on mappingmodel.EventId equals eventmodel.Id into eventmodelGroup
                          from eventmodel in eventmodelGroup.DefaultIfEmpty()

                          join apimodel in _apiModelProvider
                          on eventmodel.ApiId equals apimodel.Id into apiGroup
                          from apimodel in apiGroup.DefaultIfEmpty()

                          join cameramodel in _cameraModelProvider
                          on eventmodel.CameraId equals cameramodel.Id into cameraGroup
                          from cameramodel in cameraGroup.DefaultIfEmpty()

                          select new SurvLookupModel
                          {
                              MappingModel = mappingmodel,
                              SensorModel = sensormodel,
                              EventModel = eventmodel,
                              ApiModel = apimodel,
                              CameraModel = cameramodel,
                          };
            _log.Info($"<<SurvApiService LookupTable>>", true);
            foreach (var item in LookupTable)
            {
                _log.Info($"Group : {item?.MappingModel?.GroupName}" +
                    $", Api: {item?.ApiModel?.ApiAddress}:{item?.ApiModel.ApiPort}" +
                    $", Event : {item?.EventModel?.IpAddress}" +
                    $", Camera : {item?.CameraModel?.DeviceName}" +
                    $", Sensor : {item?.SensorModel?.ControllerId}-{item?.SensorModel?.SensorId}", true);
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void CreateApiModel(int id, string ipAddress, uint port, string username, string password)
        {
            ApiModel = new SurvApiModel(id, ipAddress, port, username, password);
        }

        public void CreateEventModel(int id, int channel, string ipAddress, string eventName, bool isOn, int eventId)
        {
            EventModel = new SurvEventModel(id, channel, ipAddress, eventName, isOn, eventId);
        }

        public void CreateInstance()
        {
            if (ApiModel == null) return;
            _handle = EsCreateInstance(ApiModel.ApiAddress, ApiModel.ApiPort, ApiModel.UserName, ApiModel.Password);
            if (_handle != null)
                _log.Info($"API({ApiModel.ApiAddress}) 연결 인스턴스 생성!");
        }

        public IntPtr CreateInstance(ISurvApiModel api)
        {
            if (api == null) return new IntPtr();
            var handle = EsCreateInstance(api.ApiAddress, api.ApiPort, api.UserName, api.Password);
            
            if (handle != null)
                _log.Info($"API({api.ApiAddress}) 연결 인스턴스 생성!");

            return handle;
        }

        public void CloseInstance()
        {
            if (_handle == null) return;
            EsDeleteInstance(_handle);
            ApiModel = null;
            EventModel = null;
            _log.Info($"API({ApiModel.ApiAddress}) 연결 인스턴스 해제!");
        }

        public void CloseInstance(IntPtr handle, ISurvApiModel api)
        {
            if (_handle == null) return;
            EsDeleteInstance(handle);

            _log.Info($"API({api.ApiAddress}) 연결 인스턴스 해제!");
        }

        public Task SendEventToSurv()
        {
            return Task.Run(() =>
            {
                try
                {
                    if(ApiModel == null || EventModel == null) return;

                    EventInfo evtInfo = new EventInfo
                    {
                        nCamChannel = EventModel.Channel,
                        pszCamAddress = Marshal.StringToHGlobalAnsi(EventModel.IpAddress),
                        pszEventName = Marshal.StringToHGlobalAnsi(EventModel.EventName),
                        bOn = EventModel.IsOn,
                        nId = EventModel.EventId
                    };
                    var size = Marshal.SizeOf(typeof(EventInfo));
                    bool result = EsSendEvent(_handle, ref evtInfo);
                    _log.Info($"전송 결과 : {result}");
                    Marshal.FreeHGlobal(evtInfo.pszCamAddress);
                    Marshal.FreeHGlobal(evtInfo.pszEventName);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendEventToSurv)} : {ex.Message}");
                }
            });
        }

        public Task SendEventToSurv(EventInfo evtInfo, IntPtr handel)
        {
            try
            {
                //var size = Marshal.SizeOf(typeof(EventInfo));
                bool result = EsSendEvent(handel, ref evtInfo);
                _log.Info($"전송 결과 : {result}");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(SendEventToSurv)} : {ex.Message}");
            }
            finally
            {
                Marshal.FreeHGlobal(evtInfo.pszCamAddress);
                Marshal.FreeHGlobal(evtInfo.pszEventName);
            }
            return Task.CompletedTask;
        }

        public Task SendEventCycle(SurvLookupModel lookupModel, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                /// Process
                /// 1. Create Api Model
                /// 2. Create Api Instance
                /// 3. Create Event Model
                /// 4. Send Process
                /// 5. Task Delay to be cancelled
                /// 6. Create Event Model
                /// 7. Send Process
                
                if(lookupModel == null) return;
                var apiModel = lookupModel?.ApiModel;
                var eventModel = lookupModel.EventModel;
                IntPtr handle = new IntPtr();
                try
                {
                    handle = CreateInstance(apiModel);
                    EventInfo evtStartInfo = new EventInfo
                    {
                        nCamChannel = eventModel.Channel,
                        pszCamAddress = Marshal.StringToHGlobalAnsi(eventModel.IpAddress),
                        pszEventName = Marshal.StringToHGlobalAnsi(eventModel.EventName),
                        bOn = true, //eventModel.IsOn
                        nId = eventModel.EventId
                    };

                    await SendEventToSurv(evtStartInfo, handle);
                    await Task.Delay(30000, token);
                    EventInfo evtFinishInfo = new EventInfo
                    {
                        nCamChannel = eventModel.Channel,
                        pszCamAddress = Marshal.StringToHGlobalAnsi(eventModel.IpAddress),
                        pszEventName = Marshal.StringToHGlobalAnsi(eventModel.EventName),
                        bOn = false, //eventModel.IsOn
                        nId = eventModel.EventId
                    };
                    await SendEventToSurv(evtFinishInfo, handle);
                    CloseInstance(handle, apiModel);
                }
                catch (TaskCanceledException)
                {
                    EventInfo evtFinishInfo = new EventInfo
                    {
                        nCamChannel = eventModel.Channel,
                        pszCamAddress = Marshal.StringToHGlobalAnsi(eventModel.IpAddress),
                        pszEventName = Marshal.StringToHGlobalAnsi(eventModel.EventName),
                        bOn = false, //eventModel.IsOn
                        nId = eventModel.EventId
                    };
                    await SendEventToSurv(evtFinishInfo, handle);
                    CloseInstance(handle, apiModel);
                    _log.Info($"Task{nameof(SendEventCycle)} was cancelled");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendEventCycle)} : {ex.Message}");
                }
                
            });
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SurvApiModel ApiModel { get; set; }
        public SurvEventModel EventModel { get; set; }
        public IEnumerable<SurvLookupModel> LookupTable { get; set; }
        #endregion
        #region - Attributes -
        private IntPtr _handle;
        private SurvApiModelProvider _apiModelProvider;
        private SurvCameraModelProvider _cameraModelProvider;
        private SurvEventModelProvider _eventModelProvider;
        private SurvMappingModelProvider _mappingModelProvider;
        private SurvSensorModelProvider _sensorModelProvider;
        private ILogService _log;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventInfo
        {
            public int nCamChannel;

            public IntPtr pszCamAddress;

            public IntPtr pszEventName;

            public bool bOn;

            public int nId;
        }

        private const string x64RelativePath = @"Dlls\x64\EvtSrv.dll";
        private const string x86RelativePath = @"Dlls\x86\EvtSrv.dll";

#if x64
        [DllImport(x64RelativePath, CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport(x86RelativePath, CallingConvention = CallingConvention.Cdecl)]
#endif
        static extern IntPtr EsCreateInstance(string ipaddress, uint port, string username, string password);

#if x64
    [DllImport(x64DllPath, CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport(x86RelativePath, CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void EsDeleteInstance(IntPtr ptr);

#if x64
        [DllImport(x64DllPath, CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport(x86RelativePath, CallingConvention = CallingConvention.Cdecl)]
#endif
        static extern bool EsSendEvent(IntPtr hEs, ref EventInfo pEvt);

        
        #endregion
    }
}
