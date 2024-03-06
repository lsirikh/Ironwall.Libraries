using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models.Mappers.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class MalfunctionEventModel
        : MetaEventModel, IMalfunctionEventModel
    {

        public MalfunctionEventModel()
        {

        }

        public MalfunctionEventModel(IMalfunctionEventMapper model, IBaseDeviceModel device) : base(model, device)
        {
            Reason = model.Reason;
            FirstStart = model.FirstStart;
            FirstEnd = model.FirstEnd;
            SecondStart = model.SecondStart;
            SecondEnd = model.SecondEnd;
        }

        public MalfunctionEventModel(IMalfunctionRequestModel model, IBaseDeviceModel device) : base(model, device)
        {
            Reason = model.Detail.Reason;
            FirstStart = model.Detail.FirstStart;
            FirstEnd = model.Detail.FirstEnd;
            SecondStart = model.Detail.SecondStart;
            SecondEnd = model.Detail.SecondEnd;
        }

        public MalfunctionEventModel(IMalfunctionEventViewModel model) : base(model)
        {
            Reason = model.Reason;
            FirstStart = model.FirstStart;
            FirstEnd = model.FirstEnd;
            SecondStart = model.SecondStart;
            SecondEnd = model.SecondEnd;
            Device = ModelTypeHelper.GetDevice(model.Device);
        }

        

        public int Reason { get; set; }
        public int FirstStart { get; set; }
        public int FirstEnd { get; set; }
        public int SecondStart { get; set; }
        public int SecondEnd { get; set; }

    }
}
