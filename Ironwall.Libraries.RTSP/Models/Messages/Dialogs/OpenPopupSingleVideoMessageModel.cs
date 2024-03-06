using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Models.Messages.Dialogs
{
    public class OpenPopupSingleVideoMessageModel
    {
        public OpenPopupSingleVideoMessageModel(CameraDeviceModel camera)
        {

            Model = camera;

        }

        public CameraDeviceModel Model { get; }
    }
}
