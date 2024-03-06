using Newtonsoft.Json;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/4/2024 1:26:33 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupBaseModel : AudioBaseModel, IAudioGroupBaseModel
    {

        #region - Ctors -
        public AudioGroupBaseModel()
        {

        }
        public AudioGroupBaseModel(int id, int group, string name) : base(id)
        {
            GroupNumber = group;
            GroupName = name;
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
        [JsonProperty(PropertyName = "group_number", Order = 1)]
        public int GroupNumber { get; set; }
        [JsonProperty(PropertyName = "group_name", Order = 2)]
        public string GroupName { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
