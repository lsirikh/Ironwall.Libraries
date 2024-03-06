namespace Ironwall.Framework.Models.Mappers
{
    public interface IMetaEventMapper : IEventMapperBase
    {
        string EventGroup { get; set; }
        int MessageType { get; set; }
        int Status { get; set; }
        string Device { get; set; }
    }
}