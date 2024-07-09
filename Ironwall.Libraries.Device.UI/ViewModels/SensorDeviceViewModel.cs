using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels.Devices;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:27:12 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SensorDeviceViewModel : DeviceViewModel, ISensorDeviceViewModel
    {

        #region - Ctors -
        public SensorDeviceViewModel()
        {
            _model = new SensorDeviceModel();
        }

        public SensorDeviceViewModel(ISensorDeviceModel model) : base(model)
        {
            //Controller = ViewModelFactory.Build<ControllerDeviceViewModel>(model.Controller);
            Controller = new ControllerDeviceViewModel(model.Controller);
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

        public ControllerDeviceViewModel Controller
        {
            get { return _controller; }
            set 
            { 
                _controller = value;
                NotifyOfPropertyChange(() => Controller);
            }
        }

        #endregion
        #region - Attributes -
        private ControllerDeviceViewModel _controller;
        #endregion
    }
}
