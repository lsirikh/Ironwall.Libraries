﻿using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Sensorway.Accounts.Base.Models;
using System;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 9:46:28 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsApiLoginResponseModel : ResponseModel, IVmsApiLoginResponseModel
    {
        public VmsApiLoginResponseModel()
        {

        }

        public VmsApiLoginResponseModel(bool success, string msg, ILoginSessionModel model)
            : base(EnumCmdType.API_LOGIN_RESPONSE, success, msg)
        {
            Body = model as LoginSessionModel;
        }

        [JsonProperty("body", Order = 4)]
        public LoginSessionModel Body { get; set; }
    }
}