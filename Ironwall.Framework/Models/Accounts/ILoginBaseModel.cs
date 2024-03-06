namespace Ironwall.Framework.Models.Accounts
{
    public interface ILoginBaseModel : IAccountBaseModel
    {
        string TimeCreated { get; set; }
        string UserId { get; set; }
    }
}