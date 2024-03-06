using Ironwall.Redis.Message.Framework;
using System.Threading.Tasks;

namespace Ironwall.Libraries.WebApi.Services
{
    public interface IWebApiService
    {
        Task SendDBAsync(BrkAction packet);
        Task SendDBAsync(BrkConnection packet);
        Task SendDBAsync(BrkDectection packet);
        Task SendDBAsync(BrkMalfunction packet);
        Task SendDBAsync(BrkWindyMode packet);
    }
}