using Ironwall.Libraries.Tcp.Client.UI.Models.Messages;
using Ironwall.Libraries.Tcp.Common.Models;
using System;

namespace Ironwall.Libraries.Tcp.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 10:18:17 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class MessageFactory
    {

        public static T Build<T>(ITcpServerModel model, bool isConnect) where T : ClientConnectionMessage, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, isConnect });
            return instance;
        }

        public static T Build<T>(int id, string ipAddress, int port, bool isConnect) where T : ClientConnectionMessage, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { id, ipAddress, port, isConnect });
            return instance;
        }
    }
}
