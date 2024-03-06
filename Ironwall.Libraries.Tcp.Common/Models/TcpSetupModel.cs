using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpSetupModel
    {
        #region - Ctors -
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
        public int HeartBeat { get; } = Properties.Settings.Default.HeartBeat;
        #endregion
        #region - Attributes -
        //private string _serverIp = Properties.Settings.Default.ServerIp;
        //private int _serverPort = Properties.Settings.Default.ServerPort;

        //private int _clientId = Properties.Settings.Default.ClientId;
        //private string _clientIp = Properties.Settings.Default.ClientIp;
        //private int _clientPort = Properties.Settings.Default.ClientPort;
        #endregion
    }
}
