using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Wpf.Surv.Sdk.Models;
using System.Threading.Tasks;

namespace Wpf.Surv.Sdk
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/26/2023 3:57:52 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvApiService
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void CreateApiModel(string ipAddress, uint port, string username, string password)
        {
            ApiModel = new SurvApiModel(ipAddress, port, username, password);
        }

        public void CreateEventModel(int channel, string ipAddress, string eventName, bool isOn, int id)
        {
            EventModel = new SurvEventModel(channel, ipAddress, eventName, isOn, id);
        }

        public void CreateInstance()
        {
            if (ApiModel == null) return;
            _handle = EsCreateInstance(ApiModel.ApiAddress, ApiModel.ApiPort, ApiModel.UserName, ApiModel.Password);
            if (_handle != null)
                Debug.WriteLine("연결 인스턴스 생성!");
        }

        public void CloseInstance()
        {
            if (_handle == null) return;
            EsDeleteInstance(_handle);
            ApiModel = null;
            EventModel = null;
            Debug.WriteLine("연결 인스턴스 해제!");
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
                        pszCamAddress = Marshal.StringToHGlobalAnsi(EventModel.CamIpAddress),
                        pszEventName = Marshal.StringToHGlobalAnsi(EventModel.EventName),
                        bOn = EventModel.IsOn,
                        nId = EventModel.Id
                    };
                    var size = Marshal.SizeOf(typeof(EventInfo));
                    //Debug.WriteLine($"size = {size}");
                    bool result = EsSendEvent(_handle, ref evtInfo);
                    Debug.WriteLine($"전송 결과 : {result}");
                    Marshal.FreeHGlobal(evtInfo.pszCamAddress);
                    Marshal.FreeHGlobal(evtInfo.pszEventName);

                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(SendEventToSurv)} : {ex.Message}");
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SurvApiModel ApiModel { get; set; }
        public SurvEventModel EventModel { get; set; }
        #endregion
        #region - Attributes -
        private IntPtr _handle;

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
