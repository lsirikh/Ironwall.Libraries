using Ironwall.Framework.Models.Maps;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    public interface IMapViewModel : IMapBaseViewModel
    {
        int Id { get; set; }
        string FileName { get; set; }
        string FileType { get; set; }
        double Height { get; set; }
        double Width { get; set; }
        string MapName { get; set; }
        int MapNumber { get; set; }
        string Url { get; set; }
        bool Used { get; set; }
        bool Visibility { get; set; }
        IMapModel Model { get; }

        void Refresh();
    }
}