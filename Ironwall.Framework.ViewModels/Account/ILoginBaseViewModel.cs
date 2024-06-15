namespace Ironwall.Framework.ViewModels.Account
{
    public interface ILoginBaseViewModel : IAccountBaseViewModel
    {
        string TimeCreated { get; set; }
        string UserId { get; set; }
    }
}