namespace Ironwall.Framework.ViewModels.Account
{
    public interface IAccountBaseViewModel
    {
        int Id { get; set; }
        void ClearProperty();
    }
}