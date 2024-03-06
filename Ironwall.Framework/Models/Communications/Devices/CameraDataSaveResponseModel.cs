using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/9/2023 2:42:23 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraDataSaveResponseModel : DeviceInfoResponseModel, ICameraDataSaveResponseModel
    {

        #region - Ctors -
        public CameraDataSaveResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_DATA_SAVE_RESPONSE;
        }

        public CameraDataSaveResponseModel(bool success, string content, IDeviceDetailModel detail)
            : base(success, content, detail)
        {
            Command = (int)EnumCmdType.CAMERA_DATA_SAVE_RESPONSE;
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
