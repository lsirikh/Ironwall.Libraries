using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IModeWindyRequestModel : IBaseMessageModel
    {
        EnumWindyMode ModeWindy { get; set; }
    }
}