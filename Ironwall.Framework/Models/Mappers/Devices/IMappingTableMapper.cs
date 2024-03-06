namespace Ironwall.Framework.Models.Mappers
{
    public interface IMappingTableMapper
    {
        string FirstPreset { get; set; }
        string GroupId { get; set; }
        string MapperId { get; set; }
        string SecondPreset { get; set; }
        string Sensor { get; set; }
    }
}