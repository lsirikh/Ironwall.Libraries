namespace Ironwall.Framework.ViewModels.Account
{
    public interface ILoginUserViewModel : ILoginBaseViewModel
    {
        int ClientId { get; set; }
        int Mode { get; set; }
        int UserLevel { get; set; }
    }
}