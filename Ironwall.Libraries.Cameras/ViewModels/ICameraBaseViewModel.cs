using Ironwall.Libraries.Cameras.Models;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public interface ICameraBaseViewModel 
        : ICameraBaseModel
    {
        bool IsSelected { get; set; }
    }
}