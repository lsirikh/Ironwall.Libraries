namespace Wpf.Libraries.Surv.Common.Models
{
    public interface ISurvEventModel : ISurvBaseModel
    {
        int Channel { get; set; }
        string EventName { get; set; }
        string IpAddress { get; set; }
        bool IsOn { get; set; }
        int EventId { get; set; }
        int ApiId { get; set; }
        int CameraId { get; set; }
    }
}