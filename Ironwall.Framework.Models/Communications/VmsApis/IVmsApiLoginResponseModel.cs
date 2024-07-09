using Sensorway.Accounts.Base.Models;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiLoginResponseModel : IResponseModel
    {
        LoginSessionModel Body { get; set; }
    }
}