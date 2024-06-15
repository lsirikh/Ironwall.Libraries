using Ironwall.Framework.Models.Maps.Symbols;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface ISymbolResponseModel : IResponseModel
    {
        List<SymbolModel> Symbols { get; }
    }
}