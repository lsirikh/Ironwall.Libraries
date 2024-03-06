using Autofac;
using Autofac.Core;
using Ironwall.Libraries.Base.Services;
using RestSharp;
using System.Configuration;
using System;

namespace Ironwall.Libraries.WebApi.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 2:10:51 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WebApiModule : Module
    {

        #region - Ctors -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.Register(ctx =>
                {
                    var host = $"http://{IpAddress}:{Port}";
                    return new WebApiClient(new RestClient(host));
                })
                .As<IWebApiClient>()
                .SingleInstance();
            }
            catch (UriFormatException ex)
            {
                throw new Exception($"잘못된 URI 형식: {IpAddress}:{Port}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("WebApiClient 인스턴스 생성 중 오류 발생", ex);
            }
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
        public string IpAddress { get; set; }
        public int Port { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
