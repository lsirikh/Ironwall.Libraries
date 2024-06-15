namespace Ironwall.Framework.ViewModels.Devices
{
    public interface ICameraDeviceViewModel
        : IBaseDeviceViewModel
    {
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}