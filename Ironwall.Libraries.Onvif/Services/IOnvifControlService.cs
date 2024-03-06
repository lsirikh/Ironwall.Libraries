using Mictlanix.DotNet.Onvif.Common;
using Mictlanix.DotNet.Onvif.Device;
using Mictlanix.DotNet.Onvif.Imaging;
using Mictlanix.DotNet.Onvif.Media;
using Mictlanix.DotNet.Onvif.Ptz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnvifControl.Libraries.Onvif.Services
{
    public interface IOnvifControlService
    {
        bool Absolute_move { get; }
        GetCapabilitiesResponse Caps { get; }
        bool Continuous_move { get; }
        DeviceClient Device { get; }
        bool Focus { get; }
        ImagingClient Imaging { get; }
        MediaClient Media { get; }
        string Profile_token { get; }
        PTZClient Ptz { get; }
        bool Relative_move { get; }

        event OnvifControlService.LogEvent SendLog;

        bool CheckCaps();
        bool CheckDevice();
        bool CheckImaging();
        bool CheckMedia();
        bool CheckProfileToken();
        bool CheckPtz();
        Task CreateProfile(CancellationToken token = default);
        Task<bool> DeviceReady(string host, string username, string password, CancellationToken token = default);
        Task<Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse> GetPresets(CancellationToken token = default);
        Task<bool> GotoPreset(PTZPreset preset, CancellationToken token = default);
        void InitClass();
        Task MainAsync(string host, string username, string password, CancellationToken token = default);
        Task MovePtz(int moveType, CancellationToken token = default);
        Task<PTZStatus> GetPtzStatus();
        Task<string> IsPanTiltMoving();
        Task<string> IsZoomMoving();
        Task<bool> StopPreset(bool stopPanTile = true, bool stopZoom = true);


    }
}