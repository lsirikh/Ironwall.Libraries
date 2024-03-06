using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Models
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
        public int PtzTimeOut
        {
            get => _ptzTimeOut;
            set
            { 

                _ptzTimeOut = value;
                Properties.Settings.Default.PtzTimeOut = _ptzTimeOut;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsPopupTimer
        {
            get => _isPopupTimer;
            set
            {
                _isPopupTimer = value;
                Properties.Settings.Default.IsPopupTimer = _isPopupTimer;
                Properties.Settings.Default.Save();
            }
        }
        public bool IsCameraPopup
        {
            get => _isCameraPopup;
            set
            {
                _isCameraPopup = value;
                Properties.Settings.Default.IsCameraPopup = _isCameraPopup;
                Properties.Settings.Default.Save();
            }
        }

        public string TableCameraDevice => Properties.Settings.Default.TableCameraDevice;
        public string TableCameraPreset => Properties.Settings.Default.TableCameraPreset;

        #endregion
        #region - Attributes -
        private bool _isPopupTimer = Properties.Settings.Default.IsPopupTimer;
        private bool _isCameraPopup = Properties.Settings.Default.IsCameraPopup;
        private int _ptzTimeOut = Properties.Settings.Default.PtzTimeOut;
        #endregion
    }
}
