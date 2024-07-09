using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;

namespace Ironwall.Libraries.Devices.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/12/2023 10:44:51 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraProfileProvider : WrapperOptionProvider<CameraProfileModel>
    {
        #region - Ctors -
        public CameraProfileProvider(CameraOptionProvider provider) : base(provider)
        {
            ClassName = nameof(CameraProfileProvider);
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
