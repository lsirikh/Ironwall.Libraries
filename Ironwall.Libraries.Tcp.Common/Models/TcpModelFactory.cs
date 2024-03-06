using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public static class TcpModelFactory
    {
        public static T Build<T>(ITcpServerModel model) where T : TcpServerModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(int id, string ipAddress, int port) where T :  new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { id, ipAddress, port });
            return instance;
        }

        public static T Build<T>(ITcpClientStatusModel model) where T : TcpClientStatusModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(bool isConnected, string status) where T : TcpClientStatusModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { isConnected, status });
            return instance;
        }

        public static T Build<T>(int id, ITcpModel tcpModel, IUserModel userModel) where T : TcpUserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { id, tcpModel, userModel });
            return instance;
        }
    }
}
