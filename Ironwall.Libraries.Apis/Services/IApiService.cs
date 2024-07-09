using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Apis.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetRequest(string url, bool isAuthentication = false, FormUrlEncodedContent formContent = null);
        Task<HttpResponseMessage> PostRequest(string body, string url, bool isAuthentication = false);
        Task<HttpResponseMessage> DeleteRequest(string url, bool isAuthentication = false);
        Task<HttpResponseMessage> PatchRequest(string url, string content, bool isAuthentication = false);
        Task<HttpResponseMessage> PutRequest(string url, string json, bool isAuthentication = false);
    }
}