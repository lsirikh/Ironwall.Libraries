namespace Ironwall.Framework.Models.Mappers
{
    public interface IReportEventMapper : IEventMapperBase
    {
        string FromEventId { get; set; }
        string Content { get; set; }
        string User { get; set; }
    }
}