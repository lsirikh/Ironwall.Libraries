namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IModeWindyResponseModel : IResponseModel
    {
        ModeWindyRequestModel RequestModel { get; set; }
    }
}