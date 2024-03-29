﻿using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Events
{
    public interface IMalfunctionEventModel : IMetaEventModel
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        int Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}