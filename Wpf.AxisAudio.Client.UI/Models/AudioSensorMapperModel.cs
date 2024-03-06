using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/3/2024 7:40:24 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSensorMapperModel
    {

        #region - Ctors -
        public AudioSensorMapperModel()
        {
            
        }
        public AudioSensorMapperModel(IAudioSensorModel model)
        {
            Id = model.Id;
            GroupNumber = model.Group.GroupNumber;
            DeviceName = model.DeviceName;
            ControllerId = model.ControllerId;
            SensorId = model.SensorId;
            DeviceType = model.DeviceType;
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
        public int Id { get; set; }
        public int GroupNumber { get; set; }
        public string DeviceName { get; set; }
        public int ControllerId { get; set; }
        public int SensorId { get; set; }
        public int DeviceType { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
