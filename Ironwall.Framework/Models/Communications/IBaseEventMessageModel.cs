namespace Ironwall.Framework.Models.Communications
{
    public interface IBaseEventMessageModel : IBaseMessageModel
    {
        string Id { get; set; }
        int Controller { get; set; }
        string Group { get; set; }
        int Sensor { get; set; }
        int UnitType { get; set; }
        int Status { get; set; }
        string DateTime { get; set; }

        //void Insert(string id, string group, int controller, int sensor, int uType, string dateTime);
    }
}