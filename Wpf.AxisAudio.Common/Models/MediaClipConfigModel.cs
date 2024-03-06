using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/21/2023 10:31:42 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MediaClipConfigModel
    {

        #region - Ctors -
        public MediaClipConfigModel()
        {
            MediaClips = new List<MediaClipModel>();
        }

        public MediaClipConfigModel(int maxGroups, int maxUploadSize, List<MediaClipModel> mediaClips)
        {
            MaxGroups = maxGroups;
            MaxUploadSize = maxUploadSize;
            MaxGroups = maxGroups;
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
        [JsonProperty(PropertyName = "maxGroups", Order = 1)]
        public int MaxGroups { get; set; }
        [JsonProperty(PropertyName = "maxUploadSize", Order = 2)]
        public int MaxUploadSize { get; set; }
        [JsonProperty(PropertyName = "mediaClips", Order = 3)]
        public List<MediaClipModel> MediaClips { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
