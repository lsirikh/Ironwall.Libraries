using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IContactOffRequestModel : IBaseEventMessageModel
    {
        ContactEventModel Body { get; set; }
    }
}