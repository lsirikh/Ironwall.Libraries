using Caliburn.Micro;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public abstract class BaseViewModel : Conductor<IScreen>
                                    , IHandle<CloseAllMessageModel>
                                    , IBaseViewModel
    {
        #region - Ctors -
        public BaseViewModel()
        {
            _log = IoC.Get<ILogService>();
            ClassName = this.GetType().Name.ToString();
            _class = this.GetType();
        }

        public BaseViewModel(IEventAggregator eventAggregator, ILogService log)
        {
            ClassName = this.GetType().Name.ToString();
            _eventAggregator = eventAggregator;
            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            try
            {
                base.OnActivateAsync(cancellationToken);
                _log.Info($"## {ClassName} OnActivate!! ##");
                _eventAggregator?.SubscribeOnUIThread(this);
                _cancellationTokenSource = new CancellationTokenSource();
            }
            catch (Exception)
            {
            }
            
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            try
            {
                base.OnDeactivateAsync(close, cancellationToken);
                _log.Info($"## {ClassName} OnDeactivate!! ##");
                _eventAggregator?.Unsubscribe(this);
                if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                GC.Collect();
            }
            catch (Exception)
            {
            }
            
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public Task HandleAsync(CloseAllMessageModel message, CancellationToken cancellationToken)
        {
            return TryCloseAsync();
        }
        #endregion
        #region - Properties -
        public int ClassId
        {
            get { return _classId; }
            set
            {
                _classId = value;
                NotifyOfPropertyChange(() => ClassId);
            }
        }


        public string ClassName
        {
            get { return _classTitle; }
            set
            {
                _classTitle = value;
                NotifyOfPropertyChange(() => ClassName);
            }
        }

        public string ClassContent
        {
            get { return _classContent; }
            set
            {
                _classContent = value;
                NotifyOfPropertyChange(() => ClassContent);
            }
        }
        public CategoryEnum ClassCategory
        {
            get { return _classCategory; }
            set
            {
                _classCategory = value;
                NotifyOfPropertyChange(() => ClassCategory);
            }
        }

        #endregion
        #region - Attributes -
        private int _classId;
        private string _classTitle;
        private string _classContent;
        private CategoryEnum _classCategory;
        protected IEventAggregator _eventAggregator;
        protected CancellationTokenSource _cancellationTokenSource;
        protected ILogService _log;
        protected Type _class;

        public const int ACTION_TOKEN_TIMEOUT = 5000;
        #endregion

    }
}
