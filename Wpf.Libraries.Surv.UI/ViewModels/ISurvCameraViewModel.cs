using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    public interface ISurvCameraViewModel : IBaseViewModel
    {
        string DeviceName { get; set; }
        int Id { get; set; }
        string IpAddress { get; set; }
        int Mode { get; set; }
        ISurvCameraModel Model { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string RtspUrl { get; set; }
        string UserName { get; set; }
        event Action<string> IpChanged;
        void SubscribeToIpChanged(Action<string> handler);
        void UnsubscribeFromIpChanged(Action<string> handler);

    }
}