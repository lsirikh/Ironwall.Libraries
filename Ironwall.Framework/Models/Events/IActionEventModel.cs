namespace Ironwall.Framework.Models.Events
{
    public interface IActionEventModel : IBaseEventModel
    {
        string Content { get; set; }
        IMetaEventModel FromEvent { get; set; }
        string User { get; set; }
    }
}