using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Events
{
    public interface IModeWindyEventModel : IBaseEventModel
    {
        EnumWindyMode ModeWindy { get; set; }
    }
}