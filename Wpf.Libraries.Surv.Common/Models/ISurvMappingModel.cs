namespace Wpf.Libraries.Surv.Common.Models
{
    public interface ISurvMappingModel : ISurvBaseModel
    {
        int GroupNumber { get; set; }
        string GroupName { get; set; }
        int EventId { get; set; }
    }
}