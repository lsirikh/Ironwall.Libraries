namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IKeepAliveResponseModel : IResponseModel
    {
        string TimeExpired { get; set; }
    }
}