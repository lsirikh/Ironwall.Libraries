namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface ILogoutRequestModel : IBaseMessageModel
    {
        string Token { get; set; }
        string UserId { get; set; }
    }
}