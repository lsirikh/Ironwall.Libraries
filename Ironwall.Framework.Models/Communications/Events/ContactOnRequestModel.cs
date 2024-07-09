using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Ironwall.Redis.Message.Framework;
using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/25/2024 10:35:37 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ContactOnRequestModel : BaseEventMessageModel, IContactOnRequestModel
    {
        public ContactOnRequestModel()
        {
        }


        public ContactOnRequestModel(IContactEventModel model)
            : base(EnumCmdType.EVENT_CONTACT_ON_REQUEST)
        {
            Body = model as ContactEventModel;
        }

        [JsonProperty("body", Order = 6)]
        public ContactEventModel Body { get; set; }
    }
}