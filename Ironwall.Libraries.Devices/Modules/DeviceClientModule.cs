using Autofac;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Services;
using System;

namespace Ironwall.Libraries.Devices.Modules
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/4/2024 10:35:46 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class DeviceClientModule : Module
    {
        #region - Ctors -
        public DeviceClientModule(ILogService log = default, int count = default)
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
                var deviceSetupModel = new DeviceSetupModel();
                builder.RegisterInstance(deviceSetupModel).AsSelf().SingleInstance();

                builder.RegisterType<DeviceCategoryProvider>().SingleInstance();
                builder.RegisterType<DeviceProvider>().SingleInstance();
                builder.RegisterType<CameraOptionProvider>().SingleInstance();

                builder.RegisterType<ControllerDeviceProvider>().As<ControllerDeviceProvider>()
                    .As<ILoadable>().SingleInstance().WithMetadata("Order", _count++);
                builder.RegisterType<SensorDeviceProvider>().As<SensorDeviceProvider>()
                    .As<ILoadable>().SingleInstance().WithMetadata("Order", _count++);
                builder.RegisterType<CameraDeviceProvider>().As<CameraDeviceProvider>()
                    .As<ILoadable>().SingleInstance().WithMetadata("Order", _count++);

                builder.RegisterType<CameraPresetProvider>().As<CameraPresetProvider>()
                    .As<ILoadable>().SingleInstance().WithMetadata("Order", _count++);
                builder.RegisterType<CameraProfileProvider>().As<CameraProfileProvider>()
                    .As<ILoadable>().SingleInstance().WithMetadata("Order", _count++);
                builder.RegisterType<CameraMappingProvider>().SingleInstance();
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