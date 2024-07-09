using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Maps.Symbols;
using System;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/8/2023 6:11:42 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class DeviceViewModel : BaseDeviceViewModel<IBaseDeviceModel>, IDeviceViewModel
    {

        #region - Ctors -
        public DeviceViewModel()
        {
            _model = new BaseDeviceModel();
        }
        public DeviceViewModel(IBaseDeviceModel model) : base(model)
        {
            _model = model;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new BaseDeviceModel();
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
