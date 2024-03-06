namespace Ironwall.Framework.Models.Devices
{
    public interface IControllerDeviceModel : IBaseDeviceModel
    {
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}