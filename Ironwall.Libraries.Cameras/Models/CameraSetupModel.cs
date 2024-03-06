using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models
{
    public class CameraSetupModel
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string TableCameraDevice => Properties.Settings.Default.TableCameraDevice;
        public string TableCameraPreset => Properties.Settings.Default.TableCameraPreset;

        #endregion
        #region - Attributes -
        #endregion
    }
}
