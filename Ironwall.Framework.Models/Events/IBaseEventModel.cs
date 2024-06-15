using Ironwall.Framework.Models.Devices;
using System;

namespace Ironwall.Framework.Models.Events
{
    public interface IBaseEventModel
    {
        string Id { get; set; }
        
        DateTime DateTime { get; set; }
    }
}