using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Models
{
    public class DeviceSetupModel
    {
        public string TableDevice => Properties.Settings.Default.TableDevice;
        public string TableController => Properties.Settings.Default.TableController;
        public string TableSensor => Properties.Settings.Default.TableSensor;
        public string TableCamera => Properties.Settings.Default.TableCamera;
        public string TableCategory => Properties.Settings.Default.TableCategory;
        public string TableDeviceInfo => Properties.Settings.Default.TableDeviceInfo;
        public string TableDeviceCameraPreset => Properties.Settings.Default.TableCameraPreset;
        public string TableDeviceCameraProfile => Properties.Settings.Default.TableCameraProfile;
        public string TableCameraMapping => Properties.Settings.Default.TableCameraMapping;
        public string TableMappingInfo => Properties.Settings.Default.TableMappingInfo;
    }
}
