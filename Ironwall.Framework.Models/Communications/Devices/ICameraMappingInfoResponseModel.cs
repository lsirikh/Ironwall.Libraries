using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraMappingInfoResponseModel : IResponseModel
    {
        MappingInfoModel Detail { get; }
    }
}