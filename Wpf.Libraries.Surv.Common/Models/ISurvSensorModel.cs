namespace Wpf.Libraries.Surv.Common.Models
{
    public interface ISurvSensorModel : ISurvBaseModel
    {
        string GroupName { get; set; }
        string DeviceName { get; set; }
        int ControllerId { get; set; }
        int SensorId { get; set; }
        //int Channel { get; set; }
        int DeviceType { get; set; }
    }
}