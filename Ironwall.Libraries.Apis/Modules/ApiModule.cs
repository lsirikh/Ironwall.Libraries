using Autofac;
using Ironwall.Libraries.Apis.Models;
using Ironwall.Libraries.Apis.Services;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Apis.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 2/8/2024 11:11:47 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ApiModule : Module
    {

        #region - Ctors -
        public ApiModule(ILogService log = default, string name = null)
        {
            _log = log;
            _setup = new ApiSetupModel();
            _name = name;
        }

        public ApiModule(ILogService log, ApiSetupModel setup, string name = "default")
        {
            _log = log;
            _setup = setup;
            _name = name;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterInstance(_setup).AsSelf().SingleInstance();
            //builder.RegisterType<ApiService>().AsImplementedInterfaces().SingleInstance().WithMetadata("Order", 2);
            builder.RegisterInstance(_setup).Named<ApiSetupModel>(_name).SingleInstance();
            builder.Register(build => new ApiService(_log, build.ResolveNamed<ApiSetupModel>(_name)))
                .Named<IApiService>(_name).SingleInstance().WithMetadata("Order", 2);
            _log?.Info($"{nameof(ApiModule)} is trying to create a single {nameof(ApiService)} instance.");
        }
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
        private ApiSetupModel _setup;
        private readonly string _name;
        #endregion
    }
}
