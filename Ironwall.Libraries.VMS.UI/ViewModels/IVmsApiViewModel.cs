using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    public interface IVmsApiViewModel : IBaseCustomViewModel<IVmsApiModel>
    {
        string ApiAddress { get; set; }
        uint ApiPort { get; set; }
        string Password { get; set; }
        string Username { get; set; }
    }
}