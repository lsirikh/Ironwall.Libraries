using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/16/2023 5:22:24 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class ScreenViewModel : Screen
        , IHandle<CloseAllMessageModel>, IScreenViewModel
    {

        #region - Ctors -
        public ScreenViewModel()
        {
            ClassName = this.GetType().Name.ToString();
        }

        public ScreenViewModel(IEventAggregator eventAggregator)
        {
            ClassName = this.GetType().Name.ToString();
            _eventAggregator = eventAggregator;
        }

        #endregion
        #region - Implementation of Interface -
        public Task HandleAsync(CloseAllMessageModel message, CancellationToken cancellationToken)
        {
            return TryCloseAsync();
        }
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            Debug.WriteLine($"######### {ClassName} OnActivate!! #########");
            _eventAggregator?.SubscribeOnUIThread(this);
            _cancellationTokenSource = new CancellationTokenSource();
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            Debug.WriteLine($"######### {ClassName} OnDeactivate!! #########");
            _eventAggregator?.Unsubscribe(this);
            _cancellationTokenSource?.Dispose();
            GC.Collect();
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
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

        public const int ACTION_TOKEN_TIMEOUT = 5000;
        #endregion
    }
}
