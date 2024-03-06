using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.WebApi.Modules;
using Ironwall.Libraries.WebApi.Utils;
using Ironwall.Middleware.Message.Framework;
using Ironwall.Redis.Message.Framework;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using RestSharp;

namespace Ironwall.Libraries.WebApi.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 2:52:19 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WebApiService : IWebApiService
    {

        #region - Ctors -
        public WebApiService(ILogService log
                            , IWebApiClient client)
        {
            _log = log;
            _client = client;
        }
        #endregion
        #region - Implementation of Interface -
        public Task SendDBAsync(BrkConnection packet)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = PacketHelper.JtoSConverter(packet);
                    Debug.WriteLine($"==>{json}");

                    var request = new WebApiRestRequest("/api/event/create/").AddJsonBody(json);
                    var response = await _client.PostAsync((WebApiRestRequest)request);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendDBAsync)} of {nameof(WebApiService)} [{_client.IpAddress}:{_client.Port}] : {ex}", true);
                }
            });
            
        }

        public Task SendDBAsync(BrkDectection packet)
        {
            return Task.Run(async () => 
            {
                try
                {
                    var json = PacketHelper.JtoSConverter(packet);
                    Debug.WriteLine($"==>{json}");

                    var request = new WebApiRestRequest("/api/event/create/").AddJsonBody(json);
                    var response = await _client.PostAsync((WebApiRestRequest)request);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendDBAsync)} of {nameof(WebApiService)} [{_client.IpAddress}:{_client.Port}] : {ex}", true);
                }
            });
        }

        public Task SendDBAsync(BrkMalfunction packet)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = PacketHelper.JtoSConverter(packet);
                    Debug.WriteLine($"==>{json}");

                    var request = new WebApiRestRequest("/api/event/create/").AddJsonBody(json);
                    var response = await _client.PostAsync((WebApiRestRequest)request);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendDBAsync)} of {nameof(WebApiService)} [{_client.IpAddress}:{_client.Port}] : {ex}", true);
                }
            });
            
        }

        //조치보고
        public Task SendDBAsync(BrkAction packet)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = PacketHelper.JtoSConverter(packet);
                    Debug.WriteLine($"==>{json}");

                    var request = new WebApiRestRequest("/api/event/create/").AddJsonBody(json);
                    var response = await _client.PostAsync((WebApiRestRequest)request);

                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendDBAsync)} of {nameof(WebApiService)} : {ex}", true);
                }
            });
            
        }

        //강풍모드
        public Task SendDBAsync(BrkWindyMode packet)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var json = PacketHelper.JtoSConverter(packet);
                    Debug.WriteLine($"==>{json}");

                    var request = new WebApiRestRequest("/api/event/create/").AddJsonBody(json);
                    var response = await _client.PostAsync((WebApiRestRequest)request);

                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendDBAsync)} of {nameof(WebApiService)} : {ex}", true);
                }
            });
            
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private IWebApiClient _client;
        #endregion
    }
}
