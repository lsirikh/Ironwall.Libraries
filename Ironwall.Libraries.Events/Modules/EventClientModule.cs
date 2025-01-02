using Autofac;
using Autofac.Core;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Events.Providers;
using System;

namespace Ironwall.Libraries.Events.Modules
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/27/2024 11:13:38 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class EventClientModule : Module
    {
        #region - Ctors -
        public EventClientModule(ILogService log = default)
        {
            _log = log;
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
            }
            catch (Exception ex) 
            {
                _log.Error(ex.Message);
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
        #endregion
    }
}