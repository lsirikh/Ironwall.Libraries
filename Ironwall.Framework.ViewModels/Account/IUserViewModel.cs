namespace Ironwall.Framework.ViewModels.Account
{
    public interface IUserViewModel : IUserBaseViewModel
    {
        string Address { get; set; }
        string Birth { get; set; }
        string Company { get; set; }
        string Department { get; set; }
        string EMail { get; set; }
        string EmployeeNumber { get; set; }
        string Image { get; set; }
        string Phone { get; set; }
        string Position { get; set; }
        bool IsSelected { get; set; }
    }
}