using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Framework.Models.Communications
{
    public interface IBaseEventMessageModel : IBaseMessageModel
    { 
        int Id { get; set; }
        DateTime DateTime { get; set; }
    }
}