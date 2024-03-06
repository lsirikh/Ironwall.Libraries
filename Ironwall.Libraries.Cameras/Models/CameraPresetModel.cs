using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models
{
    public class CameraPresetModel 
        : CameraBaseModel
        , ICameraPresetModel
    {
        //public int Id { get; set; }
        public string NameArea { get; set; }
        public int IdController { get; set; }
        public int IdSensorBgn { get; set; }
        public int IdSensorEnd { get; set; }

        public string CameraFirst { get; set; }
        public int TypeDeviceFirst { get; set; }
        public string HomePresetFirst { get; set; }
        public string TargetPresetFirst { get; set; }

        public string CameraSecond { get; set; }
        public int TypeDeviceSecond { get; set; }
        public string HomePresetSecond { get; set; }
        public string TargetPresetSecond { get; set; }

        public int ControlTime { get; set; }


    }
}
