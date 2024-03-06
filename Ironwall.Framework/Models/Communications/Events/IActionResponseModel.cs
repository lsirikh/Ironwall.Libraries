namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IActionResponseModel : IResponseModel
    {
        ActionRequestModel RequestModel { get; set; }
    }
}