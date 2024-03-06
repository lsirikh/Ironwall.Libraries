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
        Created On   : 8/4/2023 1:07:28 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingSaveResponseModel : CameraMappingInfoResponseModel, ICameraMappingSaveResponseModel
    {

        #region - Ctors -
        public CameraMappingSaveResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_SAVE_RESPONSE;
        }

        public CameraMappingSaveResponseModel(bool success, string content, MappingInfoModel detail)
            : base(success, content, detail)
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_SAVE_RESPONSE;
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
