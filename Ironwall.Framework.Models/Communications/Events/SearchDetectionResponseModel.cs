using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/19/2023 3:41:39 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SearchDetectionResponseModel : ResponseModel, ISearchDetectionResponseModel
    {

        #region - Ctors -
        public SearchDetectionResponseModel()
        {
            Command = EnumCmdType.SEARCH_EVENT_DETECTION_RESPONSE;
        }
        public SearchDetectionResponseModel(bool success, string msg, List<DetectionRequestModel> events)
             : base(success, msg)
        {
            Command = EnumCmdType.SEARCH_EVENT_DETECTION_RESPONSE;
            Events = events;
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
        [JsonProperty("detection_events", Order = 4)]
        public List<DetectionRequestModel> Events { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
