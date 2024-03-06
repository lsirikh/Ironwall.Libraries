using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/15/2023 5:52:32 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolResponseModel : ResponseModel, ISymbolResponseModel
    {

        #region - Ctors -
        public SymbolResponseModel()
        {
            Command = (int)EnumCmdType.SYMBOL_DATA_LOAD_RESPONSE;
            Symbols = new List<SymbolModel>();
        }

        public SymbolResponseModel(bool success, string content, List<SymbolModel> symbols)
            : base(success, content)
        {
            Command = (int)EnumCmdType.SYMBOL_DATA_LOAD_RESPONSE;
            Symbols = symbols;
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
        [JsonProperty("symbols", Order = 4)]
        public List<SymbolModel> Symbols { get; private set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
