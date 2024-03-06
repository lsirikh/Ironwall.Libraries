using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Web.UI.WebControls;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/10/2023 8:55:45 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MapFileSaveResponseModel : ResponseModel, IMapFileSaveResponseModel
    {

        #region - Ctors -
        public MapFileSaveResponseModel()
        {
            Command = (int)EnumCmdType.MAP_FILE_SAVE_RESPONSE;
        }

        public MapFileSaveResponseModel(bool success, string content, MapDetailModel detail) : base(success, content)
        {
            Command = (int)EnumCmdType.MAP_FILE_SAVE_RESPONSE;
            Detail = detail;
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
        [JsonProperty("Detail", Order = 4)]
        public MapDetailModel Detail { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
