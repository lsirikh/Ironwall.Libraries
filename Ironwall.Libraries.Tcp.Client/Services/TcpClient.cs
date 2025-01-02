using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Settings;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Tcp.Common;
using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Packets.Models;
using Ironwall.Libraries.Tcp.Packets.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static Ironwall.Libraries.Tcp.Common.Defines.ITcpCommon;

namespace Ironwall.Libraries.Tcp.Client.Services
{
    public abstract class TcpClient : TcpSocket, ITcpClient, ITcpDataModel, IDisposable
    {
        #region - Ctors -
        protected TcpClient(ILogService log, TcpSetupModel tcpSetupModel)
        {
            _log = log;
            _tcpSetupModel = tcpSetupModel;
           
            _locker = new object();
            _settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.ff"
            };

            _class = typeof(TcpClient);
        }
        ~TcpClient()
        {
            Dispose(false);
        }

        #endregion
        #region - Implementation of Interface -
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 관리되는 리소스 해제
                    CloseSocket();
                }

                // 관리되지 않는 리소스 해제
                _disposed = true;
            }
        }
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
                _log.Error($"Raised Exception in {nameof(InitSocket)} : {ex.Message}");
            }
        }

        public override void CloseSocket()
        {
            try
            {
                if (Socket != null && Mode != 0)
                {
                    Mode = 0;

                    // 패킷 서비스 이벤트 제거
                    _packetService.Dispose();
                    _packetService.ReceiveStarted -= PacketService_ReceiveStarted;
                    _packetService.Receiving -= PacketService_Receiving;
                    _packetService.ReceiveCompleted -= PacketService_ReceiveCompleted;
                    _packetService.SendStarted -= _packetService_SendStarted;
                    _packetService.Sending -= _packetService_Sending;
                    _packetService.SendCompleted -= PacketService_SendCompleted;

                    // 타이머 정리
                    DisposeTimer();

                    Disconnected?.Invoke();

                    // 연결 관련 이벤트 제거
                    if (Socket.Connected)
                    {
                        var disconnectEvent = new SocketAsyncEventArgs();
                        disconnectEvent.Completed -= Connect_Completed;
                        disconnectEvent.Completed -= Recieve_Completed;
                        Socket?.DisconnectAsync(disconnectEvent);
                    }

                    Disconnect_Process();

                    //if (Socket.Connected)
                    //{
                    //    //Socket AsyncEvent for Disconnection
                    //    SocketAsyncEventArgs disconnectEvent = new SocketAsyncEventArgs();
                    //    Socket?.DisconnectAsync(disconnectEvent);

                    //    //When Complete Disconnection from Remote EndPoint Call a callback function
                    //    disconnectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Disconnect_Complete);
                    //}
                    //else
                    //{
                    //    Disconnect_Process();
                    //}
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(CloseSocket)} : {ex.Message}");
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
                _log.Error($"Raised Exception in {nameof(SendRequest)} : {ex.Message}");
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
                _log.Error($"Raised Exception in {nameof(SendFileRequest)} : {ex.Message}");
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
                _log.Error($"Raised Exception in {nameof(SendMapDataRequest)} : {ex.Message}");
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
                _log.Error($"Raised Exception in {nameof(SendProfileDataRequest)} : {ex.Message}");
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
                    _log.Error($"Raised Exception in {nameof(SendVideoDataRequest)} : {ex.Message}");
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
        private async void CreateSocket(IPEndPoint ipep)
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
                _packetService.ReceiveStarted += PacketService_ReceiveStarted;
                _packetService.Receiving += PacketService_Receiving;
                _packetService.ReceiveCompleted += PacketService_ReceiveCompleted;
                _packetService.SendStarted += _packetService_SendStarted;
                _packetService.Sending += _packetService_Sending;
                _packetService.SendCompleted += PacketService_SendCompleted;

                // 연결요청 확인 이벤트
                SocketAsyncEventArgs lookingEvent = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = ipep // 서버의 IP 및 포트 설정
                };

                //연결 완료 이벤트 연결
                lookingEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Connect_Completed);

                // 타임아웃 설정 (5초)
                var connectTask = ConnectAsyncWithTimeout(createdSocket, lookingEvent, TimeSpan.FromSeconds(5));
                await connectTask; // 연결 시도 및 타임아웃 처리

                ////서버 메시지 대기
                //createdSocket.ConnectAsync(lookingEvent);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(CreateSocket)} : {ex.Message}");
            }
        }

        private async Task ConnectAsyncWithTimeout(Socket socket, SocketAsyncEventArgs eventArgs, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    var connectTask = Task.Factory.FromAsync(
                        (callback, state) => socket.BeginConnect(eventArgs.RemoteEndPoint, callback, state),
                        socket.EndConnect,
                        null
                    );

                    var timeoutTask = Task.Delay(timeout, cts.Token);

                    if (await Task.WhenAny(connectTask, timeoutTask) == connectTask)
                    {
                        // 연결 성공 시 타임아웃 취소
                        cts.Cancel();
                        await connectTask; // 작업 완료

                        // 성공 시 Connect_Completed 로직 호출
                        Connect_Completed(socket, eventArgs);
                    }
                    else
                    {
                        // 타임아웃 발생
                        throw new TimeoutException("Socket connection timed out.");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(ConnectAsyncWithTimeout)} : {ex.Message}");
                    throw; // 예외를 호출자로 전달
                }
            }
        }


        private void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Socket Main Error Check 
            if (e.SocketError == SocketError.ConnectionRefused)
            {
                _log.Info("Server Connection was Refused!");
                TcpEvent?.Invoke("Server Connection was Refused!", null, EnumTcpCommunication.COMMUNICATION_ERROR);
                CloseSocket();

                return;
            }
            else if (e.SocketError == System.Net.Sockets.SocketError.AddressAlreadyInUse)
            {
                _log.Info("IP Address is already in use!");
                TcpEvent?.Invoke("IP Address is already in use!", null, EnumTcpCommunication.COMMUNICATION_ERROR);
                CloseSocket();

                return;
            }

            try
            {
                if (e.SocketError == SocketError.Success && sender is Socket socket && socket.Connected)
                {
                    Socket = socket; // 성공한 소켓 저장
                    var info = Socket.LocalEndPoint.ToString();
                    Mode = 2;

                    SetTimerStart();
                    HeartBeatExpireTime = DateTimeHelper.GetCurrentTimeWithoutMS() + TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat);

                    // 프록시 클라이언트가 미들웨어 연결됨을 이벤트로 알림
                    Connceted?.Invoke();

                    //접속 완료 상태 메세지 전송
                    //TcpEvent?.Invoke($"(Notice) Successfully connected to server!", (IPEndPoint)((Socket)sender).RemoteEndPoint);

                    // 메시지 수신 준비
                    var receiveEvent = new SocketAsyncEventArgs
                    {
                        UserToken = Socket
                    };
                    //버퍼
                    receiveEvent.SetBuffer(new byte[PacketHeader.TOTAL_SIZE], 0, PacketHeader.TOTAL_SIZE);
                    //받음 완료 이벤트 연결
                    receiveEvent.Completed += new EventHandler<SocketAsyncEventArgs>(Recieve_Completed);
                    //받음 보냄
                    Socket.ReceiveAsync(receiveEvent);

                    _log.Info("Socket connection successfully completed.");
                }
                else
                {
                    CloseSocket();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Connect_Completed)} : {ex.Message}");
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
                    _log.Info($"연결도 없고 받는 데이터도 없어서 끊김");
                    CloseSocket();
                }

                if (!socketClient.Connected)
                {
                    // Disconnected 이벤트 알림
                    //AcceptedClientDisconnected?.Invoke(this);
                    _log.Info($"연결 끊겨서 종료");
                    CloseSocket();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Recieve_Completed)} : {ex.Message}");
            }
        }

        private void PacketService_ReceiveStarted(object ret, EnumTcpCommunication type)
        {
            //var msg = ret?.ToString() != null ? ret.ToString() : $"수신 시작";
            TcpEvent?.Invoke(ret?.ToString(), _remoteEP, type);
        }

        private void PacketService_Receiving(int current, int total, EnumTcpCommunication type, string name = null)
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
                _log.Error($"Raised Exception in {nameof(Disconnect_Complete)} : {ex.Message}");
            }
        }

        private void Disconnect_Process()
        {
            Mode = 0;

            if (Socket != null)
            {
                _log.Info($"Socket({Socket.GetHashCode()}) is being closed.");
                Socket.Close();
                Socket.Dispose();
                _log.Info($"Socket({Socket.GetHashCode()}) was successfully disposed.");
            }
        }

        public void SetServerIPEndPoint(ITcpServerModel model)
        {
            ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(model.IpAddress), Convert.ToInt32(model.Port));
        }

        protected override async void ConnectionTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                bool isHeartBeatSent = false;
                if ((HeartBeatExpireTime - DateTimeHelper.GetCurrentTimeWithoutMS()) < TimeSpan.FromSeconds(10))
                {
                    _log.Info($"##############Start TCP HeartBeat!#############");
                    isHeartBeatSent = await SendHeartBeat();
                    _log.Info($"#############Finish TCP HeartBeat!#############");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(ConnectionTick)} : {ex.Message}");
            }
        }

        public void UpdateHeartBeat(string updatedTime)
        {
            try
            {
                DateTime timeData;
                DateTime.TryParse(updatedTime, out timeData);
                HeartBeatExpireTime = timeData;
                _log.Info($"Tcp client HeartBeat was update[Expired : {HeartBeatExpireTime.ToString("yyyy-MM-dd HH:mm:ss")}]");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(UpdateHeartBeat)} : {ex.Message}");
            }
        }

        private async Task<bool> SendHeartBeat()
        {
            try
            {
                if (!Socket.Connected)
                {
                    _log.Info($"Socket is connected :{Socket.Connected} in {nameof(SendHeartBeat)}");
                    return false;
                }

                var iPEndPoint = Socket.LocalEndPoint as IPEndPoint;
                var requestModel = new HeartBeatRequestModel(iPEndPoint.Address.ToString(), iPEndPoint.Port);
                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg);
                //HeartBeatExpireTime += TimeSpan.FromSeconds(_tcpSetupModel.HeartBeat);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(SendHeartBeat)} : {ex.Message}");
                return false;
            }
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
        protected JsonSerializerSettings _settings;
        private Type _class;
        private bool _disposed = false;

        protected ILogService _log;

        //private bool inReceiveProcess;
        public TcpSetupModel _tcpSetupModel;
        private PacketService _packetService;
        private IPEndPoint _remoteEP;
        #endregion
    }
}
