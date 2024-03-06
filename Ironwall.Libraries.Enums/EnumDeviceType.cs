using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Enums
{
    public enum EnumDeviceType : int
    {
        NONE = 0,
        Controller = 1,
        Multi = 2,
        Fence = 3,
        Underground = 4,
        Contact = 5,
        PIR = 6,
        IoController = 7,
        Laser = 8,

        Cable = 9,
        IpCamera = 10,

        IpSpeaker = 11,
        Radar = 12,
        OpticalCable = 13,

        Fence_Line = 20,
    }
    
}
