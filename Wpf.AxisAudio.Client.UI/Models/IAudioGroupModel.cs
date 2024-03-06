using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wpf.AxisAudio.Common.Models
{
    public interface IAudioGroupModel : IAudioGroupBaseModel
    {
        ObservableCollection<AudioModel> AudioModels { get; set; }
        ObservableCollection<AudioSensorModel> SensorModels { get; set; }
    }
}