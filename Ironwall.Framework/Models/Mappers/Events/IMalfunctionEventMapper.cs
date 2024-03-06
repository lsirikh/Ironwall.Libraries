namespace Ironwall.Framework.Models.Mappers
{
    public interface IMalfunctionEventMapper : IMetaEventMapper
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        int Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}