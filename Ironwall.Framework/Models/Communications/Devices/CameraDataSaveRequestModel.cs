using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/9/2023 2:34:48 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraDataSaveRequestModel : UserSessionBaseRequestModel, ICameraDataSaveRequestModel
    {

        #region - Ctors -
        public CameraDataSaveRequestModel()
        {
            Command = (int)EnumCmdType.CAMERA_DATA_SAVE_REQUEST;
        }

        public CameraDataSaveRequestModel(ILoginSessionModel model, List<CameraDeviceModel> cameras)
         : base(model)
        {
            Command = (int)EnumCmdType.CAMERA_DATA_SAVE_REQUEST;
            Cameras = cameras;
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
        [JsonProperty("camera_list", Order = 5)]
        public List<CameraDeviceModel> Cameras { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
