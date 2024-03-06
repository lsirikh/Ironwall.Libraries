using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.Models
{
    public class MediaUrlModel : IMediaUrlModel
    {
        public object Any { get; set; }
        public bool InvalidAfterConnect { get; set; }
        public bool InvalidAfterReboot { get; set; }
        public string Timeout { get; set; }
        public string Uri { get; set; }
    }
}
