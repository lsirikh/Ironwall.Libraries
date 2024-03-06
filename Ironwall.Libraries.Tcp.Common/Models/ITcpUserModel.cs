using Ironwall.Framework.Models.Accounts;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public interface ITcpUserModel
    {
        int Id { get; set; }
        ITcpModel TcpModel { get; set; }
        IUserModel UserModel { get; set; }
    }
}