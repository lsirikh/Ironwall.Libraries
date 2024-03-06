namespace Ironwall.Framework.ViewModels.Events
{
    public interface IDetectionEventViewModel : IMetaEventViewModel
    {
        int Result { get; set; }
        
    }
}