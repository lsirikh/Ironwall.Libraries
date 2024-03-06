namespace Ironwall.Framework.Models.Mappers
{
    public interface IDetectionEventMapper : IMetaEventMapper
    {
        int Result { get; set; }

    }
}