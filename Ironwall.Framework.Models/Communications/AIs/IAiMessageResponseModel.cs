namespace Ironwall.Framework.Models.Communications.AIs
{
    public interface IAiMessageResponseModel : IResponseModel
    {
        string Response { get; set; }
    }
}