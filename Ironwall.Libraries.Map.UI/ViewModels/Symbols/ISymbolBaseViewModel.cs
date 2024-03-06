using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using System.Windows;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    public interface ISymbolBaseViewModel<T> : IScreen where T : ISymbolModel
    {
        void UpdateModel(T model);
        void Dispose();
        void OnClickEdit(object sender, RoutedEventArgs args);
        void OnClickExit(object sender, RoutedEventArgs args);
        void OnClickDelete(object sender, RoutedEventArgs args);
        int Id { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Angle { get; set; }
        string Lable { get; set; }
        double FontSize { get; set; }
        string FontColor { get; set; }
        bool IsShowLable { get; set; }
        int TypeShape { get; set; }
        int Layer { get; set; }
        int Map { get; set; }
        bool IsUsed { get; set; }
        bool IsVisible { get; set; }
        bool IsSelected { get; set; }
        bool OnEditable { get; set; }
        bool IsEditable { get; set; }

    }
}