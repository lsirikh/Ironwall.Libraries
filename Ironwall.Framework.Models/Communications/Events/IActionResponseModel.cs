using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IActionResponseModel : IResponseModel
    {
        ActionEventModel Body { get; set; }
    }
}