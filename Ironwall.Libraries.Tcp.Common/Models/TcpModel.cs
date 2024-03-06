namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpModel : ITcpModel
    {
        public TcpModel()
        {

        }

        public TcpModel(int id, string ipAddress, int port)
        {
            Id = id;
            IpAddress = ipAddress;
            Port = port;
        }

        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}