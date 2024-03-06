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
        Created On   : 8/10/2023 3:26:23 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetResponseModel : ResponseModel, ICameraPresetResponseModel
    {

        #region - Ctors -
        public CameraPresetResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_DATA_RESPONSE;
        }

        public CameraPresetResponseModel(bool success, string content, List<CameraPresetModel> list)
            : base(success, content)
        {
            Command = (int)EnumCmdType.CAMERA_PRESET_DATA_RESPONSE;
            List = list;
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
        [JsonProperty("preset_list", Order = 4)]
        public List<CameraPresetModel> List { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
