using Newtonsoft.Json;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/18/2023 3:34:38 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class AudioBaseModel : IAudioBaseModel
    {

        #region - Ctors -
        public AudioBaseModel()
        {

        }

        public AudioBaseModel(int id)
        {
            Id = id;
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
        [JsonProperty(PropertyName = "id", Order = 0)]
        public int Id { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
