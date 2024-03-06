using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models
{
    public class RtspLookupViewModel : IRtspLookupViewModel
    {
        public CameraDeviceViewModel CameraDeviceViewModel { get; set; }
        public CameraPresetViewModel CameraPresetViewModel { get; set; }
    }
}
