using Caliburn.Micro;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 5:03:04 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public abstract class ExBaseEventViewModel<T> : Screen, IExBaseEventViewModel<T> where T : IBaseEventModel
    {
        #region - Ctors -
        protected ExBaseEventViewModel()
        {
            _class = this.GetType();
        }
        public ExBaseEventViewModel(T model) : base()
        {
            _log = IoC.Get<ILogService>();
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
        public int Id
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
        protected ILogService _log;
        protected Type _class;
        #endregion
    }
}