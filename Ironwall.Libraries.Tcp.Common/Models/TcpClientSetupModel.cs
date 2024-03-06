using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpClientSetupModel
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
        public string ClientIp
        {
            get { return _clientIp; }
            set
            {
                _clientIp = value;
                Properties.Settings.Default.ClientIp = _clientIp;
                Properties.Settings.Default.Save();
            }
        }

        public string ServerIp
        {
            get { return _serverIp; }
            set
            {
                _serverIp = value;
                Properties.Settings.Default.ServerIp = _serverIp;
                Properties.Settings.Default.Save();
            }
        }
        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value;
                Properties.Settings.Default.ServerPort = _serverPort;
                Properties.Settings.Default.Save();
            }
        }

        public int ClientPort
        {
            get { return _clientPort; }
            set
            {
                _clientPort = value;
                Properties.Settings.Default.ClientPort = _clientPort;
                Properties.Settings.Default.Save();
            }
        }
        public int ClientId
        {
            get { return _clientId; }
            set
            {
                _clientId = value;
                Properties.Settings.Default.ClientId = _clientId;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsAutoConnect
        {
            get { return _isAutoConnect; }
            set 
            {
                _isAutoConnect = value;
                Properties.Settings.Default.IsAutoConnect = _isAutoConnect;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        #region - Attributes -
        private int _clientId = Properties.Settings.Default.ClientId;
        private string _clientIp = Properties.Settings.Default.ClientIp;
        private int _clientPort = Properties.Settings.Default.ClientPort;

        private string _serverIp = Properties.Settings.Default.ServerIp;
        private int _serverPort = Properties.Settings.Default.ServerPort;

        private bool _isAutoConnect = Properties.Settings.Default.IsAutoConnect;
        #endregion
    }
}
