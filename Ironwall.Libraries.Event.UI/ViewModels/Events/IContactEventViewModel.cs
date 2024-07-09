namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    interface IContactEventViewModel : IMetaEventViewModel
    {
        int ContactNumber { get; set; }
        int ContactSignal { get; set; }
        int ReadWrite { get; set; }
    }
}