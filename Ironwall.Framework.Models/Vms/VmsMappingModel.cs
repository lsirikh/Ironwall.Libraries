using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Vms
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/11/2024 4:43:37 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsMappingModel : BasicModel, IVmsMappingModel
    {
        #region - Ctors -
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
        [JsonProperty("group_number", Order = 2)]
        public int GroupNumber { get; set; }
        [JsonProperty("event_id", Order = 3)]
        public int EventId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}