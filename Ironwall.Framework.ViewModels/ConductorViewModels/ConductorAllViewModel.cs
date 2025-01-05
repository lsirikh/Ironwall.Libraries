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

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public abstract class ConductorAllViewModel
        : Conductor<Screen>.Collection.AllActive
        , IConductorViewModel
        , IHandle<CloseAllMessageModel>

    {
        #region - Ctors -
        public ConductorAllViewModel()
        {
            ClassName = this.GetType().Name.ToString();
        }
        #endregion

        #region - Override Methods -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            _eventAggregator?.SubscribeOnUIThread(this);
            Debug.WriteLine($"## {ClassName} OnActivate!! ##");

            IsVisible = false;
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            _eventAggregator?.Unsubscribe(this);
            Debug.WriteLine($"## {ClassName} OnDeactivate!! ##");
            IsVisible = false;
            return Task.CompletedTask;
        }

        public override Task ActivateItemAsync(Screen item, CancellationToken cancellationToken = default)
        {
            /// BaseViewModel을 상속받는  
            /// ViewModel만 ActivateItem이 가능
            if (!(item is IConductorViewModel))
                return null;

            base.ActivateItemAsync(item, cancellationToken);

            /// 해당 ShellViewModel을 Visible 하게 
            /// 관리하기 위해서 Dialog와 Popup Dialog의
            /// ShellViewModel의 ActiveItem이 Blank Item 인지
            /// 확인하는 과정이 필요하다.
            var viewModel = item as IConductorViewModel;

            viewModel.IsVisible = false;

            return Task.CompletedTask;
        }
        
        #endregion

        #region - Handles - 
        public Task HandleAsync(CloseAllMessageModel message, CancellationToken cancellationToken)
        {
            ActivateItemAsync(Items?.FirstOrDefault<Screen>(), cancellationToken);
            IsVisible = false;

            return Task.CompletedTask;
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
            get { return _className; }
            set
            {
                _className = value;
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
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }
        #endregion

        private int _classId;
        private string _className;
        private string _classContent;
        private CategoryEnum _classCategory;
        private bool _isVisible;
        protected IEventAggregator _eventAggregator;
    }
}
