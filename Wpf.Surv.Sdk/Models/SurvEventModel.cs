namespace Wpf.Surv.Sdk.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/26/2023 4:02:54 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvEventModel
    {

        #region - Ctors -
        public SurvEventModel()
        {

        }
        public SurvEventModel(int channel = 0, string ipAddress = default, string eventName = default, bool isOn = false, int id = 1)
        {
            Channel = channel;
            CamIpAddress = ipAddress;
            EventName = eventName;
            IsOn = isOn;
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
        public int Channel { get; set; }
        public string CamIpAddress { get; set; }
        public string EventName { get; set; }
        public bool IsOn { get; set; }
        public int Id { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
