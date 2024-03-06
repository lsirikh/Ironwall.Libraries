using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class DeviceInfoRequestModel
        : UserSessionBaseRequestModel, IDeviceInfoRequestModel
    {
        public DeviceInfoRequestModel()
        {
            Command = (int)EnumCmdType.DEVICE_INFO_REQUEST;
        }

        public DeviceInfoRequestModel(ILoginSessionModel model)
            : base(model)
        {
            Command = (int)EnumCmdType.DEVICE_INFO_REQUEST;
        }
    }
}
