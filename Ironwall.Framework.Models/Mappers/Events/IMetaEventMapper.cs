using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Mappers
{
    public interface IMetaEventMapper : IEventMapperBase
    {
        string EventGroup { get; set; }
        int Device { get; set; }
        bool Status { get; set; }
    }
}