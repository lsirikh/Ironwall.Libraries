namespace Wpf.AxisAudio.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 1:29:39 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSensorModel : IAudioSensorModel
    {

        #region - Ctors -
        public AudioSensorModel()
        {
            Group = new AudioGroupBaseModel();
        }
        public AudioSensorModel(int id, AudioGroupBaseModel group, string deviceName, int controllerId, int sensorId, int deviceType)
        {
            Id = id;
            Group = group;
            DeviceName = deviceName;
            ControllerId = controllerId;
            SensorId = sensorId;
            DeviceType = deviceType;

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
        public AudioGroupBaseModel Group { get; set; }
        public string DeviceName { get; set; }
        public int ControllerId { get; set; }
        public int SensorId { get; set; }
        public int DeviceType { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
