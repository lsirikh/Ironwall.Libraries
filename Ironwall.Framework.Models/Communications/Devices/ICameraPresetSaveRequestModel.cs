﻿using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraPresetSaveRequestModel : IUserSessionBaseRequestModel
    {
        List<CameraPresetModel> Body { get; set; }
    }
}