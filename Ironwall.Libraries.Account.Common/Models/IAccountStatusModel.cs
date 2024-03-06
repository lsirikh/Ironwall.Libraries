using Ironwall.Framework.Models.Accounts;

namespace Ironwall.Libraries.Account.Common.Models
{
    public interface IAccountStatusModel
    {
        bool IsLogin { get; set; }
        int Level { get; set; }
        string Status { get; set; }
        ILoginSessionModel SessionModel { get; set; }
        IUserModel UserModel { get; set; }
    }
}