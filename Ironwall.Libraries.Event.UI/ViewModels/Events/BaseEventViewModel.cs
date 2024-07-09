using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 3:36:50 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public abstract class BaseEventViewModel<T> : BaseCustomViewModel<T>, IBaseEventViewModel<T> where T : IBaseEventModel
    {
        #region - Ctors -
        public BaseEventViewModel()
        {
        }

        public BaseEventViewModel(T model) : base(model)
        {
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
        #endregion
    }
}