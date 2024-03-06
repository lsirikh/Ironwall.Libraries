using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/18/2023 9:02:41 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupModel : AudioGroupBaseModel, IAudioGroupModel
    {

        #region - Ctors -
        public AudioGroupModel()
        {
            AudioModels = new ObservableCollection<AudioModel>();
            SensorModels = new ObservableCollection<AudioSensorModel>();
        }
        public AudioGroupModel(int id, int group, string name) : base(id, group, name)
        {
            AudioModels = new ObservableCollection<AudioModel>();
            SensorModels = new ObservableCollection<AudioSensorModel>();
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
        [JsonIgnore]
        public ObservableCollection<AudioModel> AudioModels { get; set; }
        [JsonIgnore]
        public ObservableCollection<AudioSensorModel> SensorModels { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
