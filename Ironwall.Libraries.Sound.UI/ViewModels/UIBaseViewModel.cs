using Caliburn.Micro;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Sound.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/11/2023 8:50:15 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class UIBaseViewModel : Screen
    {

        #region - Ctors -
        public UIBaseViewModel()
        {
            ClassName = this.GetType().Name.ToString();
        }

        public UIBaseViewModel(IEventAggregator eventAggregator)
        {
            ClassName = this.GetType().Name.ToString();
            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            Debug.WriteLine($"######### {ClassName} OnActivate!! #########");
            _eventAggregator?.SubscribeOnPublishedThread(this);
            _cancellationTokenSource = new CancellationTokenSource();
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            Debug.WriteLine($"######### {ClassName} OnDeactivate!! #########");
            _eventAggregator?.Unsubscribe(this);
            _cancellationTokenSource.Dispose();
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
        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        protected CancellationTokenSource _cancellationTokenSource;

        public const int ACTION_TOKEN_TIMEOUT = 5000;

        private int _classId;
        private string _classTitle;
        #endregion
    }
}
