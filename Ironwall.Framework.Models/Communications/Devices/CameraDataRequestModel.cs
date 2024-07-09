using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class CameraDataRequestModel : BaseMessageModel, ICameraDataRequestModel
    {
        public CameraDataRequestModel(EnumCmdType command = EnumCmdType.CAMERA_DATA_REQUEST)
            :base(EnumCmdType.CAMERA_DATA_REQUEST)
        {
        }
    }
}
