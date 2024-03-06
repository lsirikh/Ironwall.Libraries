namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IDetectionEventViewModel : IMetaEventViewModel
    {
        int Result { get; set; }
        
    }
}