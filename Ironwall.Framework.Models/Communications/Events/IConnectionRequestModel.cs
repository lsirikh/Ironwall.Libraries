using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IConnectionRequestModel : IBaseEventMessageModel
    {
        ConnectionEventModel Body { get; set; }
    }
}