using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 1:28:10 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class SurvBaseViewModel<T> : SelectableBaseViewModel, ISurvBaseViewModel<T> 
        where T : ISurvBaseModel
    {

        #region - Ctors -
        public SurvBaseViewModel(T model)
        {
            _model = model;
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        #endregion
        #region - Implementation of Interface -
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
        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public T Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }
        #endregion
        #region - Attributes -
        protected T _model;
        #endregion
    }
}
