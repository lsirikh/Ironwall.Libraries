using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Api.Common.Models
{
    public class LoginSessionModel
    {
        public string UserId { get; set; }
        public string UserPass { get; set; }
        public string Token { get; set; }
        public string TimeCreated { get; set; }
        public string TimeExpired { get; set; }
    }
}
