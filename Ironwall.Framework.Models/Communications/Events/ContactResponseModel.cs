using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ContactResponseModel
         : ResponseModel, IContactResponseModel
    {
        public ContactResponseModel()
        {
            
        }

        public ContactResponseModel(bool success, string msg, IContactRequestModel model)
            : base(success, msg)
        {
            if (model?.Detail?.ContactSignal > 0)
                Command = EnumCmdType.EVENT_CONTACT_ON_RESPONSE;
            else
                Command = EnumCmdType.EVENT_CONTACT_OFF_RESPONSE;

            RequestModel = model as ContactRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public ContactRequestModel RequestModel { get; set; }
    }
}
