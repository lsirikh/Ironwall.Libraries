using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/4/2023 10:45:47 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingSaveRequestModel : UserSessionBaseRequestModel, ICameraMappingSaveRequestModel
    {

        #region - Ctors -
        public CameraMappingSaveRequestModel()
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_SAVE_REQUEST;
        }

        public CameraMappingSaveRequestModel(ILoginSessionModel model, List<CameraMappingModel> mappings)
         : base(model)
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_SAVE_REQUEST;
            Mappings = mappings;
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
        [JsonProperty("mapping_list", Order = 5)]
        public List<CameraMappingModel> Mappings { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
