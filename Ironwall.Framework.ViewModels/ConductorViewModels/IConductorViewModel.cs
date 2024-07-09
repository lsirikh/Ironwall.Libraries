
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public interface IConductorViewModel : IBaseViewModel
    {
        bool IsVisible { get; set; }

        Task HandleAsync(CloseAllMessageModel message, CancellationToken cancellationToken);
    }
}