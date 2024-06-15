using Ironwall.Framework.Models.Messages;
using Ironwall.Libraries.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public interface IScreenViewModel
    {
        CategoryEnum ClassCategory { get; set; }
        string ClassContent { get; set; }
        int ClassId { get; set; }
        string ClassName { get; set; }

        Task HandleAsync(CloseAllMessageModel message, CancellationToken cancellationToken);
    }
}