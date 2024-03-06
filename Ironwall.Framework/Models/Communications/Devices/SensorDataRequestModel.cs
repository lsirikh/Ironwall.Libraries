using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class SensorDataRequestModel
    : UserSessionBaseRequestModel, ISensorDataRequestModel
    {
        public SensorDataRequestModel()
        {
            Command = (int)EnumCmdType.SENSOR_DATA_REQUEST;
        }
        public SensorDataRequestModel(ILoginSessionModel model)
            : base(model)
        {
            Command = (int)EnumCmdType.SENSOR_DATA_REQUEST;
        }

    }
}
