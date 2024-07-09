using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Mappers
{
    public interface IEventMapperBase : IBaseModel
    {
        int MessageType { get; set; }
        string DateTime { get; set; }
    }
}