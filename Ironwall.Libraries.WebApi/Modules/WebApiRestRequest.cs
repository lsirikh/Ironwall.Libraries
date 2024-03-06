using RestSharp;
using System;

namespace Ironwall.Libraries.WebApi.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 3:04:42 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WebApiRestRequest : RestRequest
    {

        #region - Ctors -
        public WebApiRestRequest(string resource, Method method = Method.Get) : base(resource, method) 
        { 
        }

        public WebApiRestRequest(Uri resource, Method method = Method.Get) : base(resource, method)
        {
            
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
