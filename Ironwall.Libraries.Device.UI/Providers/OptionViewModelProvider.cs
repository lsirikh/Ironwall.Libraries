using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Device.UI.ViewModels;
using System.Diagnostics;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Providers.Models;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/26/2023 4:49:13 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OptionViewModelProvider : WrapperOptionViewModelProvider<IBaseOptionModel, CameraOptionViewModel>
    {
        #region - Ctors -
        public OptionViewModelProvider(CameraOptionProvider provider) : base(provider)
        {
            ClassName = nameof(OptionViewModelProvider);
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
