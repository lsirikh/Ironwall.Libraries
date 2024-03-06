using Ironwall.Framework.Models.Accounts;

namespace Ironwall.Framework.ViewModels.Account
{
    public interface IUserBaseViewModel_
    {
        string Address { get; set; }
        string Birth { get; set; }
        string Company { get; set; }
        string Department { get; set; }
        string EMail { get; set; }
        string EmployeeNumber { get; set; }
        string IdUser { get; set; }
        string Image { get; set; }
        int Level { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        string Phone { get; set; }
        string Position { get; set; }
        bool Used { get; set; }
        IUserModel Model { get; set; }
        ILoginSessionModel SessionModel { get; set; }

        void Clear();
        
    }
}