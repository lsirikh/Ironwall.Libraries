using Ironwall.Framework.Models.Devices;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface ICameraProfileViewModel : ICameraOptionViewModel
    {
        string Profile { get; set; }

    }
}