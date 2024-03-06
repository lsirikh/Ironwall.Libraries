using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Server.Models
{
    public class TcpServerSetupModel
    {
        #region - Ctors -
        public TcpServerSetupModel(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        
        #region - Overrides -
        #endregion
        
        #region - Binding Methods -
        #endregion
        
        #region - Processes -
        #endregion
       
        #region - IHanldes -
        #endregion
        
        #region - Properties -
        public string Ip { get; set; }

        public int Port { get; set; }
        #endregion
        
        #region - Attributes -

        #endregion
    }
}
