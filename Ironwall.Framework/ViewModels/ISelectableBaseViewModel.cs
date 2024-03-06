using Ironwall.Framework.ViewModels.ConductorViewModels;

namespace Ironwall.Framework.ViewModels
{
    public interface ISelectableBaseViewModel : IBaseViewModel
    {
        bool IsSelected { get; set; }
    }
}