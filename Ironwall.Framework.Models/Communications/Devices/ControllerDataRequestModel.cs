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
    public class ControllerDataRequestModel : BaseMessageModel, IControllerDataRequestModel
    {

        public ControllerDataRequestModel(EnumCmdType command = EnumCmdType.CONTROLLER_DATA_REQUEST)
         : base(command)
        {
        }

    }
}
