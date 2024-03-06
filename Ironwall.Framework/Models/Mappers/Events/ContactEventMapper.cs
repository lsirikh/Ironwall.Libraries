using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class ContactEventMapper
        : MetaEventMapper
        , IContactEventMapper
    {
        public ContactEventMapper()
        {

        }

        public ContactEventMapper(IContactEventModel model) : base(model)
        {
            ReadWrite = model.ReadWrite;
            ContactNumber = model.ContactNumber;
            ContactSignal = model.ContactSignal;
        }

        public ContactEventMapper(IContactRequestModel model, IBaseDeviceModel device)
            : base(model, device)
        {
            ReadWrite = model.Detail.ReadWrite;
            ContactNumber = model.Detail.ContactNumber;
            ContactSignal = model.Detail.ContactSignal;
        }

        public int ReadWrite { get; set; }
        public int ContactNumber { get; set; }
        public int ContactSignal { get; set; }

    }
}
