using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IDetectionRequestModel : IBaseMessageModel
    {
        DetectionEventModel Body { get; set; }

    }
}