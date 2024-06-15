namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    public interface IVmsBaseViewModel<T>
    {
        int Id { get; set; }
        T Model { get; set; }
    }
}