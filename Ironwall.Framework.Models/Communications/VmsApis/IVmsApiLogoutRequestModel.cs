using Sensorway.Accounts.Base.Models;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiLogoutRequestModel : IBaseMessageModel
    {
        LoginSessionModel Body { get; set; }
    }
}