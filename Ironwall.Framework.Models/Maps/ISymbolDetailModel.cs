using Ironwall.Framework.Models.Communications;
using System;

namespace Ironwall.Framework.Models.Maps
{
    public interface ISymbolDetailModel : IUpdateDetailBaseModel
    {
        int Map { get; set; }
        int ObjectShape { get; set; }
        int ShapeSymbol { get; set; }
        int Symbol { get; set; }
    }
}