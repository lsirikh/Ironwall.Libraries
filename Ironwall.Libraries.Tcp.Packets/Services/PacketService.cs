using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;
using Ironwall.Libraries.Tcp.Packets.Models;
using Ironwall.Libraries.Tcp.Packets.Utils;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Tcp.Packets.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/18/2023 10:16:44 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PacketService : IDisposable
    {

        #region - Ctors -
        public PacketService()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _dir = _currentDirectory + "/SendFolder";

            PackQueue = new Dictionary<string, PacketCollectionClass<PacketClass>>();
            _syncLock = new object();
        }

        public PacketService(ILogService log) : this()
        {
            //_currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //_dir = _currentDirectory + "/SendFolder";

            //PackQueue = new Dictionary<string, PacketCollectionClass<PacketClass>>();
            //_syncLock = new object();

            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        public void Dispose()
        {
            PackQueue.Clear();
            _currentDirectory = null;
            _dir = null;
            _syncLock = null;

            GC.Collect();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public List<PacketClass> CreateMessagePacket(string msg, EnumPacketType type)
        {
            List<PacketClass> packets = new List<PacketClass>();

            var bytes = Encoding.UTF8.GetBytes(msg);

            /// Id 설정
            var rand = new Random();
            ushort id = (ushort)rand.Next(MAX_NUM);
            /// 전체 Full Body Packet 수량 
            /// 0부터 시작하므로 몫 - 1
            ushort num = (ushort)((bytes.Length / PacketClass.BODY_SIZE) - 1);
            /// 나머지는 Nonfull Body Packet 1개
            var remain = bytes.Length % PacketClass.BODY_SIZE;
            /// 결론
            /// 나머지 != 0이면, num = (몫 - 1 ) + 1
            if (remain > 0)
                num++;

            PacketClass packet = new PacketClass();

            if (num > 0)
            {
                //FullBody PacketClass 생성
                for (ushort i = 0; i < num; i++)
                {
                    packet = new PacketClass()
                    {
                        Id = id,
                        CurrentSequence = i,
                        TotalSequence = num,
                        DataType = (byte)EnumPacketType.LONG_MESSAGE, // Lager Message
                                                                      // BodyLength = PacketClass.BODY_SIZE,
                    };

                    _log.Info($"[{packet.CurrentSequence}/{packet.TotalSequence}]{bytes.Length - PacketClass.BODY_SIZE * i} > {PacketClass.BODY_SIZE}");
                    packet.BodyLength = PacketClass.BODY_SIZE;
                    Buffer.BlockCopy(bytes, PacketClass.BODY_SIZE * i, packet.Body, 0, PacketClass.BODY_SIZE);

                    packet.CreateCRC();
                    packets.Add(packet);
                }

                packet = new PacketClass()
                {
                    Id = id,
                    CurrentSequence = num,
                    TotalSequence = num,
                    DataType = (byte)EnumPacketType.LONG_MESSAGE, // Lager Message
                };

                _log.Info($"[{packet.CurrentSequence}/{packet.TotalSequence}]{bytes.Length - PacketClass.BODY_SIZE * num} < {PacketClass.BODY_SIZE}");
                packet.BodyLength = (ushort)(bytes.Length - PacketClass.BODY_SIZE * num);
                Buffer.BlockCopy(bytes, PacketClass.BODY_SIZE * num, packet.Body, 0, packet.BodyLength);

                packet.CreateCRC();
                packets.Add(packet);
            }
            else
            {
                packet = new PacketClass
                {
                    Id = id,
                    CurrentSequence = 0,
                    TotalSequence = 0,
                    DataType = (byte)EnumPacketType.SHORT_MESSAGE, //Short Message
                    BodyLength = (ushort)bytes.Length,
                    Body = bytes,
                };

                packet.CreateCRC();
                packets.Add(packet);
            }

            return packets;
        }

        //public List<PacketClass> CreateFilePacket(string folderPath, string fileName)
        public List<PacketClass> CreateFilePacket(string file, EnumPacketType type)
        {
            List<PacketClass> packets = new List<PacketClass>();

            FileInfo fileInfo = new FileInfo(file);

            if (!fileInfo.Exists) throw new FileNotFoundException($"File not found: {file}");

            _log.Info($"=============================================================");
            _log.Info($"파일 위치: {fileInfo.FullName}");
            _log.Info($"파일 정보: {fileInfo.Name}({fileInfo.Length})");
            _log.Info($"=============================================================");

            var rand = new Random();
            ushort id = (ushort)rand.Next(MAX_NUM);

            using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {


                int bytesRead;
                long totalBytesRead = 0L;
                long remainingBytes = 0L;

                /// 1. Body에 할당할 버퍼를 잡는다.
                /// 2. PacketClass에 할당할 MetaData(Header) 정보를 셋업한다.
                /// 3. PacketClass 총 몇 묶음 나오는지 계산한다.
                /// 4. 나누고 나머지가 있으면 num + 1을 해준다.
                /// 5. 순환문을 돌면서 아래와 같은 프로세스를 처리한다.
                ///     5-1. 파일 스트림으로 PacketClass.BODY_SIZE 만큼 읽는다.
                ///     5-2. 읽어들인 데이터를 



                ushort num = (ushort)(fileStream.Length / PacketClass.BODY_SIZE);
                var remain = (fileStream.Length % PacketClass.BODY_SIZE);


                /// if (remain > 0)
                ///     num++;

                var packet = new PacketClass();

                if (num > 0)
                {
                    for (int i = 0; i < num; i++)
                    {
                        byte[] buffer = new byte[PacketClass.BODY_SIZE];
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);


                        packet = new PacketClass()
                        {
                            Id = id,
                            CurrentSequence = (ushort)i,
                            TotalSequence = num,
                            DataType = (byte)type,
                            FileName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                            FileExtension = Path.GetExtension(fileInfo.FullName),

                            BodyLength = PacketClass.BODY_SIZE,
                            Body = buffer,
                        };

                        packet.CreateCRC();
                        packets.Add(packet);

                        totalBytesRead += bytesRead;
                        remainingBytes = fileStream.Length - totalBytesRead;
                        _log.Info($"[PREPARING]남아있는 바이트 수: {remainingBytes} Byte({i}/{num})");
                    }

                    byte[] remainBuffer = new byte[remainingBytes];
                    bytesRead = fileStream.Read(remainBuffer, 0, (int)remainingBytes);

                    packet = new PacketClass()
                    {
                        Id = id,
                        CurrentSequence = (ushort)num,
                        TotalSequence = num,
                        DataType = (byte)type, // Lager Message
                        FileName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                        FileExtension = Path.GetExtension(fileInfo.FullName),

                        BodyLength = (ushort)remainingBytes,
                        Body = remainBuffer,
                    };

                    packet.CreateCRC();
                    packets.Add(packet);

                    totalBytesRead += bytesRead;
                    remainingBytes = fileStream.Length - totalBytesRead;
                    _log.Info($"[PREPARING]남아있는 바이트 수: {remainingBytes} Byte({num}/{num})");
                }
                else
                {
                    byte[] buffer = new byte[fileStream.Length];
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                    packet = new PacketClass
                    {
                        Id = 1,
                        CurrentSequence = 0,
                        TotalSequence = 0,
                        DataType = (byte)type, //Short Message
                        BodyLength = (ushort)fileStream.Length,
                        Body = buffer,
                    };

                    packet.CreateCRC();
                    packets.Add(packet);
                }
            }

            return packets;
        }

        public PacketClass ValidateCRC(byte[] receivedMsg)
        {
            var packet = new PacketClass(receivedMsg);
            ushort receivedCRC = CRC16.ComputeChecksum(receivedMsg, 0, PacketClass.TOTAL_SIZE - PacketClass.CRC_SIZE);

            if (packet.CRC != receivedCRC)
                throw new Exception($"packet CRC({packet.CRC}) is not the same as Received Message(byte array)\'s({receivedCRC}).");

            return packet;
        }

        public async Task SendPacketProcess(string sendMsg, EnumPacketType type, Socket socket)
        {
            try
            {
                await Task.Yield(); // CPU 잠금 방지

                lock (_syncLock)
                {
                    switch (type)
                    {
                        case EnumPacketType.NONE:
                            break;
                        case EnumPacketType.SHORT_MESSAGE:
                            {
                                //Short Message Without Status Event
                                var packList = CreateMessagePacket(sendMsg, type);
                                var arratItem = packList.FirstOrDefault()?.ToArray();

                                SendStarted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_SEND_BEGINNING);
                                if (arratItem != null)
                                {
                                    SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs
                                    {
                                        RemoteEndPoint = socket.RemoteEndPoint,
                                        UserToken = null,
                                        AcceptSocket = null
                                    };

                                    socketEventArgs.SetBuffer(arratItem, 0, arratItem.Length);

                                    socketEventArgs.Completed += (sender, e) =>
                                    {
                                        if (e.SocketError != SocketError.Success)
                                        {
                                            _log.Info($"Failed to send data: {e.SocketError}");
                                        }
                                        else
                                        {
                                            SendCompleted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_SEND_COMPLETE);
                                        }
                                    };

                                    // Client로 메시지 전송(비동기식)
                                    if (!socket.SendAsync(socketEventArgs))
                                    {
                                        Sending?.Invoke(0, 0, EnumTcpCommunication.MSG_PACKET_SENDING);
                                    }
                                    //socket?.Send(arratItem);
                                }
                            }
                            break;
                        case EnumPacketType.LONG_MESSAGE:
                            {
                                var packList = CreateMessagePacket(sendMsg, type);
                                if (packList.Count > 1)
                                {
                                    List<byte[]> byteList = new List<byte[]>();
                                    long transferByte = 0L;
                                    SendStarted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_SEND_BEGINNING);
                                    foreach (var item in packList)
                                    {
                                        transferByte += item.BodyLength;
                                        var arratItem = item.ToArray();
                                        byteList.Add(arratItem);


                                        SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs
                                        {
                                            RemoteEndPoint = socket.RemoteEndPoint,
                                            UserToken = null,
                                            AcceptSocket = null
                                        };

                                        socketEventArgs.SetBuffer(arratItem, 0, arratItem.Length);

                                        socketEventArgs.Completed += (sender, e) =>
                                        {
                                            if (e.SocketError != SocketError.Success)
                                            {
                                                _log.Info($"Failed to send data: {e.SocketError}");
                                            }
                                            else
                                            {
                                                Sending?.Invoke(item.CurrentSequence, item.TotalSequence, EnumTcpCommunication.MSG_PACKET_SENDING, $"{item.FileName}{item.FileExtension}");
                                                //Debug.WriteLine($"{EnumPacketType.LONG_MESSAGE} was sent completely.");
                                            }
                                        };

                                        // Client로 메시지 전송(비동기식)
                                        socket.SendAsync(socketEventArgs);

                                        Thread.Sleep(PACKET_SENDING_DELAY);

                                        //socket?.Send(arratItem);
                                        //Sending?.Invoke(item.CurrentSequence, item.TotalSequence, EnumTcpCommunication.MSG_PACKET_SENDING, $"{item.FileName}{item.FileExtension}");
                                        //await Task.Delay(PACKET_SENDING_DELAY);
                                    }
                                    SendCompleted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_SEND_COMPLETE);
                                }
                            }
                            break;
                        case EnumPacketType.FILE:
                        case EnumPacketType.MAP:
                        case EnumPacketType.PROFILE:
                        case EnumPacketType.VIDEO:
                            {
                                var packList = CreateFilePacket(sendMsg, type);
                                if (packList.Count > 1)
                                {
                                    var fileName = (new FileInfo(sendMsg)).Name;
                                    List<byte[]> byteList = new List<byte[]>();
                                    long transferByte = 0L;

                                    SendStarted?.Invoke(fileName, EnumTcpCommunication.FILE_PACKET_SEND_BEGINNING);
                                    foreach (var item in packList)
                                    {
                                        transferByte += item.BodyLength;
                                        var arratItem = item.ToArray();
                                        byteList.Add(arratItem);


                                        SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs
                                        {
                                            RemoteEndPoint = socket.RemoteEndPoint,
                                            UserToken = null,
                                            AcceptSocket = null
                                        };

                                        socketEventArgs.SetBuffer(arratItem, 0, arratItem.Length);

                                        socketEventArgs.Completed += (sender, e) =>
                                        {
                                            if (e.SocketError != SocketError.Success)
                                            {
                                                _log.Info($"Failed to send data: {e.SocketError}");
                                            }
                                            else
                                            {
                                                Sending?.Invoke(item.CurrentSequence, item.TotalSequence, EnumTcpCommunication.FILE_PACKET_SENDING, $"{item.FileName}{item.FileExtension}");
                                                //Debug.WriteLine($"{EnumPacketType.FILE} was sent completely.");
                                            }
                                        };

                                        // Client로 메시지 전송(비동기식)
                                        socket.SendAsync(socketEventArgs);


                                        Thread.Sleep(PACKET_SENDING_DELAY);
                                        //socket?.Send(arratItem);
                                        //Sending?.Invoke(item.CurrentSequence, item.TotalSequence, EnumTcpCommunication.FILE_PACKET_SENDING, $"{item.FileName}{item.FileExtension}");
                                        ////await Task.Delay(PACKET_SENDING_DELAY);
                                    }
                                    SendCompleted?.Invoke(fileName, EnumTcpCommunication.FILE_PACKET_SEND_COMPLETE);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (SocketException ex)
            {
                _log.Error($"Raised Exception in {nameof(SendPacketProcess)} for the reason : {ex.Message}");
            }
            finally
            {
                GC.Collect();
            }
        }

        public async Task ReceivedPacketProcess(byte[] receivedMsg)
        {
            try
            {
                await _semaphore.WaitAsync();

                var packet = ValidateCRC(receivedMsg);
                PacketCollectionClass<PacketClass> pCollectionClass;


                //Packet Insert In Queue(Dictionary)
                if (packet.TotalSequence > 0)
                {
                    PackQueue.TryGetValue(packet.Id.ToString(), out pCollectionClass);
                    if (pCollectionClass != null)
                    {
                        pCollectionClass.Add(packet);

                        ReceivingEvent(pCollectionClass?.Type, packet);

                        if (pCollectionClass.IsFullList)
                        {
                            CreateDataProcess(pCollectionClass);
                            PackQueue.Remove(packet.Id.ToString());

                            ReceiveCompletedEvent(pCollectionClass.Type, packet);
                        }
                    }
                    else
                    {
                        pCollectionClass = new PacketCollectionClass<PacketClass>();
                        pCollectionClass.Add(packet);
                        PackQueue.Add(packet.Id.ToString(), pCollectionClass);

                        ReceiveStartedEvent(pCollectionClass.Type, packet);
                    }
                }
                else
                {
                    //Short Message Without Status Event
                    pCollectionClass = new PacketCollectionClass<PacketClass>();
                    pCollectionClass.Add(packet);
                    PackQueue.Add(packet.Id.ToString(), pCollectionClass);

                    ReceiveCompletedEvent(pCollectionClass.Type, packet);
                    CreateDataProcess(pCollectionClass);
                    PackQueue.Remove(packet.Id.ToString());
                }
                Thread.Sleep(PACKET_RECEIVING_DELAY);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(ReceivedPacketProcess)} for the reason : {ex.Message}");
            }
            finally
            {
                GC.Collect();
                _semaphore.Release();
            }

        }

        private void ReceivingEvent(EnumPacketType? type, PacketClass packet)
        {
            if (type == null) return;

            switch (type)
            {
                case EnumPacketType.NONE:
                    break;
                case EnumPacketType.SHORT_MESSAGE:
                    break;
                case EnumPacketType.LONG_MESSAGE:
                    {
                        Receiving?.Invoke(packet.CurrentSequence, packet.TotalSequence, EnumTcpCommunication.MSG_PACKET_RECEIVING);
                    }
                    break;
                case EnumPacketType.FILE:
                case EnumPacketType.MAP:
                case EnumPacketType.PROFILE:
                case EnumPacketType.VIDEO:
                    {
                        Receiving?.Invoke(packet.CurrentSequence, packet.TotalSequence, EnumTcpCommunication.FILE_PACKET_RECEIVING, $"{packet.FileName}{packet.FileExtension}");
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReceiveStartedEvent(EnumPacketType type, PacketClass packet)
        {
            switch (type)
            {
                case EnumPacketType.NONE:
                    break;
                case EnumPacketType.SHORT_MESSAGE:
                    break;
                case EnumPacketType.LONG_MESSAGE:
                    {
                        ReceiveStarted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_RECEPTION_BEGINNIG);
                    }
                    break;
                case EnumPacketType.FILE:
                case EnumPacketType.MAP:
                case EnumPacketType.PROFILE:
                case EnumPacketType.VIDEO:
                    {
                        ReceiveStarted?.Invoke(packet.FileName, EnumTcpCommunication.FILE_PACKET_RECEPTION_BEGINNIG);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ReceiveCompletedEvent(EnumPacketType type, PacketClass packet)
        {
            switch (type)
            {
                case EnumPacketType.NONE:
                    break;
                case EnumPacketType.SHORT_MESSAGE:
                    break;
                case EnumPacketType.LONG_MESSAGE:
                    {
                        ReceiveStarted?.Invoke(null, EnumTcpCommunication.MSG_PACKET_RECEPTION_COMPLETE);
                    }
                    break;
                case EnumPacketType.FILE:
                case EnumPacketType.MAP:
                case EnumPacketType.PROFILE:
                case EnumPacketType.VIDEO:
                    {
                        ReceiveStarted?.Invoke(packet.FileName, EnumTcpCommunication.FILE_PACKET_RECEPTION_BEGINNIG);
                    }
                    break;
                default:
                    break;
            }
        }

        private async void CreateDataProcess(PacketCollectionClass<PacketClass> pCollectionClass)
        {
            var type = pCollectionClass.Type;

            byte[] buffer = new byte[pCollectionClass.MessageSize];

            switch (type)
            {
                case EnumPacketType.NONE:
                    throw new Exception($"{nameof(EnumPacketType)} is NONE, which should be set.");
                case EnumPacketType.SHORT_MESSAGE:
                    {
                        //Short Message Without Status Event

                        var packet = pCollectionClass.PacketList.FirstOrDefault();
                        Buffer.BlockCopy(packet.Body, 0, buffer, 0, packet.BodyLength);

                        var ret = Encoding.UTF8.GetString(buffer);
                        ReceiveCompleted?.Invoke(ret, EnumTcpCommunication.MSG_PACKET_RECEPTION_COMPLETE);
                        break;
                    }
                case EnumPacketType.LONG_MESSAGE:
                    {
                        var list = pCollectionClass.PacketList.OrderBy(entity => entity.CurrentSequence).ToList();
                        for (int i = 0; i < pCollectionClass.TotalSequence + 1; i++)
                        {
                            if (list[i] == null)
                                throw new Exception($"Missing packet for sequence {i}.");

                            Buffer.BlockCopy(list[i].Body, 0, buffer, PacketClass.BODY_SIZE * i, list[i].BodyLength);
                        }
                        var ret = Encoding.UTF8.GetString(buffer);
                        ReceiveCompleted?.Invoke(ret, EnumTcpCommunication.MSG_PACKET_RECEPTION_COMPLETE);
                        //return ret;
                        break;
                    }
                case EnumPacketType.FILE:
                    {
                        _dir = _currentDirectory + "/ReceiveFolder";
                        await FileGenerater(_dir, pCollectionClass, buffer);
                    }
                    break;
                case EnumPacketType.MAP:
                    {
                        _dir = _currentDirectory + "/Maps";
                        await FileGenerater(_dir, pCollectionClass, buffer);
                    }
                    break;
                case EnumPacketType.PROFILE:
                    {
                        _dir = _currentDirectory + "/Profiles";
                        await FileGenerater(_dir, pCollectionClass, buffer);
                    }
                    break;
                case EnumPacketType.VIDEO:
                    {

                    }
                    break;
                default:
                    throw new Exception("There are not relevant types for Converting Data");
            }

        }

        private Task<bool> FileGenerater(string dir, PacketCollectionClass<PacketClass> pCollectionClass, byte[] buffer)
        {
            return Task.Run(() =>
            {
                try
                {
                    var list = pCollectionClass.PacketList.OrderBy(entity => entity.CurrentSequence).ToList();
                    ReceiveCompleted?.Invoke($"{pCollectionClass.FileName} 생성 준비중 ...", EnumTcpCommunication.FILE_PACKET_RECEIVING);
                    for (int i = 0; i < pCollectionClass.TotalSequence + 1; i++)
                    {
                        //Debug.WriteLine($"생성된 바이트 수: {list}({}/{pCollectionClass.TotalSequence})");
                        Buffer.BlockCopy(list[i].Body, 0, buffer, PacketClass.BODY_SIZE * i, list[i].BodyLength);
                    }

                    if (!Directory.Exists(_dir))
                        Directory.CreateDirectory(_dir);

                    DirectoryInfo directoryInfo = new DirectoryInfo(_dir);

                    //Debug.WriteLine($"=================>{_dir}");
                    string fileName = $"{pCollectionClass.FileName}{pCollectionClass.FileExtension}"; // 파일을 저장할 경로를 지정합니다.
                    using (var stream = new FileStream(directoryInfo.FullName + "/" + fileName, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            // 파일 쓰기
                            stream.Write(buffer, 0, buffer.Length);
                            _log.Info($"{fileName} 만들기 생성 : {(new FileInfo(directoryInfo.FullName + "/" + fileName)).Exists}");
                        }
                        catch (Exception ex)
                        {
                            ReceiveCompleted?.Invoke($"Raised Exception during file generating : {ex.Message}", EnumTcpCommunication.COMMUNICATION_ERROR);
                        }
                    }
                    ReceiveCompleted?.Invoke(fileName, EnumTcpCommunication.FILE_PACKET_RECEPTION_COMPLETE);
                    return true;
                }
                catch (Exception ex)
                {
                    var msg = $"Raised Exception in {nameof(FileGenerater)} for the reason : {ex.Message}";
                    ReceiveCompleted?.Invoke(msg, EnumTcpCommunication.COMMUNICATION_ERROR);
                    _log.Error(msg);
                    return false;
                }
            });

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        Dictionary<string, PacketCollectionClass<PacketClass>> PackQueue { get; }
        #endregion
        #region - Attributes -
        const int MAX_NUM = 60000;

        const int PACKET_SENDING_DELAY = 20;
        const int PACKET_RECEIVING_DELAY = 15;

        private object _syncLock;
        private ILogService _log;
        private string _currentDirectory;
        private string _dir;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public delegate void CompletedMessage(object ret, EnumTcpCommunication type);
        public delegate void TransferMessage(int current, int total, EnumTcpCommunication type, string msg = null);
        public event CompletedMessage ReceiveStarted;
        public event TransferMessage Receiving;
        public event CompletedMessage ReceiveCompleted;
        public event CompletedMessage SendStarted;
        public event TransferMessage Sending;
        public event CompletedMessage SendCompleted;

        //public event CompletedMessage FileReceiveStarted;
        //public event TransferMessage FileReceiving;
        //public event CompletedMessage FileReceiveCompleted;
        //public event CompletedMessage FileSendStarted;
        //public event TransferMessage FileSending;
        //public event CompletedMessage FileSendCompleted;
        public ushort _count;
        #endregion
    }
}
