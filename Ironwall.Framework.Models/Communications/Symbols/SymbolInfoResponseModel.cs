using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/15/2023 5:52:47 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolInfoResponseModel : ResponseModel, ISymbolInfoResponseModel
    {

        #region - Ctors -
        public SymbolInfoResponseModel()
        {
            Command = EnumCmdType.SYMBOL_DATA_INFO_RESPONSE;
        }

        public SymbolInfoResponseModel(bool success, string content, ISymbolDetailModel detail)
            : base(success, content)
        {
            Command = EnumCmdType.SYMBOL_DATA_INFO_RESPONSE;
            Detail = ResponseFactory.Build<SymbolDetailModel>(detail);
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

        [JsonProperty("detail", Order = 4)]
        public SymbolDetailModel Detail { get; private set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
