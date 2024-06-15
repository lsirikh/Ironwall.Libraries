using Caliburn.Micro;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels;
using System;
using System.Runtime.InteropServices;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 4:15:26 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public abstract class VmsBaseViewModel<T> : SelectableBaseViewModel, IVmsBaseViewModel<T> where T : IBasicModel
    {
        #region - Ctors -
        protected VmsBaseViewModel(T model)
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