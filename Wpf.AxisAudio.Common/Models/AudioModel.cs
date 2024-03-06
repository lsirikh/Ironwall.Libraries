using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/14/2023 9:43:08 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioModel : AudioBaseModel, IAudioModel
    {

        #region - Ctors -
        public AudioModel()
        {
            Groups = new List<AudioGroupBaseModel>();
        }
        public AudioModel(int id, string device, string name, string pass, string ip, int port) : base(id)
        {
            Groups = new List<AudioGroupBaseModel>();
            DeviceName = device;
            UserName = name;
            Password = pass;
            IpAddress = ip;
            Port = port;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        [JsonProperty(PropertyName = "groups", Order = 1)]
        public List<AudioGroupBaseModel> Groups { get; set; } //Group을 참조하기 위한 용도

        [JsonProperty(PropertyName = "deviceName", Order = 2)]
        public string DeviceName { get; set; }

        [JsonProperty(PropertyName = "userName", Order = 3)]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "password", Order = 4)]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "ipAddress", Order = 5)]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "port", Order = 6)]
        public int Port { get; set; }

        //[JsonProperty(PropertyName = "isOperatable", Order = 7)]
        //public bool IsOperatable { get; set; }

        /// <summary>
        /// Mode 0 : None
        /// Mode 1 : Microphone streaming Mode
        /// Mode 2 : Audio Clip Play Mode
        /// </summary>
        [JsonProperty(PropertyName = "mode", Order = 8)]
        public int Mode { get; set; }

        [JsonProperty(PropertyName = "media", Order = 9)]
        public MediaClipConfigModel MediaClip { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
