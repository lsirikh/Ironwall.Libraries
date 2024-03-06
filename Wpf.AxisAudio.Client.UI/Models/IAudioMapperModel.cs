using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    public interface IAudioMapperModel : IAudioBaseModel
    {
        string DeviceName { get; set; }
        string IpAddress { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        int ReferGroupId { get; set; }
        string UserName { get; set; }
    }
}