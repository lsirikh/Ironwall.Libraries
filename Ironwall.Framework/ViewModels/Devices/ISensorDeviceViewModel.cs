namespace Ironwall.Framework.ViewModels.Devices
{
    public interface ISensorDeviceViewModel
        : IBaseDeviceViewModel
    {
        IControllerDeviceViewModel Controller { get; set; }
    }
}