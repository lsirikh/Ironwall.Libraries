using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface IBaseOptionViewModel<T> : IBaseCustomViewModel<T> where T : IOptionBaseModel
    {
        string ReferenceId { get; set; }
    }
}