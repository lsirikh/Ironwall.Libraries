using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models.Messages.Dialogs
{
    public class OpenPopupDoubleVideoMessageModel
    {
        public OpenPopupDoubleVideoMessageModel(ICameraDeviceModel cameraFirst, ICameraDeviceModel cameraSecond)
        {

            Model1 = cameraFirst;
            Model2 = cameraSecond;

        }

        public ICameraDeviceModel Model1 { get; }
        public ICameraDeviceModel Model2 { get; }
    }
}
