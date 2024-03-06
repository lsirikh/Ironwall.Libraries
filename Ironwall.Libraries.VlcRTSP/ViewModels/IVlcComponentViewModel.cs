using System.Threading.Tasks;
using Vlc.DotNet.Wpf;

namespace Ironwall.Libraries.VlcRTSP.ViewModels
{
    public interface IVlcComponentViewModel
    {
        string DeviceAddress { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string RtspUrl { get; set; }
        string StreamingLog { get; set; }
        int Thick { get; set; }
        string UserId { get; set; }
        bool Visibility { get; set; }
        VlcControl VlcControl { get; set; }

        Task StartVideo();
        Task StopVideo();
    }
}