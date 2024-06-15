using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class DeviceDataRequestModel
        : UserSessionBaseRequestModel, IDeviceDataRequestModel
    {
        public DeviceDataRequestModel()
        {
            Command = EnumCmdType.DEVICE_DATA_REQUEST;
        }

        public DeviceDataRequestModel(ILoginSessionModel model)
            : base(model)
        {
            Command = EnumCmdType.DEVICE_DATA_REQUEST;
        }
        
    }
}
