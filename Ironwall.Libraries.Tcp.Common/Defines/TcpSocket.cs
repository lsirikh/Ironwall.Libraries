using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Defines
{
    public abstract class TcpSocket : TcpTaskTimer, ITcpSocket
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public abstract void InitSocket();
        //protected abstract void CreateSocket();
        public abstract Task SendRequest(string msg, IPEndPoint selectedIp = null);
        public abstract Task SendFileRequest(string file, IPEndPoint selectedIp = null);
        public abstract Task SendMapDataRequest(string file, IPEndPoint selectedIp = null);
        public abstract Task SendProfileDataRequest(string file, IPEndPoint selectedIp = null);
        public abstract Task SendVideoDataRequest(string file, IPEndPoint selectedIp = null);
        public abstract void CloseSocket();
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //소켓
        public Socket Socket { get; protected set; }


        /// <summary>
        /// Mode
        ///0:Closed, 1:Created and Disconnected, 2:Connected
        /// </summary>
        public int Mode { get; set; }

        #endregion
        #region - Attributes -
        //문자열 처리용 Stringbuilder
        protected StringBuilder sb;
        #endregion
    }
}