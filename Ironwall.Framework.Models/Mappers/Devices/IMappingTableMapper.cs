namespace Ironwall.Framework.Models.Mappers
{
    public interface IMappingTableMapper : IBaseModel
    {
        string GroupId { get; set; }
        int Sensor { get; set; }
        int? FirstPreset { get; set; }
        int? SecondPreset { get; set; }
    }
}