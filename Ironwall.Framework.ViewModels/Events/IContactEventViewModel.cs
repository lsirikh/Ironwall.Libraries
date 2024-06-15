namespace Ironwall.Framework.ViewModels.Events
{
    public interface IContactEventViewModel : IMetaEventViewModel
    {
        int ContactNumber { get; set; }
        int ContactSignal { get; set; }
        int ReadWrite { get; set; }
    }
}