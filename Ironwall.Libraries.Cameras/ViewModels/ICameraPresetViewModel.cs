using Ironwall.Libraries.Cameras.Models;
using System;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public interface ICameraPresetViewModel 
        : ICameraPresetModel
        , ICameraBaseViewModel
    {
        ICameraPresetModel Model { get; set; }

        event EventHandler Notify;
    }
}