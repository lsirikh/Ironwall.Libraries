namespace Ironwall.Framework.Models.Devices
{
    public interface ISensorDeviceModel : IBaseDeviceModel
    {
        IControllerDeviceModel Controller { get; set; }
    }
}