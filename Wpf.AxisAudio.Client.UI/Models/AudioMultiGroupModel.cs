using Newtonsoft.Json;
using System.Collections.Generic;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/3/2024 2:54:11 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioMultiGroupModel : AudioBaseModel, IAudioMultiGroupModel
    {
        public AudioMultiGroupModel()
        {
            
        }


        [JsonProperty(PropertyName = "audio_id", Order = 1)]
        public int AudioId { get; set; }
        [JsonProperty(PropertyName = "groups", Order = 2)]
        public int GroupId { get; set; }
    }
}
