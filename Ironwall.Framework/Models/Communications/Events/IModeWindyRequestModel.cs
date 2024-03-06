using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IModeWindyRequestModel : IBaseMessageModel
    {
        int ModeWindy { get; set; }
    }
}