using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface IBaseDeviceViewModel<T> : IBaseCustomViewModel<T> where T : IBaseDeviceModel
    {
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        EnumDeviceType DeviceType { get; set; }
        EnumDeviceStatus Status { get; set; }
        string Version { get; set; }
    }
}