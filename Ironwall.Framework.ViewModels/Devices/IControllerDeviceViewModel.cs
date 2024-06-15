namespace Ironwall.Framework.ViewModels.Devices
{
    public interface IControllerDeviceViewModel 
        : IBaseDeviceViewModel
    {
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}