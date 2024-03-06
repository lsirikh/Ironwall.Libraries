using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public abstract class BaseEventViewModel<T> : Screen, IBaseEventViewModel<T> where T : IBaseEventModel
    {
        #region - Ctors -
        public BaseEventViewModel()
        {

        }
        public BaseEventViewModel(T model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _model = model;
        }
        #endregion
        #region - Implementation of Interface -
        public virtual void UpdateModel(T model)
        {
            _model = model;
            Refresh();
        }
        #endregion
        #region - Overrides -
        public abstract void Dispose();
        public virtual void OnLoaded(object sender, SizeChangedEventArgs e) { }
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator?.SubscribeOnUIThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator?.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public DateTime DateTime
        {
            get { return _model.DateTime; }
            set
            {
                _model.DateTime = value;
                NotifyOfPropertyChange(() => DateTime);
            }
        }

        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        protected T _model;
        #endregion
    }
}