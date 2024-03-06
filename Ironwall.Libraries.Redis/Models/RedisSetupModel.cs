using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Redis.Models
{
    public class RedisSetupModel
    {
        public string PasswordRedis { get; set; }
        public string IpAddressRedis { get; set; }
        public int PortRedis { get; set; }
        public string NameChannel { get; set; }
    }
}
