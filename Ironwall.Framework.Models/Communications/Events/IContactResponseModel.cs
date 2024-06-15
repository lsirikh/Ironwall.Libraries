namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IContactResponseModel : IResponseModel
    {
        ContactRequestModel RequestModel { get; set; }
    }
}