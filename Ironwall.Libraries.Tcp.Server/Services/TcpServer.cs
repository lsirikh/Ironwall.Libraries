using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.DataProviders;
using Ironwall.Libraries.Common.Models;
using Ironwall.Libraries.Common.Providers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Common.Proivders;
using Ironwall.Libraries.Tcp.Packets.Services;
using Ironwall.Libraries.Tcp.Server.Defines;
using Ironwall.Libraries.Tcp.Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Libraries.Tcp.Server.Services
{
    public abstract class TcpServer 
        : TcpSocket, ITcpServer
    {
        #region - Ctors -
        public TcpServer(
            TcpSetupModel tcpSetupModel
            , TcpServerSetupModel tcpServerSetupModel
            , LogProvider logProvider)
        {
            _tcpSetupModel = tcpSetupModel;
            _tcpServerSetupModel = tcpServerSetupModel;
            _logProvider = logProvider;
            _locker = new object();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void ConnectionTick(object sender, ElapsedEventArgs e)
        {
            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}][{nameof(TcpServer)}] TICK...");
        }


        protected abstract void SetupSessions();
        public override void InitSocket()
        {
            try
            {
                sb = new StringBuilder();
                ClientCount = 0;
                ClientList = new List<TcpAcceptedClient>();
                var log = InstanceFactory.Build<LogModel>();
                log.Info($"Server SetupModel => ( IP : {_tcpServerSetupModel.Ip}, Port : {_tcpServerSetupModel.Port.ToString()})");
                _logProvider.Add(log);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(_tcpServerSetupModel.Ip), Convert.ToInt32(_tcpServerSetupModel.Port));

                //Timer 초기화
                InitTimer();

                //Session 초기화
                SetupSessions();

                //Mode Prepared
                Mode = 1;

                CreateSocket(ipep);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(InitSocket)} : {ex.Message}");
            }
        }
        public override Task SendRequest(string msg, IPEndPoint endPoint = null)
        {
            lock (_locker)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        //연결된 클라이언트에게 메시지 브로드캐스팅
                        foreach (TcpAcceptedClient client in ClientList)
                        {
                            if (endPoint == null)
                            {
                                await client.SendRequest(msg);
                            }
                            else
                            {
                                if (client.Socket.RemoteEndPoint as IPEndPoint == endPoint)
                                    await client.SendRequest(msg, endPoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raised Exception in {nameof(SendRequest)} : {ex.Message}");
                    }
                });
            }
            return Task.CompletedTask;

        }

        public override Task SendFileRequest(string file, IPEndPoint endPoint = null)
        {
            lock (_locker)
            {
                Task.Run(async () =>
                {
                    try
                    {

                        //연결된 클라이언트에게 메시지 브로드캐스팅
                        foreach (TcpAcceptedClient client in ClientList)
                        {
                            if (endPoint == null)
                            {
                                await client.SendFileRequest(file);
                            }
                            else
                            {
                                if (client.Socket.RemoteEndPoint as IPEndPoint == endPoint)
                                    await client.SendFileRequest(file, endPoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raised Exception in {nameof(SendRequest)} : {ex.Message}");
                    }
                });
            }
            return Task.CompletedTask;
        }

        public override Task SendMapDataRequest(string file, IPEndPoint endPoint = null)
        {
            lock (_locker)
            {
                Task.Run(async () =>
                {
                    try
                    {

                        //연결된 클라이언트에게 메시지 브로드캐스팅
                        foreach (TcpAcceptedClient client in ClientList)
                        {
                            if (endPoint == null)
                            {
                                await client.SendMapDataRequest(file);
                            }
                            else
                            {
                                if (client.Socket.RemoteEndPoint as IPEndPoint == endPoint)
                                    await client.SendMapDataRequest(file, endPoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raised Exception in {nameof(SendMapDataRequest)} : {ex.Message}");
                    }
                });
            }
            return Task.CompletedTask;
        }

        public override Task SendProfileDataRequest(string file, IPEndPoint endPoint = null)
        {
            lock (_locker)
            {
                Task.Run(async () =>
                {
                    try
                    {

                        //연결된 클라이언트에게 메시지 브로드캐스팅
                        foreach (TcpAcceptedClient client in ClientList)
                        {
                            if (endPoint == null)
                            {
                                await client.SendProfileDataRequest(file);
                            }
                            else
                            {
                                if (client.Socket.RemoteEndPoint as IPEndPoint == endPoint)
                                    await client.SendProfileDataRequest(file, endPoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raised Exception in {nameof(SendProfileDataRequest)} : {ex.Message}");
                    }
                });
            }
            return Task.CompletedTask;
        }

        public override Task SendVideoDataRequest(string file, IPEndPoint endPoint = null)
        {
            lock (_locker)
            {
                Task.Run(async () =>
                {
                    try
                    {

                        //연결된 클라이언트에게 메시지 브로드캐스팅
                        foreach (TcpAcceptedClient client in ClientList)
                        {
                            if (endPoint == null)
                            {
                                await client.SendVideoDataRequest(file);
                            }
                            else
                            {
                                if (client.Socket.RemoteEndPoint as IPEndPoint == endPoint)
                                    await client.SendVideoDataRequest(file, endPoint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raised Exception in {nameof(SendVideoDataRequest)} : {ex.Message}");
                    }
                });
            }
            return Task.CompletedTask;
        }

        public override async void CloseSocket()
        {
            try
            {
                foreach (var client in ClientList.ToList())
                {
                    if (client.Socket != null && client.Socket.Connected)
                    {
                        await client.SendRequest($"Server was finished!", (IPEndPoint)client.Socket.RemoteEndPoint);
                        client.CloseSocket();

                        client.AcceptedClientConnected -= AcceptedClient_Conncted;
                        client.AcceptedClientEvent -= AcceptedClient_Event;
                        client.AcceptedClientDisconnected -= AcceptedClient_Disconnected;
                    }
                }

                if (_hearingEvent != null)
                    _hearingEvent.Completed -= new EventHandler<SocketAsyncEventArgs>(Accept_Completed);

                if (Socket.Connected)
                {
                    Socket.Disconnect(false);
                    Debug.WriteLine($"Server socket({Socket.GetHashCode()}) was disconnected in {nameof(CloseSocket)}");
                }
                ///Timer Task Dispose
                DisposeTimer();

                ///Socket Closed
                Socket.Close();
                Debug.WriteLine($"{nameof(TcpServer)} socket({Socket.GetHashCode()}) was closed in {nameof(CloseSocket)}");

                //Mode Created
                Mode = 0;
                Debug.WriteLine($"{nameof(TcpServer)} socket({Socket.GetHashCode()}) was disposed in {nameof(CloseSocket)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CloseSocket)} : {ex.Message}");
            }
        }
        protected void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                var ass = e.AcceptSocket;

                TcpAcceptedClient cliUser = InstanceFactory.Build<TcpAcceptedClient>();

                ClientActivate(cliUser);

                cliUser.CreateSocket(e.AcceptSocket);

                //클라이언트 리스트 등록
                ClientList.Add(cliUser);

                Socket socketServer = (Socket)sender;
                e.AcceptSocket = null;
                socketServer.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Accept_Completed)} : {ex.Message}");
            }
        }
        protected void AcceptedClient_Conncted(object cli)
        {
            try
            {
                TcpAcceptedClient client = (TcpAcceptedClient)cli;
                //client.SendRequest("Welcome!");
                ServerAccepted?.Invoke((IPEndPoint)client.Socket.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(AcceptedClient_Conncted)} : {ex.Message}");
            }
        }
        protected void AcceptedClient_Event(string data, IPEndPoint endPoint = null, EnumTcpCommunication type = EnumTcpCommunication.MSG_PACKET_RECEIVING)
        {
            ServerEvent?.Invoke(data, endPoint, type);
        }
        protected async void AcceptedClient_Disconnected(object cli)
        {
            await Task.Run(() => 
            { 
                try
                {
                    TcpAcceptedClient removeCli = (TcpAcceptedClient)cli;
                    var endPoint = (IPEndPoint)removeCli?.Socket?.RemoteEndPoint;
                    Debug.WriteLine($"EndPoint is {endPoint} in AcceptedClient_Disconnected of TcpServer");
                    
                    // 서버 연결 종료 이벤트 송신
                    ServerDisconnected?.Invoke(endPoint);
                    // 클라이언트 리스트에서 해당 소켓 삭제
                    ClientList.Remove(removeCli);
                    
                    ClientDeactivate(removeCli);
                    //Debug.WriteLine($"{nameof(AcceptedClient_Disconnected)} ClientList.Remove(removeCli)");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Rasied Exception in {nameof(AcceptedClient_Disconnected)} : {ex.Message}");
                }
            });
        }

        #endregion
        #region - Binding Methods -
        
        #endregion
        #region - Processes -
        private void CreateSocket(IPEndPoint ipep)
        {
            try
            {
                //소켓 생성
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //ipep = new IPEndPoint(IPAddress.Parse(SetupModel.Ip), Convert.ToInt32(SetupModel.ServerPort));

                var log = InstanceFactory.Build<LogModel>();
                log.Info($"Server Socket was created!( IP : {ipep.Address.ToString()}, Port : {ipep.Port.ToString()})");
                _logProvider.Add(log);
                Socket.Bind(ipep);
                Socket.Listen(5);

                //연결요청 확인 이벤트
                _hearingEvent = new SocketAsyncEventArgs();

                //이벤트 RemoteEndPoint 설정
                _hearingEvent.RemoteEndPoint = ipep;

                //연결 완료 이벤트 연결
                _hearingEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);

                //TaskTimer Start!
                SetTimerStart();

                //Mode Created
                Mode = 2;

                //서버 메시지 대기
                Socket.AcceptAsync(_hearingEvent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        public void ClientActivate(TcpAcceptedClient client)
        {
            try
            {
                client.UpdateHeartBeat(DateTime.Now + TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat));
                client.InitSocket();
                //이벤트 연결
                client.AcceptedClientConnected += AcceptedClient_Conncted;
                client.AcceptedClientEvent += AcceptedClient_Event;
                client.AcceptedClientDisconnected += AcceptedClient_Disconnected;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ClientDeactivate(TcpAcceptedClient client)
        {
            try
            {
                client.AcceptedClientConnected -= AcceptedClient_Conncted;
                client.AcceptedClientEvent -= AcceptedClient_Event;
                client.AcceptedClientDisconnected -= AcceptedClient_Disconnected;
                //client = null;
                //GC.Collect();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //연결된 클라이언트 수
        public int ClientCount { get; set; }
        //연결된 클라이언트 소켓 리스트
        public List<TcpAcceptedClient> ClientList { get; set; }
        public System.Timers.Timer refreshTimer { get; set; }
        #endregion
        #region - Attributes -
        public event ITcpCommon.TcpAccept_dele ServerAccepted;
        public event ITcpCommon.TcpEvent_dele ServerEvent;
        public event ITcpCommon.TcpDisconnect_dele ServerDisconnected;
        public TcpSetupModel _tcpSetupModel;
        public TcpServerSetupModel _tcpServerSetupModel;

        private LogProvider _logProvider;
        public SocketAsyncEventArgs _hearingEvent;
        private object _locker;
        #endregion
    }
}
