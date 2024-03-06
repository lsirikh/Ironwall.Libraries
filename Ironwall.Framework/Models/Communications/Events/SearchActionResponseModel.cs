using Ironwall.Libraries.Enums;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/21/2023 10:27:08 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SearchActionResponseModel : ResponseModel, ISearchActionReponseModel
    {

        #region - Ctors -
        public SearchActionResponseModel()
        {
            Command = (int)EnumCmdType.SEARCH_EVENT_ACTION_RESPONSE;
        }

        public SearchActionResponseModel(bool success, string msg
            , List<DetectionRequestModel> detectionEvents
            , List<MalfunctionRequestModel> malfunctionEvents 
            , List<ActionRequestModel> actionEvents
            )
             : base(success, msg)
        {
            Command = (int)EnumCmdType.SEARCH_EVENT_ACTION_RESPONSE;
            DetectionEvents = detectionEvents;
            MalfunctionEvents = malfunctionEvents;
            ActionEvents = actionEvents;
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
        public List<DetectionRequestModel> DetectionEvents { get; set; }
        public List<MalfunctionRequestModel> MalfunctionEvents { get; set; }
        public List<ActionRequestModel> ActionEvents { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
