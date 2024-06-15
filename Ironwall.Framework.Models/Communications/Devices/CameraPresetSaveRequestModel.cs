using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/11/2023 10:24:39 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetSaveRequestModel : UserSessionBaseRequestModel, ICameraPresetSaveRequestModel
    {

        #region - Ctors -
        public CameraPresetSaveRequestModel()
        {
            Command = EnumCmdType.CAMERA_PRESET_SAVE_REQUEST;
        }

        public CameraPresetSaveRequestModel(ILoginSessionModel model, List<CameraPresetModel> presets)
         : base(model)
        {
            Command = EnumCmdType.CAMERA_PRESET_SAVE_REQUEST;
            Presets = presets;
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
        [JsonProperty("preset_list", Order = 5)]
        public List<CameraPresetModel> Presets { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
