using Sensorway.Accounts.Base.Models;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiLoginRequestModel : IBaseMessageModel
    {
        LoginUserModel Body { get; set; }
    }
}