using System.Net.Http;
using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Libraries.Apis.Models;
using System.Diagnostics;
using System.Text;
using System.Reflection.Emit;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Apis.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 2/8/2024 11:14:36 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ApiService : IService, IApiService
    {

        #region - Ctors -
        public ApiService(ILogService log = default)
        {
            
        }
        public ApiService(ILogService log
                        , ApiSetupModel setupModel)
        {
            _log = log;
            _setupModel = setupModel;
            _class = this.GetType();
        }
        #endregion
        #region - Implementation of Interface -
        public Task ExecuteAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task<HttpResponseMessage> GetRequest(string url, bool isAuthentication = false, FormUrlEncodedContent formContent = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    
                    if(isAuthentication)
                        handler.Credentials = new NetworkCredential(_setupModel.Username, _setupModel.Password);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // 3초 타임아웃 설정
                        client.Timeout = TimeSpan.FromSeconds(TIMEOUT);

                        try
                        {
                            // GET 요청 URL 구성
                            Uri requestUri = null;
                            if (formContent != null)
                            {
                                // FormUrlEncodedContent를 문자열로 변환
                                var formDataStr = await formContent.ReadAsStringAsync();
                                requestUri = new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}?{formDataStr}");
                            }
                            else
                                requestUri = new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}");

                            // GET 요청 전송
                            return await client.GetAsync(requestUri);
                            
                        }
                        catch (HttpRequestException ex)
                        {
                            // 연결 실패 또는 다른 HTTP 요청 오류 처리
                            //_log?.Error($"Raised {nameof(HttpRequestException)} in {nameof(GetRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                        catch (TaskCanceledException ex)
                        {
                            // 타임아웃 처리
                            //_log?.Error($"Raised {nameof(TaskCanceledException)} in {nameof(GetRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.Error($"Raised {nameof(Exception)} in {nameof(GetRequest)} of {nameof(ApiService)} : {ex}", _class, true);
                    return null;
                }
            });
        }

        public async Task<HttpResponseMessage> PostRequest(string body, string url, bool isAuthentication = false)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();

                if (isAuthentication)
                    handler.Credentials = new NetworkCredential(_setupModel.Username, _setupModel.Password);

                //string responseBody = null;

                using (HttpClient client = new HttpClient(handler))
                {
                    // 타임 아웃 설정
                    client.Timeout = TimeSpan.FromSeconds(TIMEOUT);
                    try
                    {
                        // POST 요청 URL 구성
                        Uri requestUri = new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}");

                        // POST 요청 전송
                        var content = new StringContent(body, Encoding.UTF8, "application/json");
                        return await client.PostAsync(requestUri, content);

                    }
                    catch (HttpRequestException ex)
                    {
                        // 연결 실패 또는 다른 HTTP 요청 오류 처리
                        //_log?.Error($"Raised {nameof(HttpRequestException)} in {nameof(PostRequest)} of {nameof(ApiService)} : {ex}");
                        return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };

                    }
                    catch (TaskCanceledException ex)
                    {
                        // 타임아웃 처리
                        //_log?.Error($"Raised {nameof(TaskCanceledException)} in {nameof(PostRequest)} of {nameof(ApiService)} : {ex}");
                        return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised {nameof(Exception)} in {nameof(PostRequest)} of {nameof(ApiService)} : {ex}");
                return null;
            }
        }

        public Task<HttpResponseMessage> DeleteRequest(string url, bool isAuthentication = false)
        {
            return Task.Run(async () =>
            {
                try
                {
                    HttpClientHandler handler = new HttpClientHandler();

                    if (isAuthentication)
                        handler.Credentials = new NetworkCredential(_setupModel.Username, _setupModel.Password);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // 3초 타임아웃 설정
                        client.Timeout = TimeSpan.FromSeconds(TIMEOUT);

                        try
                        {
                            // DELETE 요청 URL 구성
                            Uri requestUri = new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}");

                            // DELETE 요청 전송
                            return await client.DeleteAsync(requestUri);
                            
                        }
                        catch (HttpRequestException ex)
                        {
                            // 연결 실패 또는 다른 HTTP 요청 오류 처리
                            //_log?.Error($"Raised {nameof(HttpRequestException)} in {nameof(DeleteRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                        catch (TaskCanceledException ex)
                        {
                            // 타임아웃 처리
                            //_log?.Error($"Raised {nameof(TaskCanceledException)} in {nameof(DeleteRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.Error($"Raised {nameof(Exception)} in {nameof(DeleteRequest)} of {nameof(ApiService)} : {ex}", _class, true);
                    return null;
                }
            });
        }

        public Task<HttpResponseMessage> PatchRequest(string url, string json, bool isAuthentication = false)
        {
            return Task.Run(async () =>
            {
                try
                {
                    HttpClientHandler handler = new HttpClientHandler();

                    if (isAuthentication)
                        handler.Credentials = new NetworkCredential(_setupModel.Username, _setupModel.Password);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // 3초 타임아웃 설정
                        client.Timeout = TimeSpan.FromSeconds(TIMEOUT);

                        try
                        {
                            // PATCH 요청 URL 구성
                            Uri requestUri =  new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}");

                            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
                            {
                                Content = new StringContent(json, Encoding.UTF8, "application/json")
                            };

                            // PATCH 요청 전송
                            return await client.SendAsync(request);
                            
                        }
                        catch (HttpRequestException ex)
                        {
                            // 연결 실패 또는 다른 HTTP 요청 오류 처리
                            //_log?.Error($"Raised {nameof(HttpRequestException)} in {nameof(PatchRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                        catch (TaskCanceledException ex)
                        {
                            // 타임아웃 처리
                            //_log?.Error($"Raised {nameof(TaskCanceledException)} in {nameof(PatchRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.Error($"Raised {nameof(Exception)} in {nameof(PatchRequest)} of {nameof(ApiService)} : {ex}", _class, true);
                    return null;
                }
            });
        }

        public Task<HttpResponseMessage> PutRequest(string url, string json, bool isAuthentication = false)
        {
            return Task.Run(async () =>
            {
                try
                {
                    HttpClientHandler handler = new HttpClientHandler();

                    if (isAuthentication)
                        handler.Credentials = new NetworkCredential(_setupModel.Username, _setupModel.Password);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // 3초 타임아웃 설정
                        client.Timeout = TimeSpan.FromSeconds(TIMEOUT);

                        try
                        {
                            // PUT 요청 URL 구성
                            Uri requestUri = new Uri($"http://{_setupModel.IpAddress}:{_setupModel.Port}/{url}");
                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            // PUT 요청 전송
                            return await client.PutAsync(requestUri, content);
                            
                        }
                        catch (HttpRequestException ex)
                        {
                            // 연결 실패 또는 다른 HTTP 요청 오류 처리
                            //_log?.Error($"Raised {nameof(HttpRequestException)} in {nameof(PutRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                        catch (TaskCanceledException ex)
                        {
                            // 타임아웃 처리
                            //_log?.Error($"Raised {nameof(TaskCanceledException)} in {nameof(PutRequest)} of {nameof(ApiService)} : {ex}", true);
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.Message };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.Error($"Raised {nameof(Exception)} in {nameof(PutRequest)} of {nameof(ApiService)} : {ex}", _class, true);
                    return null;
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private ApiSetupModel _setupModel;
        private Type _class;
        private const int TIMEOUT = 10;
        #endregion
    }
}
