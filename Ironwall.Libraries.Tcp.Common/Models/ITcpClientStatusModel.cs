namespace Ironwall.Libraries.Tcp.Common.Models
{
    public interface ITcpClientStatusModel
    {
        bool IsConnected { get; set; }
        string Status { get; set; }
    }
}