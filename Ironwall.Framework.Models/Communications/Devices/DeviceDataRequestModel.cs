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
    public class DeviceDataRequestModel : BaseMessageModel, IDeviceDataRequestModel
    { 
        public DeviceDataRequestModel(EnumCmdType command = EnumCmdType.DEVICE_DATA_REQUEST)
         : base(command)
        {
        }

    }
}
