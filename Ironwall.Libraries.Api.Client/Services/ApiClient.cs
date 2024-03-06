using Ironwall.Libraries.Api.Common.Defines;
using Ironwall.Libraries.Api.Common.Models;
using System.Diagnostics;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Timers;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace Ironwall.Libraries.Api.Client.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/15/2023 2:35:11 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class ApiClient : ApiTaskTimer
    {

        #region - Ctors -
        public ApiClient()
        {
            
        }
        #endregion
        #region - Implementation of Interface -
        protected override void Tick(object sender, ElapsedEventArgs e)
        {
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        public async Task<string> PostRequest(ApiServerModel model, string msg)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential(model.Username, model.Password);

                string responseBody = null;

                using (HttpClient client = new HttpClient(handler))
                {
                    // 타임 아웃 설정
                    client.Timeout = TimeSpan.FromSeconds(5);
                    try
                    {
                        // POST 요청 URL 구성
                        var url = $"http://{model.IpAddress}:{model.Port}/{model.Url}";
                        Uri requestUri = new Uri(url);

                        // POST 요청 전송
                        var content = new StringContent(msg, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(requestUri, content);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            response.EnsureSuccessStatusCode();
                            responseBody = await response.Content.ReadAsStringAsync();
                            Received?.Invoke(responseBody);
                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Debug.WriteLine($"Fail to login");
                            Received?.Invoke($"Fail to login");
                        }
                        else
                        {
                            Debug.WriteLine($"Fail to send message to {url}");
                            Received?.Invoke($"Fail to send message to {url}");
                        }
                        return responseBody;
                    }
                    catch (HttpRequestException ex)
                    {
                        // 연결 실패 또는 다른 HTTP 요청 오류 처리
                        Debug.WriteLine($"Raised {nameof(HttpRequestException)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                        Received?.Invoke($"Raised {nameof(HttpRequestException)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                    }
                    catch (TaskCanceledException ex)
                    {
                        // 타임아웃 처리
                        Debug.WriteLine($"Raised {nameof(TaskCanceledException)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                        Received?.Invoke($"Raised {nameof(TaskCanceledException)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                Received?.Invoke($"Raised {nameof(Exception)} in {nameof(PostRequest)} of {nameof(ApiClient)} : {ex}");
                return null;
            }
            
        }

        public async Task<string> GetRequest(ApiServerModel model)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential(model.Username, model.Password);
                    
                string responseBody = null;

                using (HttpClient client = new HttpClient(handler))
                {
                    // 타임 아웃 설정
                    client.Timeout = TimeSpan.FromSeconds(5);

                    try
                    {
                        // GET 요청 URL 구성
                        var url = $"http://{model.IpAddress}:{model.Port}/{model.Url}";
                        Uri requestUri = new Uri(url);

                        // GET 요청 전송
                        HttpResponseMessage response = await client.GetAsync(requestUri);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            response.EnsureSuccessStatusCode();
                            responseBody = await response.Content.ReadAsStringAsync();
                            Received?.Invoke(responseBody);
                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Debug.WriteLine($"Fail to login");
                            Received?.Invoke($"Fail to login");
                        }
                        else
                        {
                            Debug.WriteLine($"Fail to send message to {url}");
                            Received?.Invoke($"Fail to send message to {url}");
                        }
                        return responseBody;
                    }
                    catch (HttpRequestException ex)
                    {
                        // 연결 실패 또는 다른 HTTP 요청 오류 처리
                        Debug.WriteLine($"Raised {nameof(HttpRequestException)} in {nameof(GetRequest)} of {nameof(ApiClient)} : {ex}");
                        Received?.Invoke($"Raised {nameof(HttpRequestException)} in {nameof(GetRequest)} of {nameof(ApiClient)} : {ex}");
                    }
                    catch (TaskCanceledException ex)
                    {
                        // 타임아웃 처리
                        Debug.WriteLine($"Raised {nameof(TaskCanceledException)} in {nameof(GetRequest)} of {nameof(ApiClient)} : {ex}");
                        Received?.Invoke($"Raised {nameof(TaskCanceledException)} in {nameof(GetRequest)} of {nameof(ApiClient)} : {ex}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised {nameof(TaskCanceledException)} in {nameof(GetRequest)} : {ex}");
                Received?.Invoke($"Raised {nameof(TaskCanceledException)} in {nameof(GetRequest)} of {nameof(ApiClient)} : {ex}");
                return null;
            }
            
        }

        protected void RaiseReceivedEvent(string message)
        {
            Received?.Invoke(message);
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        //private HttpClient client;
        //private CancellationTokenSource cts;

        //public event Action<string> Log;
        public event Action<string> Received;
        #endregion
    }
}
