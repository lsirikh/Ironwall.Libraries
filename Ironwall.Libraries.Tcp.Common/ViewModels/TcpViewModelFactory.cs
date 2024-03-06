using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.ViewModels
{
    public static class TcpViewModelFactory
    {

        public static T Build<T>(ITcpUserModel model) where T : TcpUserViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model.Id, model.TcpModel, model.UserModel });
            return instance;
        }
        public static T Build<T>(int id ,ITcpModel tcpModel, IUserModel userModel) where T : TcpUserViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] {id, tcpModel, userModel });
            return instance;
        }
    }
}
