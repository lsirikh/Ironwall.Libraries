﻿using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Sensorway.Accounts.Base.Models;
using System;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 9:47:39 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsApiKeepAliveResponseModel : ResponseModel, IVmsApiKeepAliveResponseModel
    {
        public VmsApiKeepAliveResponseModel()
        {

        }

        public VmsApiKeepAliveResponseModel(bool success, string msg, ILoginSessionModel model)
            : base(EnumCmdType.API_KEEP_ALIVE_USER_RESPONSE, success, msg)
        {
            Body = model as LoginSessionModel;
        }

        [JsonProperty("body", Order = 4)]
        public LoginSessionModel Body { get; set; }
    }
}