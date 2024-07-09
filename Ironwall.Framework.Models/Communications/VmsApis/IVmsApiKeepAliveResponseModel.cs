using Sensorway.Accounts.Base.Models;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiKeepAliveResponseModel : IResponseModel
    {
        LoginSessionModel Body { get; set; }
    }
}