using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class CameraDataRequestModel
        : UserSessionBaseRequestModel, ICameraDataRequestModel
    {
        public CameraDataRequestModel()
        {
            Command = (int)EnumCmdType.CAMERA_DATA_REQUEST;
        }

        public CameraDataRequestModel(ILoginSessionModel model)
            : base(model)
        {
            Command = (int)EnumCmdType.CAMERA_DATA_REQUEST;
        }
    }
}
