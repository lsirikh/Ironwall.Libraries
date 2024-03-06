namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IContactEventViewModel : IMetaEventViewModel
    {
        int ContactNumber { get; set; }
        int ContactSignal { get; set; }
        int ReadWrite { get; set; }
    }
}