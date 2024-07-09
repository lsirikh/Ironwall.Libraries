using Ironwall.Framework.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Devices
{
    public class CameraDeviceViewModel
         : BaseDeviceViewModel, ICameraDeviceViewModel
    {
        #region - Ctors -
        public CameraDeviceViewModel()
        {

        }

        public CameraDeviceViewModel(ICameraDeviceModel model)
            : base(model)
        {
            IpAddress = model.IpAddress;
            Port = model.Port;
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
        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
