using Ironwall.Libraries.Tcp.Common.Models;

namespace Ironwall.Libraries.Tcp.Client.UI.Models.Messages
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 10:19:33 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/


    public class ClientConnectionMessage
    {
        public ClientConnectionMessage()
        {
        }

        public ClientConnectionMessage(ITcpServerModel model, bool isConnect)
        {
            Model = model;
            IsConnect = isConnect;
        }

        public ClientConnectionMessage(int id, string ipAddress, int port, bool isConnect)
        {
            Model = TcpModelFactory.Build<TcpServerModel>(id, ipAddress, port);
            IsConnect = isConnect;
        }

        public ITcpServerModel Model { get; private set; }
        public bool IsConnect { get; private set; }

    }
}
