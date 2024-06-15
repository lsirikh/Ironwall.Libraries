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
    public class DetectionEventMapper
        : MetaEventMapper
        , IDetectionEventMapper
    {

        public DetectionEventMapper()
        {

        }

        public DetectionEventMapper(IDetectionEventModel model) : base (model)
        {
            Result = model.Result;
        }

        public DetectionEventMapper(IDetectionRequestModel model, IBaseDeviceModel device) 
            : base(model, device)
        {
            Result = model.Detail.Result;
        }

        public int Result { get; set; }
    }
}
