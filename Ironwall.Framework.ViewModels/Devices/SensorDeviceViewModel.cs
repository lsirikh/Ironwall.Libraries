using Ironwall.Framework.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Devices
{
    public class SensorDeviceViewModel
        : BaseDeviceViewModel, ISensorDeviceViewModel
    {
        #region - Ctors -
        public SensorDeviceViewModel()
        {

        }
        public SensorDeviceViewModel(ISensorDeviceModel model)
            : base(model)
        {
            if(model.Controller != null)
                Controller = ViewModelFactory.Build<ControllerDeviceViewModel>(model.Controller);
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
        private IControllerDeviceViewModel _controller;
        public IControllerDeviceViewModel Controller
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
        #endregion
    }
}
