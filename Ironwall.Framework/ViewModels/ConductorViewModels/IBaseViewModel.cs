
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public interface IBaseViewModel
    {
        int ClassId { get; set; }
        string ClassName { get; set; }
        string ClassContent { get; set; }
        CategoryEnum ClassCategory { get; set; }
    }
}