namespace Ironwall.Framework.ViewModels.Account
{
    public interface IUserBaseViewModel : IAccountBaseViewModel
    {
        string IdUser { get; set; }
        int Level { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        bool Used { get; set; }
    }
}