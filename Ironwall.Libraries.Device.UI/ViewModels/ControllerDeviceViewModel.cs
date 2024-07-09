using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using System.Net;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:20:01 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ControllerDeviceViewModel : DeviceViewModel, IControllerDeviceViewModel
    {

        #region - Ctors -
        public ControllerDeviceViewModel()
        {
            _model = new ControllerDeviceModel();
        }

        public ControllerDeviceViewModel(IControllerDeviceModel model) : base(model)
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
        public string IpAddress
        {
            get { return (_model as IControllerDeviceModel).IpAddress; }
            set
            {
                (_model as IControllerDeviceModel).IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get { return (_model as IControllerDeviceModel).Port; }
            set
            {
                (_model as IControllerDeviceModel).Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
