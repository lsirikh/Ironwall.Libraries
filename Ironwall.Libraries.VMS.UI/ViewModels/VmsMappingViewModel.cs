using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;
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
    public class VmsMappingViewModel : BaseCustomViewModel<IVmsMappingModel>, IVmsMappingViewModel
    {
        #region - Ctors -
        public VmsMappingViewModel(IVmsMappingModel model) : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new VmsMappingModel();
            GC.Collect();
        }
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

        public EnumTrueFalse Status
        {
            get { return _model.Status; }
            set
            {
                _model.Status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }
        #endregion
        #region - Attributes -
        #endregion

    }
}