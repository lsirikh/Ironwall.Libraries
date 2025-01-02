using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Ironwall.Libraries.Tcp.Common.Defines.ITcpCommon;

namespace Ironwall.Libraries.Tcp.Client.Services
{
	public abstract class TcpClientAsync
		: TcpSocket
	{
		#region - Ctors -
		public TcpClientAsync(TcpClientSetupModel model)
		{
			SetupModel = model;
		}
		#endregion
		#region - Implementation of Interface -
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
				Debug.WriteLine($"Raised Exception in InitSocket : {ex.Message}", typeof(TcpClient));
			}
		}

		private void CreateSocket(IPEndPoint serverIPEndPoint)
		{
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			Socket.LingerState = new LingerOption(true, 0);
			var endPoint = new IPEndPoint(IPAddress.Parse(SetupModel.ClientIp), SetupModel.ClientPort);
			//Socket.ExclusiveAddressUse = true;
			Socket.Bind(endPoint);
			Socket.BeginConnect(serverIPEndPoint, Connected_Completed, this);
		}

		private void Connected_Completed(IAsyncResult result)
		{
			// 접속 대기를 끝낸다.
			Socket.EndConnect(result);
			Mode = 1;

			Connceted();
			// buffer로 메시지를 받고 Receive함수로 메시지가 올 때까지 대기한다.
			Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive_Completed, this);
		}

		private void Receive_Completed(IAsyncResult result)
		{
			try
			{
				if (Socket.Connected)
				{
					// EndReceive를 호출하여 데이터 사이즈를 받는다.        
					// EndReceive는 대기를 끝내는 것이다.
					int size = Socket.EndReceive(result);

					//데이터를 string으로 변환한다.
					string msg = Encoding.UTF8.GetString(buffer, 0, size);
					// StringBuilder에 추가한다.
					sb.Append(msg.Trim('\0').Trim('\r', '\n'));

					if (sb.Length > 0)
					{
						Received(sb.ToString(), (IPEndPoint)(Socket).RemoteEndPoint);
						// StringBuilder의 내용을 비운다.
						sb.Clear();
						// 메시지가 오면 이벤트를 발생시킨다. (IOCP로 넣는 것)
					}

					// buffer로 메시지를 받고 Receive함수로 메시지가 올 때까지 대기한다.
					Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive_Completed, this);
				}
			}
			catch (SocketException ex)
			{
				Debug.WriteLine($"Raised SocketException Exception in Receive_Completed : {ex.Message}", typeof(TcpClient));
			}
			
		}

		public override void CloseSocket()
		{
			if (Socket.Connected)
			{
				Socket.BeginDisconnect(false, Disconnected_Completed, this);
			}
			else
			{
				Socket.Close();
				Socket.Dispose();

			}


			Disconnected();
		}

		private void Disconnected_Completed(IAsyncResult result)
		{
			Socket.Close();
			Socket.Dispose();

		}

		public override Task SendRequest(string msg, IPEndPoint selectedIp = null)
		{
			return Task.Run(() =>
			{
				try
				{
					byte[] sendData = Encoding.UTF8.GetBytes(msg);
					//데이터 길이 세팅

					// Client로 메시지 전송(비동기식)
					Socket.Send(sendData, sendData.Length, SocketFlags.None);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Raised Exception in SendRequest : {ex.Message}", typeof(TcpClient));
				}
			});
		}
		#endregion
		#region - Overrides -
		#endregion
		#region - Binding Methods -
		#endregion
		#region - Processes -
		public void SetServerIPEndPoint(TcpServerModel model)
		{
			ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(model.IpAddress), Convert.ToInt32(model.Port));
		}
		#endregion
		#region - IHanldes -
		#endregion
		#region - Properties -
		public TcpClientSetupModel SetupModel { get; }
		public IPEndPoint ServerIPEndPoint { get; private set; }
		public TcpClientModel Model { get; set; }
		#endregion
		#region - Attributes -

		// 소켓 생성

		// TCP 인터페이스 이벤트
		public event TcpConnect_dele Connceted;
		public event TcpReceive_dele Received;
		public event TcpDisconnect_dele Disconnected;

		private byte[] buffer = new byte[1024];
		#endregion

	}
}
