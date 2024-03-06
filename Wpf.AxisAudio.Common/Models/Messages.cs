using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/14/2023 9:40:32 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class StreamRequestMessage : StreamBaseMessage
    {

        [JsonConstructor]
        public StreamRequestMessage(List<AudioModel> models, List<AudioModel> requestModel, bool control)
            :base(models, control)
        {
            RequestModel = requestModel;
        }

        [JsonProperty(PropertyName = "request_model", Order = 2)]
        public List<AudioModel> RequestModel { get; }
    }

    public class StreamResponseMessage : StreamBaseMessage
    {
        public StreamResponseMessage(List<AudioModel> models, bool control, bool isSuccess)
            : base(models, control)
        {
            IsSuccess = isSuccess;
        }

        [JsonProperty(PropertyName ="isSuccess", Order = 2) ]
        public bool IsSuccess { get; }
    }

    public class StreamInitRequestMessage : StreamBaseMessage
    {

        [JsonConstructor]
        public StreamInitRequestMessage(List<AudioModel> models, bool control = false)
            : base(models, control)
        {
        }
    }

    public class RequestClipPlayMessageModel : ClipBaseMessage
    {
        public RequestClipPlayMessageModel(List<AudioModel> models
            //, List<AudioModel> requestModel
            , bool control, int clip, int repeat = -1, int volume = 100)
            :base(models, control, clip, repeat, volume)
        {
            //RequestModel = requestModel;
        }

        //[JsonProperty(PropertyName = "request_model", Order = 3)]
        //public List<AudioModel> RequestModel { get; }
    }

    public class ResponseClipPlayMessageModel : StreamBaseMessage
    {
        public ResponseClipPlayMessageModel(List<AudioModel> models, bool control, bool isSuccess)
            : base(models, control)
        {
            IsSuccess = isSuccess;
        }

        [JsonProperty(PropertyName = "isSuccess", Order = 2)]
        public bool IsSuccess { get; }
    }

    public class StreamBaseMessage
    {
        public StreamBaseMessage(List<AudioModel> models, bool control)
        {
            Models = models;
            Control = control;
        }

        [JsonProperty(PropertyName ="models", Order = 0) ]
        public List<AudioModel> Models { get; }

        [JsonProperty(PropertyName ="control", Order = 1) ]
        public bool Control { get; }
    }

    public class ClipBaseMessage : StreamBaseMessage
    {
        public ClipBaseMessage(List<AudioModel> models, bool control, int clip, int repeat, int volume) : base(models, control) 
        {
            Clip = clip;
            Repeat = repeat;
            Volume = volume;
        }
        [JsonProperty(PropertyName = "clip", Order = 3)]
        public int Clip { get; set; }
        [JsonProperty(PropertyName = "repeat", Order = 4)]
        public int Repeat { get; set; }
        [JsonProperty(PropertyName = "volume", Order = 5)]
        public int Volume { get; set; }

    }
}
