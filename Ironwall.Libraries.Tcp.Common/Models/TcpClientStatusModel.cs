using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpClientStatusModel : ITcpClientStatusModel
    {
        public TcpClientStatusModel()
        {

        }
        public TcpClientStatusModel(ITcpClientStatusModel model)
        {
            IsConnected = model.IsConnected;
            Status = model.Status;
        }

        public TcpClientStatusModel(bool isConnected, string status)
        {
            IsConnected = isConnected;
            Status = status;
        }

        public bool IsConnected { get; set; }
        public string Status { get; set; } = "Disconnected";

    }
}
