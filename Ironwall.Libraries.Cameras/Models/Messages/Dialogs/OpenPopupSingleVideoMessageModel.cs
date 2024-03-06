using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models.Messages.Dialogs
{
    public class OpenPopupSingleVideoMessageModel
    {
        public OpenPopupSingleVideoMessageModel(ICameraDeviceModel camera)
        {

            Model = camera;

        }

        public ICameraDeviceModel Model { get; }
    }
}
