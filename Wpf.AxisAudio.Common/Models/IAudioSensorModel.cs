namespace Wpf.AxisAudio.Common.Models
{
    public interface IAudioSensorModel : IAudioBaseModel
    {
        AudioGroupBaseModel Group { get; set; }
        string DeviceName { get; set; }
        int ControllerId { get; set; }
        int SensorId { get; set; }
        int DeviceType { get; set; }
    }
}