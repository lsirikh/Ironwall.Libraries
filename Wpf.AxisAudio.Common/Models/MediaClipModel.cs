using Newtonsoft.Json;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/21/2023 10:32:56 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MediaClipModel
    {

        #region - Ctors -
        public MediaClipModel()
        {

        }

        public MediaClipModel(int id)
        {
            Id = id;
        }

        public MediaClipModel(int id, string location, string name, string type)
        {
            Id = id;
            Location = location;
            Name = name;
            Type = type;
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
        [JsonProperty(PropertyName = "id", Order = 1)]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "location", Order = 2)]
        public string Location { get; set; }
        [JsonProperty(PropertyName = "name", Order = 3)]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "yype", Order = 4)]
        public string Type { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
