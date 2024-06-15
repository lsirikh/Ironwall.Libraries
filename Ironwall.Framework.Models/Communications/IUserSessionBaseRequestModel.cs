namespace Ironwall.Framework.Models.Communications
{
    public interface IUserSessionBaseRequestModel : IBaseMessageModel
    {
        string TimeCreated { get; }
        string TimeExpired { get; }
        string Token { get; }
        string UserId { get; }
        string UserPass { get; }
    }
}