using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Factories
{
    public static class CameraFactory
    {

        public static T Build<T>(ICameraModel model) where T : CameraDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }


        public static T Build<T>(IDiscoveryDeviceModel model) where T : DiscoveryDeviceViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
    }
}
