using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Tcp.Common.Defines
{
    public interface ITcpCommon
    {
        /// <summary>
        /// Tcp Connection Delegate for Event
        /// </summary>
        public delegate void TcpConnect_dele();
        /// <summary>
        /// Tcp Connection Delegate for Event
        /// </summary>
        public delegate void TcpSocketCreate_dele();
        /// <summary>
        /// Tcp Client Connection Delegate for Event
        /// </summary>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpAccept_dele(IPEndPoint endPoint = null);
        /// <summary>
        /// Send & Receive Message Delegate for Event from Tcp Socket
        /// </summary>
        /// <param name="msg">Message Data</param>
        /// <param name="endPoint">Connection Object</param>
        /// <param name="code">Connection Object</param>
        public delegate void TcpEvent_dele(string msg, IPEndPoint endPoint = null, EnumTcpCommunication code = default);
        /// <summary>
        /// Received Message Delegate for Event from Connected Clinet 
        /// </summary>
        /// <param name="msg">Message Data</param>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpReceive_dele(string msg, IPEndPoint endPoint = null, EnumTcpCommunication code = default);
        /// <summary>
        /// Send Message Delegate for Event from Connected Clinet
        /// </summary>
        /// <param name="msg">Message Data</param>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpSend_dele(string msg, IPEndPoint endPoint = null, EnumTcpCommunication code = default);
        /// <summary>
        /// Tcp Disconnection Delegate for Event
        /// </summary>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpDisconnect_dele(IPEndPoint endPoint = null);
        /// <summary>
        /// Tcp Login Delegate for Event
        /// </summary>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpLogin_dele(string userId, IPEndPoint endPoint = null);
        /// <summary>
        /// Tcp Logout Delegate for Event
        /// </summary>
        /// <param name="endPoint">Connection Object</param>
        public delegate void TcpLogout_dele(string userId, IPEndPoint endPoint = null);

        /// <summary>
        /// Tcp Client Connection Delegate for Event designated to Tcp Server
        /// </summary>
        /// <param name="obj"></param>
        public delegate void TcpCliConn_dele(object obj);
        /// <summary>
        /// Tcp Client Acceptance Delegate for Event designated to Tcp Server
        /// </summary>
        /// <param name="obj"></param>
        public delegate void TcpCliAccept_dele(object obj);
        /// <summary>
        /// Tcp Client Disconnection Delegate for Event designated to Tcp Server
        /// </summary>
        /// <param name="obj"></param>
        public delegate void TcpCliDiscon_dele(object obj);
    }
}
