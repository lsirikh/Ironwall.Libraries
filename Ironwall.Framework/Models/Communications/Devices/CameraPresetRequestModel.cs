using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/10/2023 3:26:01 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetRequestModel : UserSessionBaseRequestModel, ICameraPresetRequestModel
    {

        #region - Ctors -
        public CameraPresetRequestModel()
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_DATA_REQUEST;
        }

        public CameraPresetRequestModel(ILoginSessionModel model)
         : base(model)
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_DATA_REQUEST;
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
