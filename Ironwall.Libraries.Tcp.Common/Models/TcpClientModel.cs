using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpClientModel : TcpModel
    {
        public TcpClientModel()
        {

        }
        public TcpClientModel(int id, string ipAddress, int port)
            :base(id, ipAddress, port)
        {

        }

        public void Insert(int id, string ipAddress, int port)
        {
            Id = id;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
