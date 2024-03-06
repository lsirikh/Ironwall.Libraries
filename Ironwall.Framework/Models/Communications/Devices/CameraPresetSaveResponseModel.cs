using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/11/2023 10:24:49 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetSaveResponseModel : ResponseModel, ICameraPresetSaveResponseModel
    {

        #region - Ctors -
        public CameraPresetSaveResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_SAVE_RESPONSE;
        }

        public CameraPresetSaveResponseModel(bool success, string content)
            : base(success, content)
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_SAVE_RESPONSE;
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
