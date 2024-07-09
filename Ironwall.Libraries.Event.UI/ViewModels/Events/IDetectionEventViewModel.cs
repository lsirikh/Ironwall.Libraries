namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    interface IDetectionEventViewModel : IMetaEventViewModel
    {
        int Result { get; set; }
        
    }
}