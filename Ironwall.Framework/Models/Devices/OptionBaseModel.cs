using Newtonsoft.Json;
using StackExchange.Redis;

namespace Ironwall.Framework.Models.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/12/2023 10:48:48 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OptionBaseModel : BaseModel,  IOptionBaseModel
    {

        #region - Ctors -
        public OptionBaseModel()
        {

        }

        public OptionBaseModel(string id, string referId) : base(id)
        {
            ReferenceId = referId;
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
        [JsonProperty("reference_id", Order = 2)]
        public string ReferenceId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
