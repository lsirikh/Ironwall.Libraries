namespace Ironwall.Framework.Models.Mappers
{
    public interface IEventMapperBase
    {
        string EventId { get; set; }
        
        string DateTime { get; set; }
    }
}