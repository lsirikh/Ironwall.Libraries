using Caliburn.Micro;
using System;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IBaseEventViewModel
    {
        int Id { get; set; }
        DateTime DateTime { get; set; }
    }
}