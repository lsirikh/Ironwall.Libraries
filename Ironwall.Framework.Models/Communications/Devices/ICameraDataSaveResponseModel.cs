﻿using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraDataSaveResponseModel : IResponseModel
    {
        List<CameraDeviceModel> Body { get; set; }
    }
}