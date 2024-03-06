using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    public interface IAudioBaseViewModel<T> : IAudioBaseModel where T : IAudioBaseModel
    {
        T Model { get; set; }
    }
}