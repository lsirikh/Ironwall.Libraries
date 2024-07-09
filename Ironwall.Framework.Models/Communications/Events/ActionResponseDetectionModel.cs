using Ironwall.Framework.Models.Events;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 10:30:29 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ActionResponseDetectionModel : ActionBaseResponseModel<DetectionEventModel>, IActionResponseDetectionModel
    {
        public ActionResponseDetectionModel()
        {

        }

        public ActionResponseDetectionModel(bool success, string msg, IDetectionEventModel model = default) : base(success, msg, model as DetectionEventModel)
        {
        }
    }
}