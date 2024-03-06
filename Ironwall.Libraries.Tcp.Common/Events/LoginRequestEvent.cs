using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Events
{
    public class LoginRequestEvent
    {
        public LoginRequestEvent()
        {
        }

        public LoginRequestModel Model { get; set; }
    }
}
