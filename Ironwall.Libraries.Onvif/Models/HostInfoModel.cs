using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.Models
{
    public class HostInfoModel : IHostInfoModel
    {
        public object Extension { get; set; }
        public bool FromDHCP { get; set; }
        public string Name { get; set; }
    }
}
