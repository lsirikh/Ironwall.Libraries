using Ironwall.Framework.Models.Communications;
using System;

namespace Ironwall.Framework.Models.Devices
{ 
    public interface IDeviceDetailModel : IUpdateDetailBaseModel
    {
        int Camera { get; }
        int Controller { get; }
        int Sensor { get; }
        
    }
}