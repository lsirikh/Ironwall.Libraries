using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ConnectionEventModel
        : MetaEventModel, IConnectionEventModel
    {
        public ConnectionEventModel()
        {

        }

        public ConnectionEventModel(IConnectionEventMapper model, IBaseDeviceModel device)
            : base(model, device)
        {
        }
        public ConnectionEventModel(IConnectionRequestModel model, IBaseDeviceModel device)
            : base(model, device)
        {
        }

        public ConnectionEventModel(IConnectionEventViewModel model) : base(model)
        {
        }
    }
}
