using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Models.Messages.Dialogs
{
    public class OpenPopupDoubleVideoMessageModel
    {
        public OpenPopupDoubleVideoMessageModel(CameraDeviceModel cameraFirst, CameraDeviceModel cameraSecond)
        {

            Model1 = cameraFirst;
            Model2 = cameraSecond;

        }

        public CameraDeviceModel Model1 { get; }
        public CameraDeviceModel Model2 { get; }
    }
}
