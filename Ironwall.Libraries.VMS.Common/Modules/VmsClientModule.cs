using Autofac;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Providers;
using System;
using Ironwall.Libraries.VMS.Common.Models;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Sensorway.Accounts.Base.Models;

namespace Ironwall.Libraries.VMS.Common.Modules
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/4/2024 11:15:04 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsClientModule : Module
    {
        #region - Ctors -
        public VmsClientModule(ILogService log = default, int count = default)
        {
            _log = log;
            _count = count;
        }

        public VmsClientModule(string apiAddress, int port, string username, string password, ILogService log = default, int count = default)
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
                builder.RegisterType<LoginSessionModel>().SingleInstance();

                builder.RegisterType<VmsApiProvider>().SingleInstance();
                builder.RegisterType<VmsSensorProvider>().SingleInstance();
                builder.RegisterType<VmsMappingProvider>().SingleInstance();
                builder.RegisterType<VmsEventProvider>().SingleInstance();
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