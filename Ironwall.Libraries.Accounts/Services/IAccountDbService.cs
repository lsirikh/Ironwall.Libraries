using Ironwall.Framework.Models.Accounts;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Accounts.Services
{
    public interface IAccountDbService
    {
        Task<UserModel> EditeUser(UserModel model);
        Task FetchLogin();
        Task FetchSession();
        Task<LoginSessionModel> FetchSessionToken(string token);
        Task FetchUserAll();
        Task<UserModel> FetchUserId(string id);
        Task<bool> RemoveUser(UserModel model);
        Task SaveLogin(LoginUserModel model);
        Task SaveSession(LoginSessionModel loginSessionModel, int mode = 0);
        Task<UserModel> SaveUser(UserModel model);
    }
}