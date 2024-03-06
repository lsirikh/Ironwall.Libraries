using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Events
{
    public interface IMetaEventModel : IBaseEventModel
    {
        string EventGroup { get; set; }
        int MessageType { get; set; }
        int Status { get; set; }
        IBaseDeviceModel Device { get; set; }
    }
}