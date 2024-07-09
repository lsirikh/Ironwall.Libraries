using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class DetectionEventModel: MetaEventModel, IDetectionEventModel
    {

        public DetectionEventModel()
        {

        }

        public DetectionEventModel(IDetectionEventMapper model, IBaseDeviceModel device) 
            : base(model, device)
        {
            Result = model.Result;
        }

        public DetectionEventModel(IDetectionEventModel model): base(model)
        {
            Result = model.Result;
        }


        [JsonProperty("result", Order = 6)]
        public int Result { get; set; }
    }
}
