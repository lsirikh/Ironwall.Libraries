using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 10:28:57 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ActionRequestDetectionModel : ActionBaseRequestModel<DetectionEventModel>, IActionRequestDetectionModel
    {
        public ActionRequestDetectionModel()
        {
            Command = EnumCmdType.EVENT_ACTION_DETECTION_REQUEST;
        }

        public ActionRequestDetectionModel(string content, string user, IDetectionEventModel model)
            : base(EnumCmdType.EVENT_ACTION_DETECTION_REQUEST, content, user, model as DetectionEventModel)
        {
        }

        public ActionRequestDetectionModel(IActionEventModel model) : base(EnumCmdType.EVENT_ACTION_DETECTION_REQUEST, model)
        {
        }
    }
}