namespace Ironwall.Framework.ViewModels.Account
{
    public interface ILoginSessionViewModel : ILoginBaseViewModel
    {
        string TimeExpired { get; set; }
        string Token { get; set; }
        string UserPass { get; set; }
    }
}