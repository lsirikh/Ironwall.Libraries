namespace Ironwall.Framework.Models.Devices
{
    public interface ICameraMappingModel : IBaseModel
    {
        string Group { get; set; }
        SensorDeviceModel Sensor { get; set; }
        CameraPresetModel FirstPreset { get; set; }
        CameraPresetModel SecondPreset { get; set; }
    }
}