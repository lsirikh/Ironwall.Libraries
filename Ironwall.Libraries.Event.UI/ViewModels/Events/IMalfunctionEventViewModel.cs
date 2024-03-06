namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IMalfunctionEventViewModel : IMetaEventViewModel
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        int Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}