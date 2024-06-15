using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Mappers
{
    public interface IMetaEventMapper : IEventMapperBase
    {
        string EventGroup { get; set; }
        EnumEventType? MessageType { get; set; }
        int Status { get; set; }
        int Device { get; set; }
    }
}