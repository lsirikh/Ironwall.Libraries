using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class EventViewModel<T> : BaseEventViewModel<T>, IEventViewModel<T> where T : IBaseEventModel
    {
        #region - Ctors -
        public EventViewModel(T eventModel) : base(eventModel)
        {
            _model = eventModel;
        }
        #endregion

        #region - Implementations for IEventViewModel -        
        public Task ExecuteAsync(CancellationToken tokenSourceEvent = default)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region - Properties for IEventViewModel -
        public CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        public string TagFault { get; set; }
        public T EventModel => _model;
        #endregion
        #region - Attriibtes -
        #endregion
    }
}
