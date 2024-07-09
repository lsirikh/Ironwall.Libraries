using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Providers
{
    public sealed class ControllerDeviceProvider : WrapperDeviceProvider<ControllerDeviceModel> 
    {
        #region - Ctors -
        public ControllerDeviceProvider(DeviceProvider provider): base(provider)
        {
            ClassName = nameof(ControllerDeviceProvider);
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
