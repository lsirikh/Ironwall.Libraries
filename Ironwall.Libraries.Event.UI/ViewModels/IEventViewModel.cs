using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IEventViewModel<T> : IBaseEventViewModel<T> where T : IBaseEventModel
    {
        #region - Interfaces -
        Task ExecuteAsync(CancellationToken tokenSourceEvent);
        T EventModel { get; }
        CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        #endregion
    }
}
