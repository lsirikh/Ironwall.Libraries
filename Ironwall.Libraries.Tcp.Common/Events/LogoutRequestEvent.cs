using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.Events
{
    public class LogoutRequestEvent
    {
        public LogoutRequestEvent()
        {
        }
        public LogoutRequestModel Model { get; set; }
    }
}
