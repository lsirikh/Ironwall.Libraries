using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpServerModel : TcpModel, ITcpServerModel
    {
        public TcpServerModel()
        {

        }

        public TcpServerModel(int id, string ipAddress, int port)
        {
            Id = id;
            IpAddress = ipAddress;
            Port = port;
        }

        public TcpServerModel(ITcpServerModel model)
        {
            Id = model.Id;
            IpAddress = model.IpAddress;
            Port = model.Port;
        }

        //public void Insert(int id, string ipAddress, int port)
        //{
        //    Id = id;
        //    IpAddress = ipAddress;
        //    Port = port;
        //}
    }
}
