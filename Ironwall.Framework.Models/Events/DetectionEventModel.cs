using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class DetectionEventModel
        : MetaEventModel
        , IDetectionEventModel
    {

        public DetectionEventModel()
        {

        }

        public DetectionEventModel(IDetectionEventMapper model, IBaseDeviceModel device) 
            : base(model, device)
        {
            Result = model.Result;
        }

        public DetectionEventModel(IDetectionRequestModel model, IBaseDeviceModel device)
            : base(model, device)
        {
            Result = model.Detail.Result;
        }

        //public DetectionEventModel(IDetectionEventViewModel model) : base(model)
        //{
        //    Result = model.Result;
        //    try
        //    {
        //        Device = ModelFactory.Build<SensorDeviceModel>(model.Device as ISensorDeviceViewModel);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public int Result { get; set; }
    }
}
