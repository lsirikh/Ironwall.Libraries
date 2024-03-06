namespace Wpf.Libraries.Surv.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/26/2023 4:02:54 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvEventModel : ISurvEventModel
    {

        #region - Ctors -
        public SurvEventModel()
        {

        }
        public SurvEventModel(int id, int channel = 0, string ipAddress = default, string eventName = "Alarm", bool isOn = false, int eventId = 1, int apiId = 0, int cameraId = 0)
        {
            Id = id;
            Channel = channel;
            IpAddress = ipAddress;
            EventName = eventName;
            IsOn = isOn;
            EventId = eventId;
            ApiId = apiId;
            CameraId = cameraId;
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
        public int Channel { get; set; }
        public string IpAddress { get; set; }
        public string EventName { get; set; }
        public bool IsOn { get; set; }
        public int EventId { get; set; }
        public int ApiId { get; set; }
        public int CameraId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
