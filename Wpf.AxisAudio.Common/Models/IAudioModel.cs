using System.Collections.Generic;

namespace Wpf.AxisAudio.Common.Models
{
    public interface IAudioModel : IAudioBaseModel
    {
        List<AudioGroupBaseModel> Groups { get; set; }
        string DeviceName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
        int Mode { get; set; }
        MediaClipConfigModel MediaClip { get; set; }
    }
}