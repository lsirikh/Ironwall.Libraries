namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface IControllerDeviceViewModel : IDeviceViewModel
    {
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}