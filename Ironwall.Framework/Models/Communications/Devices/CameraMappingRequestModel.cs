using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/4/2023 3:41:53 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingRequestModel : UserSessionBaseRequestModel, ICameraMappingRequestModel
    {

        #region - Ctors -
        public CameraMappingRequestModel()
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_DATA_REQUEST;
        }

        public CameraMappingRequestModel(ILoginSessionModel model)
         : base(model)
        {
            Command = (int)EnumCmdType.CAMERA_MAPPING_DATA_REQUEST;
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
