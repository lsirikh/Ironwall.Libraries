namespace Ironwall.Framework.Models.Communications.Events
{

    public interface IActionRequestModel : IBaseEventMessageModel
    {
        string Content { get; set; }
        string EventId { get; set; }
        string User { get; set; }
    }
}