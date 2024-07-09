using Ironwall.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public interface IEventViewModel
    {
        #region - Interfaces -
        Task ExecuteAsync(CancellationToken tokenSourceEvent = default, CancellationTokenSource tokenSourceOuter = default);
        IEventModel EventModel { get; }
        CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        #endregion
    }
}