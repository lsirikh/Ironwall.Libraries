using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Events
{
    public class ConnectionEventViewModel
        : MetaEventViewModel
        , IConnectionEventViewModel
    {
        #region - Ctors -
        public ConnectionEventViewModel()
        {

        }

        public ConnectionEventViewModel(IConnectionEventModel model)
            : base(model)
        {
            try
            {
                switch ((EnumDeviceType)model.Device.DeviceType)
                {
                    case EnumDeviceType.NONE:
                        break;
                    case EnumDeviceType.Controller:
                        {
                            Device = ViewModelFactory.Build<ControllerDeviceViewModel>(model.Device as IControllerDeviceModel);
                        }
                        break;
                    case EnumDeviceType.Multi:
                    case EnumDeviceType.Fence:
                    case EnumDeviceType.Underground:
                    case EnumDeviceType.Contact:
                    case EnumDeviceType.PIR:
                    case EnumDeviceType.IoController:
                    case EnumDeviceType.Laser:
                        {
                            Device = ViewModelFactory.Build<SensorDeviceViewModel>(model.Device as ISensorDeviceModel);
                        }
                        break;
                    case EnumDeviceType.IpCamera:
                        break;
                    case EnumDeviceType.Cable:
                        break;
                    default:
                        break;
                }
                
            }
            catch (Exception)
            {

                throw;
            }
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
