using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.WebApi.Modules
{
    public interface IWebApiClient
    {
        Task<object> PostAsync(WebApiRestRequest request, CancellationToken token = default);
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}