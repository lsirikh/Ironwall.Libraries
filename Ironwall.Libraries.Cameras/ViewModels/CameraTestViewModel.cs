using Ironwall.Libraries.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public class CameraTestViewModel
        : CameraBaseViewModel
    {
        public CameraTestViewModel()
        {

        }

        public CameraTestViewModel(ICameraDeviceModel model)
        {

        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        public ICameraDeviceModel Model 
        { 
            get { return base._model as ICameraDeviceModel; }
            set
            {
                base._model = value;
            }
        }
    }
}
