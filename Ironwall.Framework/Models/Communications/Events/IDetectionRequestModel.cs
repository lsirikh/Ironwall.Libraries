namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IDetectionRequestModel : IBaseEventMessageModel
    {
        DetectionDetailModel Detail { get; set; }

        //void Insert(string id, string group, int controller, int sensor, int uType, DetectionDetailModel detail, string dateTime);
    }
}