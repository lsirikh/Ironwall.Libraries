using Ironwall.Framework.Models.Devices;
using System;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:52:05 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class ViewModelFactory
    {
        public static T Build<T>(IBaseDeviceModel model) where T : DeviceViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IControllerDeviceModel model) where T : ControllerDeviceViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }

        public static T Build<T>(ISensorDeviceModel model) where T : SensorDeviceViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }

        public static T Build<T>(ICameraDeviceModel model) where T : CameraDeviceViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }

        public static T Build<T>(ICameraPresetModel model) where T : CameraPresetViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }

        public static T Build<T>(ICameraProfileModel model) where T : CameraProfileViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }

        public static T Build<T>(ICameraMappingModel model) where T : CameraMappingViewModel, new()
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
                return instance;
            }
            catch
            {
                return null;
            }

        }
    }
}
