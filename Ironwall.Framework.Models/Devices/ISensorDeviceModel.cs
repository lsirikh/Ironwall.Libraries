namespace Ironwall.Framework.Models.Devices
{
    public interface ISensorDeviceModel : IBaseDeviceModel
    {
        ControllerDeviceModel Controller { get; set; }
    }
}