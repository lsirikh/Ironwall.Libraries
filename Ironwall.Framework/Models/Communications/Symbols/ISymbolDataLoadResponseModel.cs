using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface ISymbolDataLoadResponseModel : IResponseModel
    {
        List<MapModel> Maps { get; }
        List<PointClass> Points { get; }
        List<ObjectShapeModel> Objects { get; }
        List<ShapeSymbolModel> Shapes { get; }
        List<SymbolModel> Symbols { get; }
    }
}