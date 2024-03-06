using Ironwall.Framework.Models.Maps;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface ISymbolInfoResponseModel : IResponseModel
    {
        SymbolDetailModel Detail { get; }
    }
}