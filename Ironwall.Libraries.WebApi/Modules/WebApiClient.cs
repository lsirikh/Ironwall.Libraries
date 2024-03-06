using Ironwall.Message.Base;
using RestSharp;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using Ironwall.Middleware.Message.Framework;
using Ironwall.Libraries.WebApi.Utils;
using System.Threading;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.WebApi.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 2:17:48 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WebApiClient : IWebApiClient
    {

        #region - Ctors -
        public WebApiClient(IRestClient restClient)
        {
            _restClient = restClient;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public async Task<object> PostAsync(WebApiRestRequest request, CancellationToken token = default)
        {
            try
            {
                return await _restClient.PostAsync<object>(request, token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Raised {nameof(Exception)} in {nameof(PostAsync)} of {nameof(WebApiClient)} : {ex}");
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string IpAddress { get ; set ; }
        public int Port { get ; set ; }
        #endregion
        #region - Attributes -
        private IRestClient _restClient;
        private ILogService _log;
        #endregion
    }
}
