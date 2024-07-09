using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/25/2024 10:36:24 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ContactOnResponseModel : ResponseModel, IContactOnResponseModel
    {
        public ContactOnResponseModel()
        {

        }

        public ContactOnResponseModel(bool success, string msg, IContactOnRequestModel model)
            : base(EnumCmdType.EVENT_CONTACT_ON_RESPONSE, success, msg)
        {
            RequestModel = model as ContactOnRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public ContactOnRequestModel RequestModel { get; set; }
    }
}