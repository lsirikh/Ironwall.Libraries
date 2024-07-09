using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Framework.Models.Events
{
    public interface IBaseEventModel : IBaseModel
    {
        EnumEventType? MessageType { get; set; }
        DateTime DateTime { get; set; }
    }
}