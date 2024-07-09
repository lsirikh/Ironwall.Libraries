using Ironwall.Framework.Models.Devices;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    interface ICameraProfileViewModel : ICameraOptionViewModel
    {
        string Profile { get; set; }

    }
}