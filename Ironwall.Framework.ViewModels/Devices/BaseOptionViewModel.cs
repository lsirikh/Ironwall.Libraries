using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using System;

namespace Ironwall.Framework.ViewModels.Devices
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/17/2024 7:26:42 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class BaseOptionViewModel : Screen, IBaseOptionViewModel
    {
        #region - Ctors -
        public BaseOptionViewModel()
        {

        }

        public BaseOptionViewModel(IBaseOptionModel model)
        {
            Id = model.Id;
            ReferenceId = model.ReferenceId;
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
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private int _referenceId;

        public int ReferenceId
        {
            get { return _referenceId; }
            set
            {
                _referenceId = value;
                NotifyOfPropertyChange(() => ReferenceId);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}