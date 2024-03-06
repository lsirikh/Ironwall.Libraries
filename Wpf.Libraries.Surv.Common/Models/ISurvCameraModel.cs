namespace Wpf.Libraries.Surv.Common.Models
{
    public interface ISurvCameraModel : ISurvBaseModel
    {
        string DeviceName { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int Mode { get; set; }
        string RtspUrl { get; set; }
    }
}