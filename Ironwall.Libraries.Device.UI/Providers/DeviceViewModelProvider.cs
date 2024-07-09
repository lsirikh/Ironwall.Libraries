using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ironwall.Libraries.Device.UI.ViewModels;
using Caliburn.Micro;
using System.Threading;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:50:03 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class DeviceViewModelProvider : WrapperDeviceViewModelProvider<IBaseDeviceModel, DeviceViewModel>
    {
        #region - Ctors -
        public DeviceViewModelProvider(DeviceProvider provider) : base(provider)
        {
            ClassName = nameof(DeviceViewModelProvider);
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
