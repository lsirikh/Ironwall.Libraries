using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Api.Common.Models
{
    public class LoginAccountModel
    {
        public string UserId { get; set; }
        public int UserLevel { get; set; }
        public int ClientId { get; set; }
        public int Mode { get; set; }
        public string TimeCreated { get; set; }
    }
}
