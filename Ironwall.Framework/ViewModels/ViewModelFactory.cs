using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public static class ViewModelFactory
    {

        #region - From UserModel to UserViewModel - 
        public static T Build<T>(IUserModel model) where T : UserViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(
            int id,
            string userId,
            string password,
            string name,
            string empnum,
            string birth,
            string phone,
            string address,
            string email,
            string image,
            string position,
            string department,
            string company,
            int level,
            bool used) where T : UserViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] {
            id,userId,password,name,empnum,birth,phone,address,email,image,position,
            department,company,level,used});
            return instance;
        }

        public static T Build<T>(ILoginSessionModel model) where T : LoginSessionViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(ILoginUserModel model) where T : LoginUserViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        //public static T Build<T>(IUserModel model, ILoginSessionModel loginSessionModel) where T : UserViewModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, loginSessionModel });
        //    return instance;
        //}
        #endregion

        #region - From EventModel to EventViewModel - 
        public static T Build<T>(IMetaEventModel model) where T : MetaEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IDetectionEventModel model) where T : DetectionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IMalfunctionEventModel model) where T : MalfunctionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IActionEventModel model) where T : ActionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
        #endregion

        #region - From DeviceModel to DeviceViewModel -
        public static T Build<T>(IBaseDeviceModel model) where T : BaseDeviceViewModel, new()
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
        #endregion
    }
}
