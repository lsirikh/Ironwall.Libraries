using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/26/2023 4:12:02 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class BaseOptionViewModel<T> : BaseCustomViewModel<T>, IBaseOptionViewModel<T> where T : IOptionBaseModel
    {

        #region - Ctors -
        public BaseOptionViewModel()
        {

        }

        public BaseOptionViewModel(T model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _model = model;
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
        public string ReferenceId
        {
            get { return _model.ReferenceId; }
            set
            {
                _model.ReferenceId = value;
                NotifyOfPropertyChange(() => ReferenceId);
            }
        }
        

        #endregion
        #region - Attributes -
        #endregion
    }
}
