namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IContactOnResponseModel : IResponseModel
    {
        ContactOnRequestModel RequestModel { get; set; }
    }
}