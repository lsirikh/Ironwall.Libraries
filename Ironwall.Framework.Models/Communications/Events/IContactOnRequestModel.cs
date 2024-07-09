using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IContactOnRequestModel : IBaseEventMessageModel
    {
        ContactEventModel Body { get; set; }
    }
}