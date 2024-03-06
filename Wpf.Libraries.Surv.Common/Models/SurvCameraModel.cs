namespace Wpf.Libraries.Surv.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 10:05:19 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvCameraModel : ISurvCameraModel
    {

        #region - Ctors -
        public SurvCameraModel()
        {

        }

        public SurvCameraModel(int id, string devicename, string ipaddress, int port, string username, string password, int mode, string rtspurl)
        {
            Id = id;
            DeviceName = devicename;
            IpAddress = ipaddress;
            Port = port;
            UserName = username;
            Password = password;
            Mode = mode;
            RtspUrl = rtspurl;

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
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Mode { get; set; }
        public string RtspUrl { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
