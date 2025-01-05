using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/3/2024 8:38:46 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsSensorViewModel : BaseCustomViewModel<IVmsSensorModel>, IVmsSensorViewModel
    {
        #region - Ctors -
        public VmsSensorViewModel(IVmsSensorModel model) : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new VmsSensorModel();
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

        public BaseDeviceModel Device
        {
            get { return _model.Device; }
            set
            {
                _model.Device = value;
                NotifyOfPropertyChange(() => Device);
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