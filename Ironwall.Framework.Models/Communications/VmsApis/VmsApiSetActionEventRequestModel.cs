﻿using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Sensorway.Events.Base.Models;
using System;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 9:48:51 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsApiSetActionEventRequestModel : BaseMessageModel, IVmsApiSetActionEventRequestModel
    {
        public VmsApiSetActionEventRequestModel()
        {
        }


        public VmsApiSetActionEventRequestModel(IActionEventModel model)
            : base(EnumCmdType.API_ACT_EVENT_REQUEST)
        {
            Body = model as ActionEventModel;
        }

        [JsonProperty("body", Order = 2)]
        public ActionEventModel Body { get; set; }
    }
}