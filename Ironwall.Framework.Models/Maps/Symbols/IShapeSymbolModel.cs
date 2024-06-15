using System.Windows.Media;

namespace Ironwall.Framework.Models.Maps.Symbols
{
    public interface IShapeSymbolModel : ISymbolModel
    {
        string ShapeFill { get; set; }
        string ShapeStroke { get; set; }
        double ShapeStrokeThick { get; set; }
        PointCollection Points { get; set; }
    }
}