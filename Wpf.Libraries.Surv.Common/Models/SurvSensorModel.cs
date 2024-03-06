namespace Wpf.Libraries.Surv.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 8:53:37 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvSensorModel : ISurvSensorModel
    {

        #region - Ctors -
        public SurvSensorModel()
        {
        }
        public SurvSensorModel(int id, string groupName, string deviceName, int controllerId, int sensorId, int deviceType)
        {
            Id = id;
            GroupName = groupName;
            DeviceName = deviceName;
            ControllerId = controllerId;
            SensorId = sensorId;
            //Channel = channel;
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
        public string GroupName { get; set; }
        public string DeviceName { get; set; }
        public int ControllerId { get; set; }
        public int SensorId { get; set; }
        //public int Channel { get; set; }
        public int DeviceType { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
