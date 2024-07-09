using Ironwall.Framework.Models.Maps;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface IMapFileSaveResponseModel : IResponseModel
    {
        MapDetailModel Detail { get; set; }
    }
}