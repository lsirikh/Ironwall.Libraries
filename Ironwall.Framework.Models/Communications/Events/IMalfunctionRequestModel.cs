using Ironwall.Framework.Models.Events;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IMalfunctionRequestModel: IBaseMessageModel
    {
        MalfunctionEventModel Body { get; set; }
    }
}