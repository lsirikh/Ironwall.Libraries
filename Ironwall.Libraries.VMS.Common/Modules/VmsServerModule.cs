using Autofac;
using Autofac.Core;
using Ironwall.Libraries.Apis.Models;
using Ironwall.Libraries.Apis.Modules;
using Ironwall.Libraries.Apis.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Libraries.VMS.Common.Models;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.Common.Services;
using Sensorway.Accounts.Base.Models;
using System;
using System.Net;

namespace Ironwall.Libraries.VMS.Common.Modules
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/4/2024 11:14:50 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsServerModule : Module
    {
        #region - Ctors -
        public VmsServerModule(ILogService log = default, int count = default)
        {
            _log = log;
            _count = count;
        }
        
        public VmsServerModule(string apiAddress, int port, string username, string password, ILogService log = default, int count = default)
        {
            _log = log;
            _count = count;

            _apiAddress = apiAddress;
            _port = port;
            _userName = username;
            _password = password;
        }

        #endregion
        #region - Implementation of Interface -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.RegisterType<VmsSetupModel>().SingleInstance();
                builder.RegisterModule(new ApiModule(_log, new ApiSetupModel
                {
                    IpAddress = _apiAddress,
                    Port = _port,
                    Username = _userName,
                    Password = _password,
                }, "VmsApi"));
                builder.RegisterType<LoginSessionModel>().SingleInstance();

                builder.RegisterType<VmsApiProvider>().SingleInstance();
                builder.RegisterType<VmsSensorProvider>().SingleInstance();
                builder.RegisterType<VmsMappingProvider>().SingleInstance();
                builder.RegisterType<VmsEventProvider>().SingleInstance();


                builder.RegisterType<VmsControlService>().SingleInstance();
                builder.RegisterType<VmsDbService>().As<VmsDbService>()
                    .As<IVmsDbService>().As<IService>().SingleInstance().WithMetadata("Order", _count++);

                builder.Register(build => new VmsApiService( _log,
                                                            build.ResolveNamed<IApiService>("VmsApi"),
                                                            build.Resolve<IVmsDbService>(),
                                                            build.Resolve<LoginSessionModel>(),
                                                            build.Resolve<VmsEventProvider>()
                                                            )).AsImplementedInterfaces().SingleInstance().WithMetadata("Order", _count++);
            }
            catch
            {
                throw;
            }
        }
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
        private ILogService _log;
        private int _count;
        private string _apiAddress;
        private int _port;
        private string _userName;
        private string _password;
        #endregion
    }
}