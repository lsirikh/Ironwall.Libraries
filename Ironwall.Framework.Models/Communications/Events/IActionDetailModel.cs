namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IActionDetailModel
    {
        string Content { get; set; }
        string FromEventId { get; set; }
        string Id { get; set; }
        string IdUser { get; set; }
    }
}