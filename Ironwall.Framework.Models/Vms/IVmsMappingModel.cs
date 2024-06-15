namespace Ironwall.Framework.Models.Vms
{
    public interface IVmsMappingModel: IBasicModel
    {
        int EventId { get; set; }
        int GroupNumber { get; set; }
    }
}