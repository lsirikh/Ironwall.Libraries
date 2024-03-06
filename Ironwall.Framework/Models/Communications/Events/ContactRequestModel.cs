using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Ironwall.Redis.Message.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ContactRequestModel
        : BaseEventMessageModel
        , IContactRequestModel
    {
        public ContactRequestModel()
        {
        }

        public ContactRequestModel(BrkContactOut model)
            : base(model)
        {
            if (model.ContactOutSignal > 0)
                Command = (int)EnumCmdType.EVENT_CONTACT_ON_REQUEST;
            else
                Command = (int)EnumCmdType.EVENT_CONTACT_OFF_REQUEST;

            Detail = RequestFactory.Build<ContactDetailModel>(model.ReadWrite, model.ContactOutNumber, model.ContactOutSignal);
        }

        public ContactRequestModel(IContactEventModel model)
            : base(model)
        {
            if (model.ContactSignal > 0)
                Command = (int)EnumCmdType.EVENT_CONTACT_ON_REQUEST;
            else
                Command = (int)EnumCmdType.EVENT_CONTACT_OFF_REQUEST;

            Detail = RequestFactory.Build<ContactDetailModel>(model.ReadWrite, model.ContactNumber, model.ContactSignal);
        }

        [JsonProperty("detail", Order = 6)]
        public ContactDetailModel Detail { get; set; }
    }
}
