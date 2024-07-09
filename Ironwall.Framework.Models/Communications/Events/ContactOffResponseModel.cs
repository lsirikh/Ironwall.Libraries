using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/25/2024 10:36:59 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ContactOffResponseModel : ResponseModel, IContactOffResponseModel
    {
        public ContactOffResponseModel()
        {

        }

        public ContactOffResponseModel(bool success, string msg, IContactOffRequestModel model)
            : base(EnumCmdType.EVENT_CONTACT_OFF_RESPONSE, success, msg)
        {
            RequestModel = model as ContactOffRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public ContactOffRequestModel RequestModel { get; set; }
    }
}