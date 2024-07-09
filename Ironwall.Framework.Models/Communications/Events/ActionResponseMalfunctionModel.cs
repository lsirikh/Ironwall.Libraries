using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/25/2024 5:43:00 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ActionResponseMalfunctionModel
        : ActionBaseResponseModel<MalfunctionEventModel>, IActionResponseMalfunctionModel
    {

        public ActionResponseMalfunctionModel()
        {
            
        }

        public ActionResponseMalfunctionModel(bool success, string msg, IMalfunctionEventModel model = default) : base(success, msg, model as MalfunctionEventModel)
        {
        }
    }
}