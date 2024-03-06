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
        Created On   : 7/4/2023 3:46:43 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingResponseModel : ResponseModel, ICameraMappingResponseModel
    {

        #region - Ctors -
        public CameraMappingResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_DATA_RESPONSE;
        }

        public CameraMappingResponseModel(bool success, string content, List<CameraMappingModel> list)
            : base(success, content)
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_DATA_RESPONSE;
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
        [JsonProperty("mapping_list", Order = 4)]
        public List<CameraMappingModel> List { get; private set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
