namespace Wpf.Surv.Sdk.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/26/2023 4:00:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvApiModel
    {

        #region - Ctors -
        public SurvApiModel()
        {

        }
        public SurvApiModel(string ip, uint port, string username, string password)
        {
            ApiAddress = ip;
            ApiPort = port;
            UserName = username;
            Password = password;
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
        public string ApiAddress { get; set; }
        public uint ApiPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
