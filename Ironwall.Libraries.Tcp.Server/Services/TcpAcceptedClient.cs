using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Tcp.Common;
using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Packets.Models;
using Ironwall.Libraries.Tcp.Packets.Services;
using Ironwall.Libraries.Tcp.Server.Defines;
using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Libraries.Tcp.Server.Services
{
    public class TcpAcceptedClient 
        : TcpSocket, ITcpAcceptedClient
    {
        #region - Ctors -
        public TcpAcceptedClient()
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void InitSocket()
        {
            try
            {
                sb = new StringBuilder();
                _locker = new object();
                //Timer 초기화
                InitTimer();

                Mode = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(InitSocket)} : {ex.Message}");
            }
        }

        public override void CloseSocket()
        {
            try
            {
                Debug.WriteLine("TcpAcceptedClient CloseSocket was called");

                if (Socket != null && Mode != 0)
                {
                    /*if (_receiveEvent != null)
                        _receiveEvent.Completed -= new EventHandler<SocketAsyncEventArgs>(Recieve_Completed);*/
                    
                    _packetService.Dispose();
                    _packetService.ReceiveStarted -= _packetService_ReceiveStarted;
                    _packetService.Receiving -= _packetService_Receiving;
                    _packetService.ReceiveCompleted -= PacketService_ReceiveCompleted;
                    _packetService.SendStarted -= _packetService_SendStarted;
                    _packetService.Sending -= _packetService_Sending;
                    _packetService.SendCompleted -= _packetService_SendCompleted;

                    DisposeTimer();
                    AcceptedClientDisconnected?.Invoke(this);

                    if (Socket.Connected)
                    {
                        //Socket AsyncEvent for Disconnection
                        SocketAsyncEventArgs disconnectEvent = new SocketAsyncEventArgs();
                        Socket.DisconnectAsync(disconnectEvent);

                        //When Complete Disconnection from Remote EndPoint Call a callback function
                        disconnectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Disconnect_Complete);
                    }
                    else
                    {
                        Disconnect_Process();
                    }
                    #region Deprecated
                    /*else
                    {
                        //Socket Close to finish using socket
                        Socket?.Close();
                        Debug.WriteLine($"{nameof(TcpAcceptedClient)} socket({Socket?.GetHashCode()}) was closed in {nameof(CloseSocket)}");
                        //Socket Dispose to release resources
                        Socket?.Dispose();
                        Debug.WriteLine($"{nameof(TcpAcceptedClient)} socket({Socket?.GetHashCode()}) was disposed in {nameof(CloseSocket)}");
                    }*/
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CloseSocket)} : {ex.Message}");
            }
        }

        public override async Task SendRequest(string msg, IPEndPoint selectedIp = null)
        {
            try
            {
                var byteArray = Encoding.UTF8.GetBytes(msg);

                if (selectedIp != null)
                    _remoteEP = selectedIp;

                if (byteArray.Length > PacketHeader.BODY_SIZE)
                {
                    await _packetService.SendPacketProcess(msg, Packets.Utils.EnumPacketType.LONG_MESSAGE, Socket);
                }
                else
                {
                    await _packetService.SendPacketProcess(msg, Packets.Utils.EnumPacketType.SHORT_MESSAGE, Socket);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(SendRequest)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        public override async Task SendFileRequest(string file, IPEndPoint selectedIp = null)
        {
            try
            {
                if (selectedIp != null)
                    _remoteEP = selectedIp;

                await _packetService.SendPacketProcess(file, Packets.Utils.EnumPacketType.FILE, Socket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(SendRequest)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        public override async Task SendMapDataRequest(string file, IPEndPoint selectedIp = null)
        {
            try
            {
                if (selectedIp != null)
                    _remoteEP = selectedIp;

                await _packetService.SendPacketProcess(file, Packets.Utils.EnumPacketType.MAP, Socket);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(SendMapDataRequest)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        public override async Task SendProfileDataRequest(string file, IPEndPoint selectedIp = null)
        {
            try
            {
                if (selectedIp != null)
                    _remoteEP = selectedIp;

                await _packetService.SendPacketProcess(file, Packets.Utils.EnumPacketType.PROFILE, Socket);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(SendProfileDataRequest)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        public override async Task SendVideoDataRequest(string file, IPEndPoint selectedIp = null)
        {
            try
            {
                if (selectedIp != null)
                    _remoteEP = selectedIp;

                await _packetService.SendPacketProcess(file, Packets.Utils.EnumPacketType.VIDEO, Socket);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(SendVideoDataRequest)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        private void _packetService_SendStarted(object ret, EnumTcpCommunication type)
        {
            //var msg = ret.ToString() != null ? $"송신 시작 : {ret.ToString()}" : $"송신 시작";
            AcceptedClientEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        private void _packetService_Sending(int current, int total, EnumTcpCommunication type, string name = null)
        {
            var msg = name != null ? $"송신중 : {name}({Math.Round((double)current / total * 100, 2)}%)" : $"송신중 : {Math.Round((double)current / total * 100, 2)}%";
            AcceptedClientEvent?.Invoke(msg, _remoteEP, EnumTcpCommunication.MSG_PACKET_SENDING);
        }

        private void _packetService_SendCompleted(object ret, EnumTcpCommunication type)
        {
            //var msg = ret.ToString() != null ? $"송신 완료 : {ret.ToString()}" : $"송신 완료";
            AcceptedClientEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        protected override void ConnectionTick(object sender, ElapsedEventArgs e)
        {
            //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}][{nameof(TcpAcceptedClient)}] TICK...{(HeartBeatExpireTime - DateTime.Now).Seconds}");
            //Debug.WriteLine($"[{HeartBeatExpireTime.ToString("yyyy-MM-dd HH:mm:ss.ff")} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]");
            //DateTime.Now - DateTime.Parse(sessionModel.TimeExpired) > TimeSpan.Zero
            if ((DateTime.Now - HeartBeatExpireTime) > TimeSpan.Zero )
            {
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]****Heartbeat time was expired!****");
                Debug.WriteLine($"heartbeat : {HeartBeatExpireTime}");
                CloseSocket();
            }

            ///if(HBTime > CurrentTime)
            /// CloseSocket()
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        public void CreateSocket(Socket socketClient)
        {
            try
            {
                //전달받은 소켓 전역으로 활용
                Socket = socketClient;

                Mode = 1;
                //HeartBeatExpireTime = DateTime.Now + TimeSpan.FromSeconds(20);
                SetTimerInterval(1000);
                SetTimerStart();


                Socket.LingerState = new LingerOption(true, 1);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                //PacketService 설정
                _packetService = new PacketService();
                _packetService.SendStarted += _packetService_SendStarted;
                _packetService.Sending += _packetService_Sending;
                _packetService.SendCompleted += _packetService_SendCompleted;
                _packetService.ReceiveStarted += _packetService_ReceiveStarted;
                _packetService.Receiving += _packetService_Receiving;
                _packetService.ReceiveCompleted += PacketService_ReceiveCompleted;

                // 서버에 보낼 객체를 만든다.
                var _receiveEvent = new SocketAsyncEventArgs();
                //보낼 데이터를 설정하고
                _receiveEvent.UserToken = Socket;
                //receiveEvent.UserToken = e.ConnectSocket;

                //ID 설정
                ClientAddress = $"{((IPEndPoint)Socket.RemoteEndPoint)?.Address.ToString()}:{((IPEndPoint)Socket.RemoteEndPoint)?.Port.ToString()}";

                //데이터 길이 세팅
                _receiveEvent.SetBuffer(new byte[PacketHeader.TOTAL_SIZE], 0, PacketHeader.TOTAL_SIZE);
                //받음 완료 이벤트 연결
                _receiveEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve_Completed);
                //클라이언트 연결 후, 호출한 서버클래스 내부 작업 요청
                AcceptedClientConnected?.Invoke(this);

                //받음 보냄
                Socket.ReceiveAsync(_receiveEvent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CreateSocket)} : {ex.Message}");
            }
        }

        
        public void UpdateHeartBeat(DateTime dateTime)
        {
            HeartBeatExpireTime = dateTime;
            Debug.WriteLine($"<<<<<<<<<<<<UpdateHeartBeat : {HeartBeatExpireTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}>>>>>>>>>>>>>");
        }

        private async void Recieve_Completed(object sender, SocketAsyncEventArgs e)
        {
            //서버에서 넘어온 정보
            Socket socketClient = (Socket)sender;

            try
            {
                // 접속이 연결되어 있으면...
                if (socketClient.Connected && e.BytesTransferred > 0)
                {
                    _remoteEP = (IPEndPoint)socketClient?.RemoteEndPoint;

                    try
                    {
                        _ = _packetService.ReceivedPacketProcess(e.Buffer);
                        await Task.Delay(10);
                    }
                    catch
                    {
                    }
                    
                    byte[] data = e.Buffer;
                    // 메모리 버퍼를 초기화 한다. 크기는 4096이다.
                    e.SetBuffer(new byte[data.Length], 0, data.Length);

                    // 메시지가 오면 이벤트를 발생시킨다. (IOCP로 넣는 것)
                    socketClient.ReceiveAsync(e);
                }
                else 
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]연결도 없고 받는 데이터도 없어서 끊김");
                    CloseSocket();
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Recieve_Completed)} : {ex.Message}");
            }
        }
       
        private void _packetService_ReceiveStarted(object ret, EnumTcpCommunication type)
        {
           // var msg = ret?.ToString() != null ? $"수신 시작 : {ret.ToString()}" : $"수신 시작";
            AcceptedClientEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        private void _packetService_Receiving(int current, int total, EnumTcpCommunication type, string name = null)
        {
            string msg;
            if (total == 0)
                msg = $"Receving...";
            else
                msg = name != null ? $"Receving a packet : {name}({Math.Round((double)current / total * 100, 1)}%)" : $"Receving a packet : {Math.Round((double)current / total * 100, 1)}%";

            AcceptedClientEvent?.Invoke(msg, _remoteEP, EnumTcpCommunication.MSG_PACKET_RECEIVING);
        }

        private void PacketService_ReceiveCompleted(object ret, EnumTcpCommunication type)
        {
            //수신시 확인
            //var msg = ret?.ToString() != null ? ret.ToString() : $"수신 완료";
            AcceptedClientEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        private void Disconnect_Complete(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (!((Socket)sender).Connected)
                {
                    Disconnect_Process();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Disconnect_Complete)} : {ex.Message}");
            }
        }

        private void Disconnect_Process()
        {
            Mode = 0;


            Debug.WriteLine($"{nameof(TcpAcceptedClient)} socket({Socket.GetHashCode()}) was disconnected in {nameof(Disconnect_Complete)}");
            //Socket Close to finish using socket
            Socket?.Close();
            Debug.WriteLine($"{nameof(TcpAcceptedClient)} socket({Socket.GetHashCode()}) was closed in {nameof(Disconnect_Complete)}");
            //Socket Dispose to release resources
            Socket?.Dispose();
            Debug.WriteLine($"{nameof(TcpAcceptedClient)} socket({Socket.GetHashCode()}) was disposed in {nameof(Disconnect_Complete)}");
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        /// <summary>
        /// 이 유저의 아이디
        /// </summary>
        public string ClientAddress { get; set; }
        public DateTime HeartBeatExpireTime { get; set; }
        public PacketService _packetService { get; set; }
        #endregion
        #region - Attributes -
        public event ITcpCommon.TcpCliAccept_dele AcceptedClientConnected;
        public event ITcpCommon.TcpEvent_dele AcceptedClientEvent;
        public event ITcpCommon.TcpCliDiscon_dele AcceptedClientDisconnected;
        //private SocketAsyncEventArgs _receiveEvent;
        private object _locker;
        private IPEndPoint _remoteEP;
        #endregion
    }
}
