using Caliburn.Micro;
using System;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IBaseEventViewModel
    {
        DateTime DateTime { get; set; }
        string Id { get; set; }
    }
}