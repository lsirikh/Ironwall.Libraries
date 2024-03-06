using Ironwall.Libraries.RTSP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Models
{
    public class RtspLookupViewModel : IRtspLookupViewModel
    {
        public CameraDeviceViewModel CameraDeviceViewModel { get; set; }
        public CameraPresetViewModel CameraPresetViewModel { get; set; }
    }
}
