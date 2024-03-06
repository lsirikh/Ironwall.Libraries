using Ironwall.Framework.Models.Accounts;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Services
{
    public interface IAccountDbService
    {
        Task<bool> EditeUser(IUserModel model);
        Task FetchLogin(CancellationToken token = default);
        Task FetchSession(CancellationToken token = default);
        Task<ILoginSessionModel> FetchSessionToken(string token);
        Task FetchUserAll(CancellationToken token = default);
        Task<IUserModel> FetchUserId(string id);
        Task<bool> RemoveUser(IUserModel model);
        Task SaveLogin(ILoginUserModel model);
        Task SaveSession(ILoginSessionModel loginSessionModel, int mode = 0);
        Task<IUserModel> SaveUser(IUserModel model);
    }
}