using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{

    public interface IActionRequestModel : IBaseEventMessageModel
    {
        //int EventId { get; set; }
        //string Content { get; set; }
        //string User { get; set; }

        ActionEventModel Body { get; set; }
    }
}