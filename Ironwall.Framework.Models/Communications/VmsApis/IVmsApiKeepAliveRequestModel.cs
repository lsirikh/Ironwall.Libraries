namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiKeepAliveRequestModel : IBaseMessageModel
    {
        string Token { get; set; }
    }
}