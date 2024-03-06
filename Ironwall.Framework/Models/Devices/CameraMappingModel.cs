using Ironwall.Framework.Models.Mappers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Xml.Linq;

namespace Ironwall.Framework.Models.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/28/2023 2:46:18 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingModel : BaseModel, ICameraMappingModel
    {

        #region - Ctors -
        public CameraMappingModel()
        {

        }

        public CameraMappingModel(string id, string group, SensorDeviceModel sensor, CameraPresetModel preset1, CameraPresetModel preset2) : base(id)
        {
            Group = group;
            Sensor = sensor;
            FirstPreset = preset1;
            SecondPreset = preset2;
        }

        public CameraMappingModel(IMappingTableMapper model, SensorDeviceModel sensor, CameraPresetModel preset1, CameraPresetModel preset2) : base(model.MapperId)
        {
            Group = model.GroupId;
            Sensor = sensor;
            FirstPreset = preset1;
            SecondPreset = preset2;
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
        [JsonProperty("group", Order = 2)]
        public string Group { get; set; }
        [JsonProperty("sensor", Order = 3)]
        public SensorDeviceModel Sensor { get; set; }
        [JsonProperty("first_preset", Order = 4)]
        public CameraPresetModel FirstPreset { get; set; }
        [JsonProperty("second_preset", Order = 5)]
        public CameraPresetModel SecondPreset { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
