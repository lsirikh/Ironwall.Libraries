using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Events
{
    public interface IDetectionEventModel : IMetaEventModel
    {
        int Result { get; set; }
    }
}