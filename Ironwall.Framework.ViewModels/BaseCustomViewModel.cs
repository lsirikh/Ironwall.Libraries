using Caliburn.Micro;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace Ironwall.Framework.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/6/2023 9:16:32 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class BaseCustomViewModel<T> : SelectableBaseViewModel, IBaseCustomViewModel<T> where T : IBaseModel
    {

        #region - Ctors -
        public BaseCustomViewModel()
        {
        }
        public BaseCustomViewModel(T model)
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

        public T Model => _model;
     
        #endregion
        #region - Attributes -
        protected T _model;
        #endregion
    }
}
