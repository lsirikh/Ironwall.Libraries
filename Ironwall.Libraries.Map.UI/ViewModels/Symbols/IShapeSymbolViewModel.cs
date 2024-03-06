using System.Windows.Media;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    public interface IShapeSymbolViewModel : ISymbolViewModel
    {
        PointCollection Points { get; set; }
        string ShapeFill { get; set; }
        string ShapeStroke { get; set; }
        double ShapeStrokeThick { get; set; }
    }
}