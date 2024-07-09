using Autofac;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Libraries.Events.Services;
using System;
using System.Diagnostics.Metrics;
using System.Net;

namespace Ironwall.Libraries.Events.Modules
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/27/2024 11:26:18 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class EventServerModule : Module
    {
        #region - Ctors -
        public EventServerModule(ILogService log = default, int count = default)
        {
            _log = log;
            _count = count;
        }
        #endregion
        #region - Implementation of Interface -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                var setupModel = new EventSetupModel();
                builder.RegisterInstance(setupModel).AsSelf().SingleInstance();
                builder.RegisterType<EventProvider>().SingleInstance();
                builder.RegisterType<ConnectionEventProvider>().SingleInstance();
                builder.RegisterType<DetectionEventProvider>().SingleInstance();
                builder.RegisterType<MalfunctionEventProvider>().SingleInstance();
                builder.RegisterType<ActionEventProvider>().SingleInstance();

                builder.RegisterType<EventDbService>().AsImplementedInterfaces().As<IService>().SingleInstance().WithMetadata("Order", _count);
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
        #endregion
    }
}