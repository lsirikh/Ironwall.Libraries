using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IActionBaseRequestModel<T> : IBaseEventMessageModel where T : MetaEventModel
    {
        T Event { get; set; }
        string Content { get; set; }
        string User { get; set; }
    }
}