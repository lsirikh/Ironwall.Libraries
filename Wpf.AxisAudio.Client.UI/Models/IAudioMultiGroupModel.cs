using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    public interface IAudioMultiGroupModel : IAudioBaseModel
    {
        int AudioId { get; set; }
        int GroupId { get; set; }
    }
}