using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Settings;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Tcp.Common;
using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Packets.Models;
using Ironwall.Libraries.Tcp.Packets.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Ironwall.Libraries.Tcp.Common.Defines.ITcpCommon;

namespace Ironwall.Libraries.Tcp.Client.Services
{
    public abstract class TcpClient
        : TcpSocket, ITcpClient, ITcpDataModel
    {
        #region - Ctors -
        public TcpClient()
        {

        }
        public TcpClient(TcpSetupModel tcpSetupModel)
        {
            _tcpSetupModel = tcpSetupModel;
           
            _locker = new object();
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

                if (ServerIPEndPoint == null)
                    return;

                //When User is Login, execute!
                InitTimer();

                //Prepare
                Mode = 0;
                //CloseSocket();

                CreateSocket(ServerIPEndPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(InitSocket)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

        public override void CloseSocket()
        {
            try
            {
                if (Socket != null && Mode != 0)
                {
                    Mode = 0;

                    _packetService.Dispose();
                    _packetService.ReceiveStarted -= _packetService_ReceiveStarted;
                    _packetService.Receiving -= _packetService_Receiving;
                    _packetService.ReceiveCompleted -= PacketService_ReceiveCompleted;
                    _packetService.SendStarted -= _packetService_SendStarted;
                    _packetService.Sending -= _packetService_Sending;
                    _packetService.SendCompleted -= PacketService_SendCompleted;

                    DisposeTimer();
                    Disconnected?.Invoke();
                    if (Socket.Connected)
                    {
                        //Socket AsyncEvent for Disconnection
                        SocketAsyncEventArgs disconnectEvent = new SocketAsyncEventArgs();
                        Socket?.DisconnectAsync(disconnectEvent);

                        //When Complete Disconnection from Remote EndPoint Call a callback function
                        disconnectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Disconnect_Complete);
                    }
                    else
                    {
                        Disconnect_Process();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CloseSocket)} of {nameof(TcpClient)} : {ex.Message}");
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
                Debug.WriteLine($"Raised Exception in {nameof(SendFileRequest)} of {nameof(TcpClient)} : {ex.Message}");
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

        public override Task SendVideoDataRequest(string file, IPEndPoint selectedIp = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (selectedIp != null)
                        _remoteEP = selectedIp;

                    _packetService.SendPacketProcess(file, Packets.Utils.EnumPacketType.VIDEO, Socket);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SendVideoDataRequest)} of {nameof(TcpClient)} : {ex.Message}");
                }
            });
        }

        private void _packetService_SendStarted(object ret, EnumTcpCommunication type)
        {
            //var msg = ret?.ToString() != null ? $"송신 시작 : {ret.ToString()}" : $"송신 시작";
            TcpEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }
        
        private void _packetService_Sending(int current, int total, EnumTcpCommunication type, string name = null)
        {
            string msg;
            if(total == 0)
                msg = $"송신중";
            else
                msg = name != null ? $"송신중 : {name}({Math.Round((double)current / total * 100, 2)}%)" : $"송신중 : {Math.Round((double)current / total * 100, 2)}%";

            TcpEvent?.Invoke(msg, _remoteEP, EnumTcpCommunication.MSG_PACKET_SENDING);
        }

        private void PacketService_SendCompleted(object ret, EnumTcpCommunication type)
        {
            //var msg = ret?.ToString() != null ? $"송신 완료 : {ret.ToString()}" : $"송신 완료";
            TcpEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        protected void SocketReceived(string data, IPEndPoint endPoint)
        {
            TcpEvent?.Invoke(data, endPoint, EnumTcpCommunication.MSG_PACKET_RECEIVING);
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
                Socket createdSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                createdSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                createdSocket.LingerState = new LingerOption(true, 1);
                
                //var endPoint = new IPEndPoint(IPAddress.Parse(SetupModel.ClientIp), SetupModel.ClientPort);
                //createdSocket.Bind(endPoint);

                Mode = 1;

                //PacketService 생성
                _packetService = new PacketService();
                _packetService.ReceiveStarted += _packetService_ReceiveStarted;
                _packetService.Receiving += _packetService_Receiving;
                _packetService.ReceiveCompleted += PacketService_ReceiveCompleted;
                _packetService.SendStarted += _packetService_SendStarted;
                _packetService.Sending += _packetService_Sending;
                _packetService.SendCompleted += PacketService_SendCompleted;

                //연결요청 확인 이벤트
                SocketAsyncEventArgs lookingEvent = new SocketAsyncEventArgs();

                //이벤트 RemoteEndPoint 설정
                lookingEvent.RemoteEndPoint = ipep;

                //연결 완료 이벤트 연결
                lookingEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Connect_Completed);

                //서버 메시지 대기
                createdSocket.ConnectAsync(lookingEvent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CreateSocket)} of {nameof(TcpClient)} : {ex.Message}");
            }
        }

       

        private void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Socket Main Error Check 
            if (e.SocketError == System.Net.Sockets.SocketError.ConnectionRefused)
            {
                Debug.WriteLine("Server Connection was Refused!");
                TcpEvent?.Invoke("Server Connection was Refused!", null, EnumTcpCommunication.COMMUNICATION_ERROR);
                CloseSocket();

                return;
            }
            else if (e.SocketError == System.Net.Sockets.SocketError.AddressAlreadyInUse)
            {
                Debug.WriteLine("IP Address is already in use!");
                TcpEvent?.Invoke("IP Address is already in use!", null, EnumTcpCommunication.COMMUNICATION_ERROR);
                CloseSocket();

                return;
            }

            try
            {
                if (true == ((Socket)sender).Connected)
                {
                    Socket = e.ConnectSocket;
                    var info = Socket.LocalEndPoint.ToString();
                    Mode = 2;

                    SetTimerStart();
                    HeartBeatExpireTime = DateTimeHelper.GetCurrentTimeWithoutMS() + TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat);

                    // 프록시 클라이언트가 미들웨어 연결됨을 이벤트로 알림
                    Connceted?.Invoke();

                    //접속 완료 상태 메세지 전송
                    //TcpEvent?.Invoke($"(Notice) Successfully connected to server!", (IPEndPoint)((Socket)sender).RemoteEndPoint);

                    //서버에 보낼 객체를 만든다.
                    SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();
                    //보낼 데이터를 설정하고
                    receiveEvent.UserToken = Socket;
                    //데이터 길이 세팅
                    //버퍼
                    receiveEvent.SetBuffer(new byte[PacketHeader.TOTAL_SIZE], 0, PacketHeader.TOTAL_SIZE);
                    //받음 완료 이벤트 연결
                    receiveEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve_Completed);
                    //받음 보냄
                    Socket.ReceiveAsync(receiveEvent);
                }
                else
                {
                    CloseSocket();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Connect_Completed)} of {nameof(TcpClient)} : {ex.Message}");
            }

        }
                

        private async void Recieve_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                //서버에서 넘어온 정보
                Socket socketClient = (Socket)sender;

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

                    // 수신 데이터는 e.Buffer에 있다.
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

                if (!socketClient.Connected)
                {
                    // Disconnected 이벤트 알림
                    //AcceptedClientDisconnected?.Invoke(this);
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]연결 끊겨서 종료!!!!!!!!!!!!!!!!!");
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
            //var msg = ret?.ToString() != null ? ret.ToString() : $"수신 시작";
            TcpEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        private void _packetService_Receiving(int current, int total, EnumTcpCommunication type, string name = null)
        {
            string msg;
            if (total == 0)
                msg = $"Receving...";
            else
                msg = name != null ? $"Receving a packet : {name}({Math.Round((double)current / total * 100, 1)}%)" : $"Receving a packet : {Math.Round((double)current / total * 100, 1)}%";

            TcpEvent?.Invoke(msg, _remoteEP, EnumTcpCommunication.MSG_PACKET_RECEIVING);
        }

        private void PacketService_ReceiveCompleted(object ret, EnumTcpCommunication type)
        {
            //수신시 확인
            //var msg = ret?.ToString() != null ? ret.ToString() : $"수신 완료";
            TcpEvent?.Invoke(ret?.ToString(), _remoteEP, type);
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

            Debug.WriteLine($"{nameof(TcpClient)} socket({Socket.GetHashCode()}) was disconnected in {nameof(Disconnect_Complete)}");
            //Socket Close to finish using socket
            Socket?.Close();
            Debug.WriteLine($"{nameof(TcpClient)} socket({Socket.GetHashCode()}) was closed in {nameof(Disconnect_Complete)}");
            //Socket Dispose to release resources
            Socket?.Dispose();
            Debug.WriteLine($"{nameof(TcpClient)} socket({Socket.GetHashCode()}) was disposed in {nameof(Disconnect_Complete)}");
        }

        public void SetServerIPEndPoint(ITcpServerModel model)
        {
            ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(model.IpAddress), Convert.ToInt32(model.Port));
        }

        protected override async void ConnectionTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}][{nameof(ConnectionTick)}] TICK...{(HeartBeatExpireTime - DateTime.Now).Seconds}");
                //Debug.WriteLine($"[{HeartBeatExpireTime.ToString("yyyy-MM-dd HH:mm:ss.ff")} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]");
                bool isHeartBeatSent = false;
                if ((HeartBeatExpireTime - DateTimeHelper.GetCurrentTimeWithoutMS()) < TimeSpan.FromSeconds(5))
                {
                    isHeartBeatSent = await SendHeartBeat();
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]UpdateHeartBeat!!!!! result : {isHeartBeatSent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(ConnectionTick)} : {ex.Message}");
            }
        }

        public void UpdateHeartBeat(string updatedTime)
        {
            try
            {
                DateTime timeData;
                DateTime.TryParse(updatedTime, out timeData);
                HeartBeatExpireTime = timeData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}");
            }
        }

        private Task<bool> SendHeartBeat()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!Socket.Connected)
                    {
                        Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Socket is connected :{Socket.Connected} in {nameof(SendHeartBeat)}");
                        return false;
                    }

                    var iPEndPoint = Socket.LocalEndPoint as IPEndPoint;
                    var requestModel = RequestFactory.Build<HeartBeatRequestModel>(iPEndPoint.Address.ToString(), iPEndPoint.Port);
                    var msg = JsonConvert.SerializeObject(requestModel);
                    await SendRequest(msg);
                    //HeartBeatExpireTime += TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(SendHeartBeat)} : {ex.Message}");
                    return false;
                }
            });
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IPEndPoint ServerIPEndPoint { get; private set; }
        public DateTime HeartBeatExpireTime { get; set; }
        public EnumTcpStateType EnumTcpStateType { get; set; }
        public EnumTcpDataType EnumTcpDataType { get; set; }
        public byte[] Binary { get; set ; }
        public byte[] MetaData { get; set ; }
        #endregion
        #region - Attributes -
        // TCP 인터페이스 이벤트
        public MemoryStream TempMemory;
        public int index;
        public event TcpConnect_dele Connceted;
        public event TcpEvent_dele TcpEvent;
        public event TcpDisconnect_dele Disconnected;
        private object _locker;

        //private bool inReceiveProcess;
        public TcpSetupModel _tcpSetupModel;
        private PacketService _packetService;
        private IPEndPoint _remoteEP;
        #endregion
    }
}
