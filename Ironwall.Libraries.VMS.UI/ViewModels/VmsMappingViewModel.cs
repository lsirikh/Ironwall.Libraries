using Ironwall.Framework.Models.Vms;
using System;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 4:29:48 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsMappingViewModel : VmsBaseViewModel<IVmsMappingModel>
    {
        #region - Ctors -
        public VmsMappingViewModel(IVmsMappingModel model) : base(model)
        {
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
        public int GroupNumber
        {
            get { return _model.GroupNumber; }
            set 
            { 
                _model.GroupNumber = value;
                NotifyOfPropertyChange(() => GroupNumber);
            }
        }

        public int EventId
        {
            get { return _model.EventId; }
            set
            {
                _model.EventId = value;
                NotifyOfPropertyChange(() => EventId);
            }
        }
        #endregion
        #region - Attributes -
        #endregion

    }
}