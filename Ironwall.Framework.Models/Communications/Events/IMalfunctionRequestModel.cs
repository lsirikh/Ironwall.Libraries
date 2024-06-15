namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IMalfunctionRequestModel: IBaseEventMessageModel
    {
        MalfunctionDetailModel Detail { get; set; }
    }
}