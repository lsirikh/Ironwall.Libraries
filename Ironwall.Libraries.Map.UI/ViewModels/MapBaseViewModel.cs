using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    public abstract class MapBaseViewModel
        : Screen, IMapBaseViewModel
    {
        #region - Ctors -
        public MapBaseViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        public MapBaseViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion
        #region - Implementation of Interface -
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
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        private bool isSelected;
        #endregion
    }
}
