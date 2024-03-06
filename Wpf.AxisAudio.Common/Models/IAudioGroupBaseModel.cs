using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Common.Models
{
    public interface IAudioGroupBaseModel : IAudioBaseModel
    {
        string GroupName { get; set; }
        int GroupNumber { get; set; }
    }
}