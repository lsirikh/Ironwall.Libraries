using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;

namespace Ironwall.Libraries.Tcp.Common.ViewModels
{
    public interface ITcpUserViewModel
    {
        int Id { get; set; }
        string Address { get; set; }
        int Port { get; set; }

        string IdUser { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        string EmployeeNumber { get; set; }
        string Birth { get; set; }
        string Company { get; set; }
        string Department { get; set; }
        string EMail { get; set; }
        string Image { get; set; }
        string IpAddress { get; set; }
        int Level { get; set; }
        string Phone { get; set; }
        string Position { get; set; }
        bool Used { get; set; }
        ITcpModel TcpModel { get; set; }
        IUserModel UserModel { get; set; }

        public void Refresh();
    }
}