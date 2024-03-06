using Ironwall.Libraries.Tcp.Common.Defines;
using System.Net;

namespace Ironwall.Libraries.Tcp.Client.Services
{
	public interface ITcpClient
	{
		event ITcpCommon.TcpConnect_dele Connceted;
		event ITcpCommon.TcpEvent_dele TcpEvent;
		event ITcpCommon.TcpDisconnect_dele Disconnected;

	}
}