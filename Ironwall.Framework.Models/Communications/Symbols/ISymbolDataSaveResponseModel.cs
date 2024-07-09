using Ironwall.Framework.Models.Maps;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface ISymbolDataSaveResponseModel : IResponseModel
    {
        SymbolMoreDetailModel Detail { get; }
    }
}