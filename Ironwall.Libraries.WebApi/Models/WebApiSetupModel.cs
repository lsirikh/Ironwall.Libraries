using System.Net;

namespace Ironwall.Libraries.WebApi.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 2:06:50 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WebApiSetupModel
    {

        #region - Ctors -
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
        public string IpAddress => Properties.Settings.Default.IpAddress;
        public int Port => Properties.Settings.Default.Port;
        #endregion
        #region - Attributes -
        #endregion
    }
}
