using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface ISymbolDataSaveRequestModel : IUserSessionBaseRequestModel
    {
        //List<MapModel> Maps { get; }
        List<ObjectShapeModel> Objects { get; }
        List<PointClass> Points { get; }
        List<ShapeSymbolModel> Shapes { get; }
        List<SymbolModel> Symbols { get; }
    }
}