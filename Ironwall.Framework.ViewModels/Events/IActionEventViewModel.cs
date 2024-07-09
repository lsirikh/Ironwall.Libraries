namespace Ironwall.Framework.ViewModels.Events
{
    public interface IActionEventViewModel : IBaseEventViewModel
    {
        string Content { get; set; }
        IMetaEventViewModel FromEvent { get; set; }
        string User { get; set; }
    }
}