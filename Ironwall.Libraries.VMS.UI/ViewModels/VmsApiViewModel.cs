using Caliburn.Micro;
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
       Created On   : 6/12/2024 4:14:36 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsApiViewModel : BaseCustomViewModel<IVmsApiModel>, IVmsApiViewModel
    {
        #region - Ctors -
        public VmsApiViewModel(IVmsApiModel model) : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new VmsApiModel();
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
        public string ApiAddress
        {
            get { return _model.ApiAddress; }
            set
            {
                _model.ApiAddress = value;
                NotifyOfPropertyChange(() => ApiAddress);
            }
        }

        public uint ApiPort
        {
            get { return _model.ApiPort; }
            set
            {
                _model.ApiPort = value;
                NotifyOfPropertyChange(() => ApiPort);
            }
        }

        public string Username
        {
            get { return _model.Username; }
            set
            {
                _model.Username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        public string Password
        {
            get { return _model.Password; }
            set
            {
                _model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        #endregion
        #region - Attributes -
        #endregion

    }
}