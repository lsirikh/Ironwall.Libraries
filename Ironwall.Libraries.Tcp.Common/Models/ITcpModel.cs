namespace Ironwall.Libraries.Tcp.Common.Models
{
    public interface ITcpModel
    {
        int Id { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}