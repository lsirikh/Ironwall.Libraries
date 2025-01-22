using Ironwall.Framework.Models.Ais;

namespace Ironwall.Framework.Models.Communications.AIs
{
    public interface IAiApiSettingRequestModel : IBaseMessageModel
    {
        NetworkSettingModel SettingModel { get; set; }
    }
}