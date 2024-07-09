using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Devices.Providers.Models;

namespace Ironwall.Libraries.Devices.Providers
{
    public class SensorDeviceProvider : WrapperDeviceProvider<SensorDeviceModel> 
    {
        #region - Ctors -
        public SensorDeviceProvider(DeviceProvider provider) : base(provider)
        {
            ClassName = nameof(SensorDeviceProvider);
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
