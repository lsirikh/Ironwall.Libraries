namespace Wpf.Libraries.Surv.UI.ViewModels
{
    public interface ISurvBaseViewModel<T>
    {
        int Id { get; set; }
        T Model { get; set; }
    }
}