using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public class TcpUserModel : ITcpUserModel
    {
        public TcpUserModel()
        {

        }

        public TcpUserModel(int id, ITcpModel tcpModel, IUserModel userModel)
        {
            Id = id;
            TcpModel = tcpModel;
            UserModel = userModel;
        }

        public int Id { get; set; }
        public ITcpModel TcpModel { get; set; }
        public IUserModel UserModel { get; set; }
    }
}
